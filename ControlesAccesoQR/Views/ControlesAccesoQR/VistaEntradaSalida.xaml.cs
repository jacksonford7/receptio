using System;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using ControlesAccesoQR;
using ControlesAccesoQR.ViewModels.ControlesAccesoQR;
using ControlesAccesoQR.Models;
using ControlesAccesoQR.Services;
using ControlesAccesoQR.ViewModels;
using EstadoProcesoTipo = ControlesAccesoQR.Models.EstadoProceso;
using Transaction.ServicioTransaction;

namespace ControlesAccesoQR.Views.ControlesAccesoQR
{
    public partial class VistaEntradaSalida : UserControl
    {
        private readonly StringBuilder _qrBuffer = new StringBuilder();
        private readonly DispatcherTimer _qrTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000) };
        private string _lastScan = string.Empty;
        private DateTime _lastScanTime = DateTime.MinValue;
        private CancellationTokenSource _qrCts;
        private bool _autoSubmitting;
        private readonly IFingerprintWorkflow _fingerprintWorkflow = new FingerprintWorkflow();
        private CancellationTokenSource _cts;

        public VistaEntradaSalida()
        {
            InitializeComponent();
            _qrTimer.Tick += QrTimer_Tick;
            Loaded += VistaEntradaSalida_Loaded;
            Unloaded += VistaEntradaSalida_Unloaded;

            Loaded += (_, __) => { QrInput?.Focus(); };
            DataContextChanged += (_, __) =>
            {
                if (DataContext is INotifyPropertyChanged npc)
                {
                    npc.PropertyChanged -= OnVmPropertyChanged;
                    npc.PropertyChanged += OnVmPropertyChanged;
                }
            };
        }

        private void OnVmPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(VistaEntradaSalidaViewModel.QrValue) || e.PropertyName == "QrValue")
            {
                DebouncedAutoIngresar(200); // evita dobles envíos (Enter + cambio texto)
            }
        }

        private void DebouncedAutoIngresar(int delayMs)
        {
            _qrCts?.Cancel();
            _qrCts = new CancellationTokenSource();
            var token = _qrCts.Token;

            Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(delayMs, token);
                    if (token.IsCancellationRequested) return;

                    await Dispatcher.InvokeAsync(() =>
                    {
                        if (_autoSubmitting) return;
                        _autoSubmitting = true;
                        try
                        {
                            var vm = DataContext;
                            var cmd = (vm?.GetType().GetProperty("IngresarCommand")?.GetValue(vm)) as ICommand;
                            var param = vm?.GetType().GetProperty("QrValue")?.GetValue(vm);

                            if (cmd != null && cmd.CanExecute(param))
                            {
                                cmd.Execute(param);
                            }
                            else
                            {
                                IngresarButton?.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                            }
                        }
                        finally
                        {
                            _autoSubmitting = false;
                        }
                    });
                }
                catch (TaskCanceledException) { /* no-op */ }
            }, token);
        }

        private void VistaEntradaSalida_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window != null)
                window.PreviewKeyDown += Window_PreviewKeyDown;
        }

        private void VistaEntradaSalida_Unloaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window != null)
                window.PreviewKeyDown -= Window_PreviewKeyDown;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.FocusedElement is TextBox)
                return;

            if (e.Key == Key.Enter)
            {
                _qrTimer.Stop();
                var text = _qrBuffer.ToString();
                _qrBuffer.Clear();
                if (string.IsNullOrWhiteSpace(text))
                    return;

                var now = DateTime.UtcNow;
                if (text == _lastScan && (now - _lastScanTime).TotalMilliseconds < 200)
                    return;

                _lastScan = text;
                _lastScanTime = now;

                if (DataContext is VistaEntradaSalidaViewModel vm)
                    vm.SubmitPass(text, "qr");
                e.Handled = true;
            }
            else
            {
                char c = KeyToChar(e.Key);
                if (c != '\0')
                {
                    _qrBuffer.Append(c);
                    _qrTimer.Stop();
                    _qrTimer.Start();
                }
            }
        }

        private void QrTimer_Tick(object sender, EventArgs e)
        {
            _qrTimer.Stop();
            _qrBuffer.Clear();
        }

        private static char KeyToChar(Key key)
        {
            if (key >= Key.D0 && key <= Key.D9)
                return (char)('0' + (key - Key.D0));
            if (key >= Key.NumPad0 && key <= Key.NumPad9)
                return (char)('0' + (key - Key.NumPad0));
            if (key >= Key.A && key <= Key.Z)
                return (char)('A' + (key - Key.A));
            return '\0';
        }

        private async void IngresarButton_Click(object sender, RoutedEventArgs e)
        {
            _qrCts?.Cancel();
            if (DataContext is VistaEntradaSalidaViewModel vm)
            {
                if (vm.IngresarCommand.CanExecute(null))
                    await vm.IngresarCommand.ExecuteAsync();

                if (!vm.IngresoRealizado)
                    return;

                vm.ChoferID = "0920316932";

                if (!string.IsNullOrWhiteSpace(vm.ChoferID))
                {
                    if (DevBypass.IsDevKiosk)
                    {
                        await CompletarValidacionHuellaAsync(vm, "BYPASS CGDE041", 1);
                        var estadoH = await vm.ActualizarEstadoAsync("H");
                        if (estadoH == null)
                            return;
                        MessageBox.Show("Huella validada");

                        await CompletarLecturaRfidAsync(vm, "TAG_SIMULADO");
                        if (await vm.ActualizarEstadoAsync("R") == null)
                            return;
                        MessageBox.Show("RFID detectado");

                        await EjecutarImpresionAsync(vm);
                        return;
                    }
                    _cts = new CancellationTokenSource();
                    FingerprintPanel.DataContext = new FingerprintPanelViewModel(_fingerprintWorkflow, _cts);
                    FingerprintPanel.Visibility = Visibility.Visible;

                    Guid choferGuid = Guid.TryParse(vm.ChoferID, out var g) ? g : Guid.Empty;
                    var decision = await _fingerprintWorkflow.ValidateAsync(choferGuid, _cts.Token);
                    FingerprintPanel.Visibility = Visibility.Collapsed;
                    await CompletarValidacionHuellaAsync(vm, decision.Resultado?.ToString() ?? string.Empty, decision.IsValid ? 1 : 0);

                    if (!decision.IsValid)
                        return;

                    // 2) Estado = H (se espera objeto con PlacaCamion, igual a tu flujo actual)
                    var resultadoH = await vm.ActualizarEstadoAsync("H");
                    if (resultadoH == null)
                        return;

                    // 3) Obtener tag desde Transaction (usando la placa del resultado H)
                    string tagBaseDatos = null;
                    using (var servicio = new ServicioTransactionClient())
                    {
                        tagBaseDatos = await servicio.ObtenerTagAsync(resultadoH.PlacaCamion);
                    }

                    // 4) Lectura RFID con tag obtenido
                    await CompletarLecturaRfidAsync(vm, tagBaseDatos);

                    // 5) Estado = R
                    if (await vm.ActualizarEstadoAsync("R") == null)
                        return;

                    // 6) Impresión
                    await EjecutarImpresionAsync(vm);

                }
            }
        }

        private async void ImprimirButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is VistaEntradaSalidaViewModel vm)
                await EjecutarImpresionAsync(vm);
        }

        private async Task EjecutarImpresionAsync(VistaEntradaSalidaViewModel vm)
        {
            if (DevBypass.IsDevKiosk)
            {
                await CompletarImpresionAsync(vm);
                if (await vm.ActualizarEstadoAsync("P") == null)
                    return;
                MessageBox.Show("Impresión simulada (CGDE041)");
            }
            else
            {
                await CompletarImpresionAsync(vm);
                if (await vm.ActualizarEstadoAsync("P") == null)
                    return;
                LimpiarFormularioPostProceso();
            }
        }

        private Task CompletarValidacionHuellaAsync(VistaEntradaSalidaViewModel vm, string respuesta, int messageId)
        {
            vm.MainViewModel.Procesos.Add(new Proceso
            {
                STEP = "HUELLA",
                RESPONSE = respuesta,
                MESSAGE_ID = messageId
            });

            vm.MainViewModel.EstadoProceso = EstadoProcesoTipo.IngresoRegistrado;
            return Task.CompletedTask;
        }

        private async Task CompletarLecturaRfidAsync(VistaEntradaSalidaViewModel vm, string tagEsperado)
        {
            if (!DevBypass.IsDevKiosk)
            {
                var rfidOk = await vm.ValidarRfidAsync(tagEsperado);
                if (!rfidOk)
                    MessageBox.Show(vm.RfidMensaje, "RFID", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            vm.MainViewModel.Procesos.Add(new Proceso
            {
                STEP = "RFID",
                RESPONSE = tagEsperado ?? string.Empty,
                MESSAGE_ID = 1
            });
        }

        private Task CompletarImpresionAsync(VistaEntradaSalidaViewModel vm)
        {
            if (vm.ImprimirQrCommand.CanExecute(null))
                vm.ImprimirQrCommand.Execute(null);
            return Task.CompletedTask;
        }

        private void LimpiarFormularioPostProceso()
        {
            // Mantener limpio el formulario cuando corresponda en el flujo real
        }

    }
}

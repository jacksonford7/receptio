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
using EstadoProcesoTipo = ControlesAccesoQR.Models.EstadoProceso;

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

                vm.ChoferID = "1";

                if (!string.IsNullOrWhiteSpace(vm.ChoferID))
                {
                    if (DevBypass.IsDevKiosk)
                    {
                        await CompletarValidacionHuellaAsync(vm, "BYPASS CGDE041", 1);
                        if (!await vm.ActualizarEstadoAsync("H"))
                            return;
                        MessageBox.Show("Huella validada");

                        await CompletarLecturaRfidAsync(vm, "TAG_SIMULADO");
                        if (!await vm.ActualizarEstadoAsync("R"))
                            return;
                        MessageBox.Show("RFID detectado");
                        return;
                    }

                    await CompletarValidacionHuellaAsync(vm, "VALIDACION AUTOMATICA", 1);

                    if (!await vm.ActualizarEstadoAsync("H"))
                        return;
                    await CompletarLecturaRfidAsync(vm, null);
                    if (!await vm.ActualizarEstadoAsync("R"))
                        return;
                }
            }
        }

        private async void ImprimirButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is VistaEntradaSalidaViewModel vm)
            {
                if (DevBypass.IsDevKiosk)
                {
                    await CompletarImpresionAsync(vm);
                    if (!await vm.ActualizarEstadoAsync("P"))
                        return;
                    MessageBox.Show("Impresión simulada (CGDE041)");
                    return;
                }

                await CompletarImpresionAsync(vm);
                if (!await vm.ActualizarEstadoAsync("P"))
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

        private async Task CompletarLecturaRfidAsync(VistaEntradaSalidaViewModel vm, string tag)
        {
            if (!DevBypass.IsDevKiosk)
            {
                var rfidOk = await vm.ValidarRfidAsync();
                if (!rfidOk)
                    MessageBox.Show(vm.RfidMensaje, "RFID", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            vm.MainViewModel.Procesos.Add(new Proceso
            {
                STEP = "RFID",
                RESPONSE = tag ?? string.Empty,
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

using System;
using System.Text;
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
        private readonly StringBuilder _scannerBuffer = new StringBuilder();
        private readonly DispatcherTimer _scannerTimer;
        private DateTime _lastKeystroke;
        private DateTime _lastSubmission;
        private string _lastScan = string.Empty;

        public VistaEntradaSalida()
        {
            InitializeComponent();
            _scannerTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
            _scannerTimer.Tick += ScannerTimer_Tick;
        }

        private void UserControl_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var now = DateTime.Now;
            if ((now - _lastKeystroke).TotalMilliseconds > 50)
                _scannerBuffer.Clear();

            _lastKeystroke = now;
            _scannerBuffer.Append(e.Text);
            _scannerTimer.Stop();
            _scannerTimer.Start();
        }

        private async void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            e.Handled = true;

            if (_scannerTimer.IsEnabled)
            {
                _scannerTimer.Stop();
                var scanned = _scannerBuffer.ToString();
                _scannerBuffer.Clear();
                await SubmitScannerAsync(scanned);
            }
            else if (DataContext is VistaEntradaSalidaViewModel vm)
            {
                await vm.SubmitPassAsync(vm.CodigoQR, VistaEntradaSalidaViewModel.InputMethod.Keyboard);
            }
        }

        private async void ScannerTimer_Tick(object sender, EventArgs e)
        {
            _scannerTimer.Stop();
            var scanned = _scannerBuffer.ToString();
            _scannerBuffer.Clear();
            await SubmitScannerAsync(scanned);
        }

        private async Task SubmitScannerAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Length < 4)
                return;

            if ((DateTime.Now - _lastSubmission).TotalMilliseconds < 200 && text == _lastScan)
                return;

            _lastSubmission = DateTime.Now;
            _lastScan = text;

            if (DataContext is VistaEntradaSalidaViewModel vm)
                await vm.SubmitPassAsync(text, VistaEntradaSalidaViewModel.InputMethod.QrScanner);
        }

        private async void IngresarButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is VistaEntradaSalidaViewModel vm)
            {
                if (vm.IngresarCommand is AsyncRelayCommand arc && arc.CanExecute(null))
                    await arc.ExecuteAsync();

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

                    var dialogo = new DialogoHuella { Owner = Window.GetWindow(this), DataContext = new HuellaViewModel(vm.ChoferID) };
                    if (dialogo.ShowDialog() == true && dialogo.DataContext is HuellaViewModel hv)
                    {
                        await CompletarValidacionHuellaAsync(vm, hv.Resultado, hv.HuellaValida ? 1 : 0);

                        if (hv.HuellaValida)
                        {
                            if (!await vm.ActualizarEstadoAsync("H"))
                                return;
                            await CompletarLecturaRfidAsync(vm, null);
                            if (!await vm.ActualizarEstadoAsync("R"))
                                return;
                        }
                    }
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

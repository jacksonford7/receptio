using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ControlesAccesoQR;
using ControlesAccesoQR.ViewModels.ControlesAccesoQR;
using ControlesAccesoQR.Models;
using EstadoProcesoTipo = ControlesAccesoQR.Models.EstadoProceso;

namespace ControlesAccesoQR.Views.ControlesAccesoQR
{
    public partial class VistaEntradaSalida : UserControl
    {
        public VistaEntradaSalida()
        {
            InitializeComponent();
        }

        private async void IngresarButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is VistaEntradaSalidaViewModel vm)
            {
                if (vm.IngresarCommand.CanExecute(null))
                    vm.IngresarCommand.Execute(null);

                if (!string.IsNullOrWhiteSpace(vm.ChoferID))
                {
                    if (DevBypass.IsDevKiosk)
                    {
                        await CompletarValidacionHuellaAsync(vm, "BYPASS CGDE041", 1);
                        MessageBox.Show("Huella validada");
                        await vm.ActualizarEstadoAsync("H");

                        await CompletarLecturaRfidAsync(vm, "TAG_SIMULADO");
                        MessageBox.Show("RFID detectado");
                        await vm.ActualizarEstadoAsync("R");
                        return;
                    }

                    var dialogo = new DialogoHuella { Owner = Window.GetWindow(this), DataContext = new HuellaViewModel(vm.ChoferID) };
                    if (dialogo.ShowDialog() == true && dialogo.DataContext is HuellaViewModel hv)
                    {
                        await CompletarValidacionHuellaAsync(vm, hv.Resultado, hv.HuellaValida ? 1 : 0);

                        if (hv.HuellaValida)
                        {
                            await vm.ActualizarEstadoAsync("H");
                            await CompletarLecturaRfidAsync(vm, null);
                            await vm.ActualizarEstadoAsync("R");
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
                    MessageBox.Show("Impresión simulada (CGDE041)");
                    await vm.ActualizarEstadoAsync("P");
                    return;
                }

                await CompletarImpresionAsync(vm);
                await vm.ActualizarEstadoAsync("P");
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

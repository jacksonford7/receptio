using System.Windows;
using System.Windows.Controls;
using ControlesAccesoQR.ViewModels.ControlesAccesoQR;
using ControlesAccesoQR.Views.ControlesAccesoQR;
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

        private void EscanearQrButton_Click(object sender, RoutedEventArgs e)
        {
            var dialogo = new DialogoQr { Owner = Window.GetWindow(this) };
            if (dialogo.ShowDialog() == true)
            {
                if (DataContext is VistaEntradaSalidaViewModel vm)
                {
                    vm.NumeroPase = dialogo.NumeroPase;
                    if (vm.EscanearQrCommand.CanExecute(null))
                        vm.EscanearQrCommand.Execute(null);
                }
            }
        }

        private void IngresarButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is VistaEntradaSalidaViewModel vm)
            {
                if (vm.IngresarCommand.CanExecute(null))
                    vm.IngresarCommand.Execute(null);

                if (!string.IsNullOrWhiteSpace(vm.ChoferID))
                {
                    var dialogo = new DialogoHuella { Owner = Window.GetWindow(this), DataContext = new HuellaViewModel(vm.ChoferID) };
                    if (dialogo.ShowDialog() == true && dialogo.DataContext is HuellaViewModel hv)
                    {
                        vm.MainViewModel.Procesos.Add(new Proceso
                        {
                            STEP = "HUELLA",
                            RESPONSE = hv.Resultado,
                            MESSAGE_ID = hv.HuellaValida ? 1 : 0
                        });

                        if (hv.HuellaValida)
                            vm.MainViewModel.EstadoProceso = EstadoProceso.IngresoRegistrado;
                    }
                }
            }
        }
    }
}

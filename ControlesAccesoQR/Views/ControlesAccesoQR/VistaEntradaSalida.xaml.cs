using System.Windows;
using System.Windows.Controls;
using ControlesAccesoQR.ViewModels.ControlesAccesoQR;
using ControlesAccesoQR.Views.ControlesAccesoQR;

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
                    vm.QrIngresado = dialogo.NumeroPase;
                    if (vm.EscanearQrCommand.CanExecute(null))
                        vm.EscanearQrCommand.Execute(null);
                }
            }
        }
    }
}

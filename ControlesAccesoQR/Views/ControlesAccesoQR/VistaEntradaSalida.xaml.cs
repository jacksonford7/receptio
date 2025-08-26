using System.Windows.Controls;
using ControlesAccesoQR.ViewModels.ControlesAccesoQR;

namespace ControlesAccesoQR.Views.ControlesAccesoQR
{
    public partial class VistaEntradaSalida : UserControl
    {
        public VistaEntradaSalida(MainWindowViewModel mainViewModel)
        {
            InitializeComponent();
            DataContext = new VistaEntradaSalidaViewModel(mainViewModel);
            Loaded += (_, __) => { QrInput?.Focus(); };
        }
    }
}


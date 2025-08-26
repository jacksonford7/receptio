using System.Windows.Controls;
using ControlesAccesoQR.ViewModels.ControlesAccesoQR;
using ControlesAccesoQR.accesoDatos;
using ControlesAccesoQR.Servicios;

namespace ControlesAccesoQR.Views.ControlesAccesoQR
{
    public partial class VistaEntradaSalida : UserControl
    {
        public VistaEntradaSalida(MainWindowViewModel mainViewModel)
        {
            InitializeComponent();
            DataContext = new VistaEntradaSalidaViewModel(
                new PasePuertaDataAccess(),
                new EstadoService(),
                new HuellaService(),
                new RfidReader(),
                new PrintService(),
                mainViewModel);
            Loaded += (_, __) => { QrInput?.Focus(); };
        }
    }
}


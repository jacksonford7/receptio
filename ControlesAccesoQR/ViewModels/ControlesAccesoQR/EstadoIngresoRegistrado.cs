using System.Windows;

namespace ControlesAccesoQR.ViewModels.ControlesAccesoQR
{
    internal class EstadoIngresoRegistrado : EstadoProceso
    {
        internal override void EstablecerControles(MainWindowViewModel viewModel)
        {
            Titulo = "INGRESO REGISTRADO";
            EsVisibleBoton = Visibility.Collapsed;
        }

        internal override void CambiarEstado()
        {
            // No se avanza automáticamente en este ejemplo
        }
    }
}

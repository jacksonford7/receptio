using System.Windows;

namespace ControlesAccesoQR.ViewModels.ControlesAccesoQR
{
    internal class EstadoSalidaRegistrada : EstadoProceso
    {
        internal override void EstablecerControles(MainWindowViewModel viewModel)
        {
            Titulo = "SALIDA REGISTRADA";
            EsVisibleBoton = Visibility.Collapsed;
        }

        internal override void CambiarEstado()
        {
            // Una vez registrada la salida, no hay cambio automático
        }
    }
}

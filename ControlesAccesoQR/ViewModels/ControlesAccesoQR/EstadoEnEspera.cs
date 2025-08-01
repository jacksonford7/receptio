using System.Windows;

namespace ControlesAccesoQR.ViewModels.ControlesAccesoQR
{
    internal class EstadoEnEspera : EstadoProceso
    {
        internal override void EstablecerControles(MainWindowViewModel viewModel)
        {
            Titulo = "EN ESPERA";
            Mensaje = string.Empty;
            EsVisibleBoton = Visibility.Collapsed;
        }

        internal override void CambiarEstado()
        {
            // No cambia automáticamente a otro estado
        }
    }
}

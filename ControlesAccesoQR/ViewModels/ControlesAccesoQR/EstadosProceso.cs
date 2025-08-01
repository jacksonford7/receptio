using System.Windows;

namespace ControlesAccesoQR.ViewModels.ControlesAccesoQR
{
    /// <summary>
    /// Estado inicial del proceso. Solo muestra la pantalla en espera.
    /// </summary>
    internal class EstadoEnEspera : EstadoProceso
    {
        internal override void EstablecerControles(MainWindowViewModel viewModel)
        {
            // No se realiza ninguna acción específica para este estado.
        }

        internal override void CambiarEstado()
        {
            // No se define transición.
        }
    }

    /// <summary>
    /// Estado que indica que el ingreso fue registrado.
    /// </summary>
    internal class EstadoIngresoRegistrado : EstadoProceso
    {
        internal override void EstablecerControles(MainWindowViewModel viewModel)
        {
            // Lógica simplificada para el ejemplo.
        }

        internal override void CambiarEstado()
        {
            // Transición no definida.
        }
    }

    /// <summary>
    /// Estado que indica que la salida fue registrada.
    /// </summary>
    internal class EstadoSalidaRegistrada : EstadoProceso
    {
        internal override void EstablecerControles(MainWindowViewModel viewModel)
        {
            // Lógica simplificada para el ejemplo.
        }

        internal override void CambiarEstado()
        {
            // Transición no definida.
        }
    }
}

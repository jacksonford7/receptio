using System;

namespace ControlesAccesoQR.Servicios
{
    public static class EstadoPanelEvents
    {
        public static event Action<string> EstadoCodigoCambiado;

        public static void RaiseEstadoCodigoCambiado(string codigo)
        {
            var handler = EstadoCodigoCambiado;
            if (handler != null) handler(codigo);
        }
    }
}

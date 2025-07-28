using System.Collections.Generic;

namespace RECEPTIO.CapaPresentacion.UI.Interfaces.RFID
{
    public interface IAntena
    {
        bool ConectarAntena();
        void IniciarLectura();
        List<string> TerminarLectura();
        List<string> ObtenerTagsLeidos();
        void DesconectarAntena();
        void Dispose();
    }
}

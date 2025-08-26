using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RECEPTIO.CapaPresentacion.UI.Interfaces.RFID;
using Spring.Context.Support;

namespace ControlesAccesoQR.Servicios
{
    public class RfidReader : IRfidReader
    {
        public async Task<string> LeerAsync(CancellationToken ct)
        {
            if (DevBypass.IsDevKiosk)
                return "SIMULADO-0001";

            IAntena antena = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Springs/SpringAntena.xml");
                antena = (IAntena)ctx["AdministradorAntena"];
                if (!antena.ConectarAntena())
                    return null;

                antena.IniciarLectura();
                await Task.Delay(1000, ct);
                List<string> tags = antena.ObtenerTagsLeidos();
                return tags != null && tags.Count > 0 ? tags[0] : null;
            }
            finally
            {
                antena?.TerminarLectura();
                antena?.DesconectarAntena();
                antena?.Dispose();
            }
        }
    }
}

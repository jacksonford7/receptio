using System.Threading;
using System.Threading.Tasks;
using ControlesAccesoQR.accesoDatos;
using ControlesAccesoQR.Models;

// Alias canónico
using EstadoProcesoEnum = ControlesAccesoQR.Models.EstadoProceso;

namespace ControlesAccesoQR.Servicios
{
    public class EstadoService : IEstadoService
    {
        private readonly PasePuertaDataAccess _dataAccess;

        public EstadoService()
        {
            _dataAccess = new PasePuertaDataAccess();
        }

        public Task<ActualizarEstadoResult> ActualizarAsync(string numeroPase, string estado, CancellationToken ct = default)
        {
            return _dataAccess.ActualizarEstadoAsync(numeroPase, estado, ct);
        }

        public void Set(EstadoProcesoEnum estado)
        {
            EstadoPanelEvents.RaiseEstadoCodigoCambiado(estado.ToString());
        }
    }
}

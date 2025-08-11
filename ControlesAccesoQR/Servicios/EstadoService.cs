using System.Threading;
using System.Threading.Tasks;
using ControlesAccesoQR.accesoDatos;
using ControlesAccesoQR.Models;

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
    }
}

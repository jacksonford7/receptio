using System.Threading;
using System.Threading.Tasks;
using ControlesAccesoQR.Models;

// Alias canónico
using EstadoProcesoEnum = ControlesAccesoQR.Models.EstadoProceso;

namespace ControlesAccesoQR.Servicios
{
    public interface IEstadoService
    {
        Task<ActualizarEstadoResult> ActualizarAsync(string numeroPase, string estado, CancellationToken ct = default);
        void Set(EstadoProcesoEnum estado);
    }
}

using System.Threading;
using System.Threading.Tasks;
using ControlesAccesoQR.Models;
using ControlesAccesoQR.Estados;

namespace ControlesAccesoQR.Servicios
{
    public interface IEstadoService
    {
        Task<ActualizarEstadoResult> ActualizarAsync(string numeroPase, string estado, CancellationToken ct = default);
        void Set(EstadoProceso estado);
    }
}

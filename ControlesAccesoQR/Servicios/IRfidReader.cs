using System.Threading;
using System.Threading.Tasks;

namespace ControlesAccesoQR.Servicios
{
    public interface IRfidReader
    {
        Task<string> LeerAsync(CancellationToken ct);
    }
}

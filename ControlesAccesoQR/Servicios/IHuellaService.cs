using System.Threading.Tasks;

namespace ControlesAccesoQR.Servicios
{
    public interface IHuellaService
    {
        Task<bool> CapturarYValidarAsync(string choferId);
    }
}

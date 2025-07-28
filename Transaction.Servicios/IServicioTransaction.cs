using RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Interfaces;
using RECEPTIO.CapaAplicacion.Transaction.Aplicacion.Interfaces;
using System.ServiceModel;

namespace RECEPTIO.CapaServiciosDistribuidos.Transaction.Servicios
{
    [ServiceContract]
    public interface IServicioTransaction :IProcesosN4, IError, IPreGate, ITag, ITransaccionQuiosco, ILoginBase, IComunKiosco
    {
    }
}

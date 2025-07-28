using RECEPTIO.CapaAplicacion.Mobile.Aplicacion.Interfaces;
using RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Interfaces;
using System.ServiceModel;

namespace RECEPTIO.CapaServiciosDistribuidos.Mobile.Servicios
{
    [ServiceContract]
    public interface IServicioMobile : IProcesoN4, IPrincipal, IError
    {
    }
}

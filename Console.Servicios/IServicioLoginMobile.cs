using RECEPTIO.CapaAplicacion.Console.Aplicacion.Interfaces;
using System.ServiceModel;

namespace RECEPTIO.CapaServiciosDistribuidos.Console.Servicios
{
    [ServiceContract]
    public interface IServicioLoginMobile : ILogin
    {
    }
}

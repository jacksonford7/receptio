using RECEPTIO.CapaAplicacion.Console.Aplicacion.Interfaces;
using RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Interfaces;
using System.ServiceModel;

namespace RECEPTIO.CapaServiciosDistribuidos.Console.Servicios
{
    [ServiceContract]
    public interface IServicioConsole : ILogin, ITroubleDesk, ISupervisor, IError, ILoginBase
    {
    }
}

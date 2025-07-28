using RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Interfaces;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using System.ServiceModel;

namespace RECEPTIO.CapaAplicacion.Mobile.Aplicacion.Interfaces
{
    [ServiceContract]
    public interface IPrincipal : IBase
    {
        [OperationContract]
        ZONE ObtenerZonaConTiposTransacciones(string ip);

        [OperationContract]
        DEVICE ObtenerDevice(string ip);
    }
}

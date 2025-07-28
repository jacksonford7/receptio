using System;
using System.ServiceModel;

namespace RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Interfaces
{
    [ServiceContract]
    public interface ILoginBase : IBase
    {
        [OperationContract]
        Tuple<bool, string> AutenticarAccion(string usuario, string contrasena);
    }
}

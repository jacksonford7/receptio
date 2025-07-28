using RECEPTIO.CapaAplicacion.Console.Aplicacion.Contratos;
using RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Interfaces;
using System.ServiceModel;

namespace RECEPTIO.CapaAplicacion.Console.Aplicacion.Interfaces
{
    [ServiceContract]
    public interface ILogin : IBase
    {
        [OperationContract]
        DatosLogin Autenticar(string usuario, string contrasena, string ip);

        [OperationContract]
        void CerrarSesion(long idSesion);
    }
}

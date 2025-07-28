using RECEPTIO.CapaDominio.Nucleo.Entidades;
using System.ServiceModel;

namespace RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Interfaces
{
    [ServiceContract]
    public interface IError : IBase
    {
        [OperationContract]
        int CrearError(ERROR error);

        int GrabarErrorTecnico(ERROR error);
    }
}

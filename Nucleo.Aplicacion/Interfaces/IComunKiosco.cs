using RECEPTIO.CapaDominio.Nucleo.Entidades;
using System.Collections.Generic;
using System.ServiceModel;

namespace RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Interfaces
{
    [ServiceContract]
    public interface IComunKiosco : IBase
    {
        [OperationContract]
        APPLICATION ObtenerAplicacion(int idAplicacion);

        [OperationContract]
        IEnumerable<MESSAGE> ObtenerMensajesErrores();

        [OperationContract]
        KIOSK ObtenerQuiosco(string ip);

        [OperationContract]
        DEPOT ObtenerDepot(int id);
    }
}

using RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Contratos;
using RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Interfaces;
using RECEPTIO.CapaAplicacion.TransactionDepot.Aplicacion.Contratos;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using System.ServiceModel;

namespace RECEPTIO.CapaAplicacion.TransactionDepot.Aplicacion.Interfaces
{
    [ServiceContract]
    public interface IGeneralDepot : IBase
    {
        [OperationContract]
        RespuestaN4Depot AutentificacionWS(string usuario, long clave);

        [OperationContract]
        RespuestaN4Depot GeneraVisita(long idTransaccion,string token,string cedula, string placa, long turno);

        [OperationContract]
        RespuestaN4Depot Procesar(long idTransaccion,string token, long preGate, string placa, string contenedor, string booking);

        //[OperationContract]
        //RespuestaN4Depot GeneraEventoFacturaN4(long idTransaccion,string token, string contenedor);

        [OperationContract]
        RespuestaN4Depot GeneraSalida(long idTransaccion, long preGate, string token);
    }
}

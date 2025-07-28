using RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Contratos;
using RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Interfaces;
using RECEPTIO.CapaAplicacion.TransactionEmpty.Aplicacion.Contratos;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using System.ServiceModel;

namespace RECEPTIO.CapaAplicacion.TransactionEmpty.Aplicacion.Interfaces
{
    [ServiceContract]
    public interface IGeneral : IBase
    {
        [OperationContract]
        RespuestaN4 Procesar(long id, KIOSK kiosco);

        [OperationContract]
        Respuesta RegistrarProceso(KIOSK_TRANSACTION transaccion);
    }
}

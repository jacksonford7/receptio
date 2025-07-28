using RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Contratos;
using RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Interfaces;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using System.ServiceModel;

namespace RECEPTIO.CapaAplicacion.Transaction.Aplicacion.Interfaces
{
    [ServiceContract]
    public interface ITransaccionQuiosco : IBase
    {
        [OperationContract]
        Respuesta RegistrarProceso(KIOSK_TRANSACTION transaccion);
    }
}

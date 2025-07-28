using RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Interfaces;
using RECEPTIO.CapaAplicacion.TransactionEmpty.Aplicacion.Interfaces;
using System.ServiceModel;

namespace RECEPTIO.CapaServiciosDistribuidos.TransactionEmpty.Servicios
{
    [ServiceContract]
    public interface IServicioTransactionEmpty : IGeneral, IError, IComunKiosco
    {
    }
}

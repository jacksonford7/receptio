using RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Interfaces;
using RECEPTIO.CapaAplicacion.TransactionDepot.Aplicacion.Interfaces;
using System.ServiceModel;

namespace RECEPTIO.CapaServiciosDistribuidos.TransactionDepot.Servicios
{
    [ServiceContract]
    public interface IServicioTransactionDepot : IGeneralDepot//, IError, IComunKiosco
    {
    }
}


using System.ServiceModel;

namespace RECEPTIO.CapaAplicacion.Transaction.Aplicacion.Interfaces
{
    [ServiceContract]
    public interface IProveedor
    {
        [OperationContract]
        bool TienePaseVip(string numeroPase);
    }
}

using System.ServiceModel;

namespace RECEPTIO.CapaAplicacion.Transaction.Aplicacion.Interfaces
{
    [ServiceContract]
    public interface ITag
    {
        [OperationContract]
        string ObtenerTag(string placa);
    }
}

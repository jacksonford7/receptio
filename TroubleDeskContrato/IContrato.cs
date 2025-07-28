using System.ServiceModel;

namespace RECEPTIO.CapaPresentacion.TroubleDeskContrato
{
    [ServiceContract]
    public interface IContrato
    {
        [OperationContract]
        void AnunciarError(string mensajeError);
    }
}

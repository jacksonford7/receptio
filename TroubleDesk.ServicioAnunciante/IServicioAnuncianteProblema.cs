using System.ServiceModel;

namespace RECEPTIO.CapaServicioDistribuidos.TroubleDesk.ServicioAnunciante
{
    [ServiceContract]
    public interface IServicioAnuncianteProblema
    {
        [OperationContract]
        void AnunciarProblema(int idTransaccionQuiosco);

        [OperationContract]
        void AnunciarProblemaMobile(long idTosProcess, short idZona);

        [OperationContract]
        void AnunciarProblemaGenericoMobile(string mensajeError, short idZona);

        [OperationContract]
        void AnunciarProblemaClienteAppTransaction(int idError, short idZona);

        [OperationContract]
        void AnunciarProblemaServicioWebTransaction(string error, short idZona);
    }
}

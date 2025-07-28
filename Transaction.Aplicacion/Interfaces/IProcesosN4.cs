using RECEPTIO.CapaAplicacion.Transaction.Aplicacion.Contratos;
using System.ServiceModel;

namespace RECEPTIO.CapaAplicacion.Transaction.Aplicacion.Interfaces
{
    [ServiceContract]
    public interface IProcesosN4
    {
        [OperationContract]
        DatosN4 EjecutarProcesosEntrada(DatosEntradaN4 datos);

        [OperationContract]
        DatosN4 EjecutarProcesosSalida(DatosPreGateSalida datos);
    }
}

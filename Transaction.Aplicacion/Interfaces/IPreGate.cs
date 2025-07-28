using RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Interfaces;
using RECEPTIO.CapaAplicacion.Transaction.Aplicacion.Contratos;
using System.Collections.Generic;
using System.ServiceModel;

namespace RECEPTIO.CapaAplicacion.Transaction.Aplicacion.Interfaces
{
    [ServiceContract]
    public interface IPreGate : IBase
    {
        [OperationContract]
        DatosPreGate ValidarPreGate(string cedula, short idQuiosco, Dictionary<short, bool> valoresSensores);

        [OperationContract]
        DatosPreGateSalida ValidarEntradaQuiosco(long idPreGate, short idQuiosco, Dictionary<short, bool> valoresSensores);
    }
}

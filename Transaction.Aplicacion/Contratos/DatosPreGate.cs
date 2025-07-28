using RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Contratos;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using System.Runtime.Serialization;

namespace RECEPTIO.CapaAplicacion.Transaction.Aplicacion.Contratos
{
    [DataContract]
    public class DatosPreGate : Respuesta
    {
        [DataMember]
        public PRE_GATE PreGate { get; set; }

        [DataMember]
        public int IdTransaccion { get; set; }

        [DataMember]
        public int Peso { get; set; }
    }
}

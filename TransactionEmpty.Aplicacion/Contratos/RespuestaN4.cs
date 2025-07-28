using RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Contratos;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using System.Runtime.Serialization;

namespace RECEPTIO.CapaAplicacion.TransactionEmpty.Aplicacion.Contratos
{
    [DataContract]
    public class RespuestaN4 : Respuesta
    {
        [DataMember]
        public string Xml { get; set; }

        [DataMember]
        public int IdTransaccion { get; set; }

        [DataMember]
        public PRE_GATE PreGate { get; set; }

        [DataMember]
        public string XmlRecepcion { get; set; }

        [DataMember]
        public long IdPreGateRecepcion { get; set; }
    }
}

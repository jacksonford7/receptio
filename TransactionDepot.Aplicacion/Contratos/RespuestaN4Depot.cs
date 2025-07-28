using RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Contratos;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using System.Runtime.Serialization;

namespace RECEPTIO.CapaAplicacion.TransactionDepot.Aplicacion.Contratos
{
    [DataContract]
    public class RespuestaN4Depot : Respuesta
    {

        [DataMember]
        public long IdPreGateRecepcion { get; set; }

        [DataMember]
        public long IdTransaccion { get; set; }

        [DataMember]
        public string MensajeDetalle { get; set; }

        [DataMember]
        public string Token { get; set; }

    }
}

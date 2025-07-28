using RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Contratos;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using System.Runtime.Serialization;

namespace RECEPTIO.CapaAplicacion.Transaction.Aplicacion.Contratos
{
    [DataContract]
    public class DatosPreGateSalida : Respuesta
    {
        [DataMember]
        public PRE_GATE PreGate { get; set; }

        [DataMember]
        public string NombreQuiosco { get; set; }

        [DataMember]
        public string IdTransaccion { get; set; }

        [DataMember]
        public int Peso { get; set; }

        [DataMember]
        public int TipoTransaccion { get; set; }
    }
}

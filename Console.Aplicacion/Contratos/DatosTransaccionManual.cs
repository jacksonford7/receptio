using System.Runtime.Serialization;

namespace RECEPTIO.CapaAplicacion.Console.Aplicacion.Contratos
{
    [DataContract]
    public class DatosTransaccionManual
    {
        [DataMember]
        public long GKeyUnidad { get; set; }

        [DataMember]
        public string TipoCarga { get; set; }

        [DataMember]
        public string ObjetoSolicita { get; set; }

        [DataMember]
        public string UsuarioSolicita { get; set; }

        [DataMember]
        public string Contenedor { get; set; }

        [DataMember]
        public string Mrn { get; set; }

        [DataMember]
        public string Msn { get; set; }

        [DataMember]
        public string Hsn { get; set; }

        [DataMember]
        public string NumeroEntrega { get; set; }

        [DataMember]
        public string Comentarios { get; set; }
    }
}

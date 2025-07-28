using RECEPTIO.CapaDominio.Nucleo.Entidades;
using System.Runtime.Serialization;

namespace RECEPTIO.CapaAplicacion.Mobile.Aplicacion.Contratos
{
    [DataContract]
    public class DatosContenedor
    {
        [DataMember]
        public CONTAINER Contenedor { get; set; }

        [DataMember]
        public string Linea { get; set; }

        [DataMember]
        public string Aisv { get; set; }

        [DataMember]
        public string TipoCarga { get; set; }

        [DataMember]
        public string Iso { get; set; }

        [DataMember]
        public string Tamano { get; set; }
    }
}

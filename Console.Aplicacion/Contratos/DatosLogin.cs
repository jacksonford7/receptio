using System.Runtime.Serialization;

namespace RECEPTIO.CapaAplicacion.Console.Aplicacion.Contratos
{
    [DataContract]
    public class DatosLogin
    {
        [DataMember]
        public long IdSesion { get; set; }

        [DataMember]
        public bool EsLider { get; set; }

        [DataMember]
        public string Mensaje { get; set; }

        [DataMember]
        public bool EstaAutenticado { get; set; }

        [DataMember]
        public string Zona { get; set; }

        [DataMember]
        public int IdUsuario { get; set; }
    }
}

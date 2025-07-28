using System.Runtime.Serialization;

namespace RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Contratos
{
    [DataContract]
    public class Respuesta
    {
        [DataMember]
        public bool FueOk { get; set; }

        [DataMember]
        public string Mensaje { get; set; }
    }
}

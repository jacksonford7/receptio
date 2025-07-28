using RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Contratos;
using System.Runtime.Serialization;

namespace RECEPTIO.CapaAplicacion.Mobile.Aplicacion.Contratos
{
    [DataContract]
    public class RespuestaProceso : Respuesta
    {
        [DataMember]
        public long IdTosProcess { get; set; }
    }
}

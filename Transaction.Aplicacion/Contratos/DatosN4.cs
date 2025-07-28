using RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Contratos;
using System.Runtime.Serialization;

namespace RECEPTIO.CapaAplicacion.Transaction.Aplicacion.Contratos
{
    [DataContract]
    public class DatosN4 : Respuesta
    {
        [DataMember]
        public string Xml { get; set; }
    }
}

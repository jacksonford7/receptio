using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RECEPTIO.CapaAplicacion.Mobile.Aplicacion.Contratos
{
    [DataContract]
    public class DatosReceiveExport : DatosN4
    {
        [DataMember]
        public IEnumerable<DatosContenedor> DataContenedores { get; set; }
    }
}

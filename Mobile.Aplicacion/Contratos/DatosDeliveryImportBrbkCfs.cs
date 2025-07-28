using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RECEPTIO.CapaAplicacion.Mobile.Aplicacion.Contratos
{
    [DataContract]
    public class DatosDeliveryImportBrbkCfs : DatosN4
    {
        [DataMember]
        public string Bl { get; set; }

        [DataMember]
        public string Qty { get; set; }
    }

    [DataContract]
    public class DatosDeliveryImportP2D : DatosN4
    {
        [DataMember]
        public IEnumerable<DatosBLP2D> DatosTransaccionP2D { get; set; }
    }
    public class DatosBLP2D
    {
        [DataMember]
        public string NumeroTransaccion { get; set; }
        
        [DataMember]
        public string Bl { get; set; }

        [DataMember]
        public string Qty { get; set; }
    }
}

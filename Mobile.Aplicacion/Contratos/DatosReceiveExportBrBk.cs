using System.Runtime.Serialization;

namespace RECEPTIO.CapaAplicacion.Mobile.Aplicacion.Contratos
{
    [DataContract]
    public class DatosReceiveExportBrBk : DatosN4
    {
        [DataMember]
        public string Bl { get; set; }

        [DataMember]
        public string Ip { get; set; }

        [DataMember]
        public int Cantidad { get; set; }

        [DataMember]
        public string VeeselVisit { get; set; }

        [DataMember]
        public string Notas { get; set; }

        [DataMember]
        public string Dae { get; set; }
    }

    [DataContract]
    public class DatosReceiveExportBanano : DatosReceiveExportBrBk
    {
    }
}

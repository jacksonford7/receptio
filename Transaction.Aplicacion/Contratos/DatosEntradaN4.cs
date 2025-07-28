using System.Runtime.Serialization;

namespace RECEPTIO.CapaAplicacion.Transaction.Aplicacion.Contratos
{
    [DataContract]
    public class DatosEntradaN4
    {
        [DataMember]
        public string PlacaVehiculo { get; set; }

        [DataMember]
        public string CedulaChofer { get; set; }

        [DataMember]
        public string NombreQuiosco { get; set; }

        [DataMember]
        public string IdTransaccion { get; set; }

        [DataMember]
        public long IdPreGate { get; set; }

        [DataMember]
        public int Peso { get; set; }

        [DataMember]
        public int TipoTransaccion { get; set; }
    }
}

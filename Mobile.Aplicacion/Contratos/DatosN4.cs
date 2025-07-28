using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RECEPTIO.CapaAplicacion.Mobile.Aplicacion.Contratos
{
    [DataContract]
    public class DatosN4
    {
        [DataMember]
        public string PlacaVehiculo { get; set; }

        [DataMember]
        public string CedulaChofer { get; set; }

        [DataMember]
        public string IdCompania { get; set; }

        [DataMember]
        public string IdPreGate { get; set; }

        [DataMember]
        public IEnumerable<string> NumerosTransacciones { get; set; }

        public int GosTvKey
        {
            get
            {
                return Convert.ToInt32(IdPreGate) * -1;
            }
        }
    }
}

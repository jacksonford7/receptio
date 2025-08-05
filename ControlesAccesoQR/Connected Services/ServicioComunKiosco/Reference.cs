using System;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace ControlesAccesoQR.ServicioComunKiosco
{
    [ServiceContract]
    public interface IServicioComunKiosco
    {
        [OperationContract]
        KIOSK ObtenerQuiosco(string ip);
    }

    [DataContract]
    public class KIOSK
    {
        [DataMember]
        public short KIOSK_ID { get; set; }

        [DataMember]
        public string NAME { get; set; }

        [DataMember]
        public string IP { get; set; }

        [DataMember]
        public bool IS_ACTIVE { get; set; }

        [DataMember]
        public short ZONE_ID { get; set; }

        [DataMember]
        public bool IS_IN { get; set; }
    }

    public partial class ServicioComunKioscoClient : ClientBase<IServicioComunKiosco>, IServicioComunKiosco
    {
        public ServicioComunKioscoClient()
        {
        }

        public ServicioComunKioscoClient(string endpointConfigurationName) : base(endpointConfigurationName)
        {
        }

        public KIOSK ObtenerQuiosco(string ip)
        {
            return Channel.ObtenerQuiosco(ip);
        }
    }
}

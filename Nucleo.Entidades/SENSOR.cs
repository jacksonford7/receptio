//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RECEPTIO.CapaDominio.Nucleo.Entidades
{
    using System;
    using System.Runtime.Serialization;
    using System.Collections.Generic;
    
    [DataContract(IsReference=true)]
    public partial class SENSOR
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SENSOR()
        {
            this.SENSOR_KIOSK_TRANSACTIONS = new HashSet<SENSOR_KIOSK_TRANSACTION>();
        }
    
        [DataMember]
        public short SENSOR_ID { get; set; }
        [DataMember]
        public string NAME { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    	[DataMember]
        public virtual ICollection<SENSOR_KIOSK_TRANSACTION> SENSOR_KIOSK_TRANSACTIONS { get; set; }
    }
}

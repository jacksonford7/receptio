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
    public partial class PRE_GATE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PRE_GATE()
        {
            this.PRE_GATE_DETAILS = new HashSet<PRE_GATE_DETAIL>();
            this.KIOSK_TRANSACTIONS = new HashSet<KIOSK_TRANSACTION>();
            this.TOS_PROCCESSES = new HashSet<TOS_PROCCESS>();
            this.TROUBLE_TICKETS_MOBILE_TROUBLE_TICKET = new HashSet<MOBILE_TROUBLE_TICKET>();
            this.REPRINTS = new HashSet<REPRINT>();
            this.STOCK_REGISTER = new HashSet<STOCK_REGISTER>();
        }
    
        [DataMember]
        public long PRE_GATE_ID { get; set; }
        [DataMember]
        public System.DateTime CREATION_DATE { get; set; }
        [DataMember]
        public string USER { get; set; }
        [DataMember]
        public string DRIVER_ID { get; set; }
        [DataMember]
        public string TRUCK_LICENCE { get; set; }
        [DataMember]
        public bool IS_RECYCLED { get; set; }
        [DataMember]
        public int DEVICE_ID { get; set; }
        [DataMember]
        public string STATUS { get; set; }
        [DataMember]
        public byte[] CONCURRENCY { get; set; }
        [DataMember]
        public Nullable<long> PRE_GATE_ID_REF { get; set; }
    
    	[DataMember]
        public virtual DEVICE DEVICE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    	[DataMember]
        public virtual ICollection<PRE_GATE_DETAIL> PRE_GATE_DETAILS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    	[DataMember]
        public virtual ICollection<KIOSK_TRANSACTION> KIOSK_TRANSACTIONS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    	[DataMember]
        public virtual ICollection<TOS_PROCCESS> TOS_PROCCESSES { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    	[DataMember]
        public virtual ICollection<MOBILE_TROUBLE_TICKET> TROUBLE_TICKETS_MOBILE_TROUBLE_TICKET { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    	[DataMember]
        public virtual ICollection<REPRINT> REPRINTS { get; set; }
    	[DataMember]
        public virtual BY_PASS BY_PASS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    	[DataMember]
        public virtual ICollection<STOCK_REGISTER> STOCK_REGISTER { get; set; }
    }
}

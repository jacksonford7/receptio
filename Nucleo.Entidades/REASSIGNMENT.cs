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
    public partial class REASSIGNMENT
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public System.DateTime DATE { get; set; }
        [DataMember]
        public long USER_SESSION_ID { get; set; }
        [DataMember]
        public bool IS_RECYCLED { get; set; }
        [DataMember]
        public string USER { get; set; }
        [DataMember]
        public int MOTIVE_ID { get; set; }
        [DataMember]
        public long TT_ID { get; set; }
    
    	[DataMember]
        public virtual USER_SESSION USER_SESSION { get; set; }
    	[DataMember]
        public virtual REASSIGNMENT_MOTIVE REASSIGNMENT_MOTIVE { get; set; }
    	[DataMember]
        public virtual PROCESS_TROUBLE_TICKET PROCESS_TROUBLE_TICKET { get; set; }
    }
}

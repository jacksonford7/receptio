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
    public partial class LINE_DEPOTS
    {
        [DataMember]
        public long ID_LINE_DEPO { get; set; }
        [DataMember]
        public int ID_LINE { get; set; }
        [DataMember]
        public int ID_DEPOT { get; set; }
        [DataMember]
        public bool ACTIVE { get; set; }
        [DataMember]
        public string CREATE_USER { get; set; }
        [DataMember]
        public Nullable<System.DateTime> CREATE_DATE { get; set; }
        [DataMember]
        public Nullable<System.DateTime> MOD_DATE { get; set; }
        [DataMember]
        public string MOD_USER { get; set; }
    
    	[DataMember]
        public virtual DEPOT DEPOT { get; set; }
    	[DataMember]
        public virtual LINE LINE { get; set; }
    }
}

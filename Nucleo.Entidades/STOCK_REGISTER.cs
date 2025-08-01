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
    public partial class STOCK_REGISTER
    {
        [DataMember]
        public int ID_LINE { get; set; }
        [DataMember]
        public int ID_DEPOT { get; set; }
        [DataMember]
        public int ID_OPERATION { get; set; }
        [DataMember]
        public Nullable<long> PRE_GATE_ID { get; set; }
        [DataMember]
        public Nullable<long> PRE_GATE_DETAILS_ID { get; set; }
        [DataMember]
        public string OPERATION_USER { get; set; }
        [DataMember]
        public string OPERATION_OBJETC { get; set; }
        [DataMember]
        public string OPERATION_NOTES { get; set; }
        [DataMember]
        public int QTY { get; set; }
        [DataMember]
        public int MULTIPLIER { get; set; }
        [DataMember]
        public Nullable<bool> ACTIVE { get; set; }
        [DataMember]
        public Nullable<System.DateTime> CREATE_DATE { get; set; }
        [DataMember]
        public Nullable<int> CREATE_YEAR { get; set; }
        [DataMember]
        public Nullable<int> CREATE_MONTH { get; set; }
        [DataMember]
        public Nullable<int> CREATE_WEEK { get; set; }
        [DataMember]
        public long ID { get; set; }
        [DataMember]
        public Nullable<System.DateTime> REGISTER_DATE { get; set; }
    
    	[DataMember]
        public virtual DEPOT DEPOT { get; set; }
    	[DataMember]
        public virtual LINE LINE { get; set; }
    	[DataMember]
        public virtual STOCK_OPERATION STOCK_OPERATION { get; set; }
    	[DataMember]
        public virtual PRE_GATE PRE_GATES { get; set; }
    }
}

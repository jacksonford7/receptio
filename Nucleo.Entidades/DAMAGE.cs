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
    public partial class DAMAGE
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string DAMAGE_TYPE { get; set; }
        [DataMember]
        public string COMPONENT { get; set; }
        [DataMember]
        public string SEVERITY { get; set; }
        [DataMember]
        public string LOCATION { get; set; }
        [DataMember]
        public short QUANTITY { get; set; }
        [DataMember]
        public string NOTES { get; set; }
        [DataMember]
        public int CONTAINER_ID { get; set; }
    
    	[DataMember]
        public virtual CONTAINER CONTAINER { get; set; }
    }
}

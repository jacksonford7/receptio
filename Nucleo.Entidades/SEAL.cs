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
    public partial class SEAL
    {
        [DataMember]
        public int SEAL_ID { get; set; }
        [DataMember]
        public string CAPTION { get; set; }
        [DataMember]
        public string VALUE { get; set; }
        [DataMember]
        public int CONTAINER_ID { get; set; }
        [DataMember]
        public bool IS_RECYCLED { get; set; }
    
    	[DataMember]
        public virtual CONTAINER CONTAINER { get; set; }
    }
}

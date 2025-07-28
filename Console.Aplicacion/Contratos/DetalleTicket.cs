using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RECEPTIO.CapaAplicacion.Console.Aplicacion.Contratos
{
    [DataContract]
    public class DetalleTicket
    {
        [DataMember]
        public IEnumerable<Proceso> Procesos { get; set; }

        [DataMember]
        public string CedulaChofer { get; set; }

        [DataMember]
        public string PlacaCamion { get; set; }

        [DataMember]
        public string TipoTransaccion { get; set; }

        [DataMember]
        public string Contenedores { get; set; }

        [DataMember]
        public string Containers { get; set; }

        [DataMember]
        public long PregateID { get; set; }

        [DataMember]
        public IEnumerable<TransaccionContenedor> TransaccionContenedores { get; set; }
    }

    [DataContract]
    public class Proceso
    {
        [DataMember]
        public string Paso { get; set; }

        [DataMember]
        public bool FueOk { get; set; }

        [DataMember]
        public string MensajeUsuario { get; set; }

        [DataMember]
        public string MensajeTecnico { get; set; }

        [DataMember]
        public string MensajeEspecifico { get; set; }

        [DataMember]
        public string Respuesta { get; set; }

        [DataMember]
        public DateTime FechaProceso { get; set; }
    }

    [DataContract]
    public class TransaccionContenedor
    {
        [DataMember]
        public string NumeroTransaccion { get; set; }

        [DataMember]
        public string Contenedor { get; set; }
    }
}

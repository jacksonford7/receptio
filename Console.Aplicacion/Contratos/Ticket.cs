using System;
using System.Runtime.Serialization;

namespace RECEPTIO.CapaAplicacion.Console.Aplicacion.Contratos
{
    [DataContract]
    public class Ticket
    {
        [DataMember]
        public long IdTicket { get; set; }

        [DataMember]
        public int IdTransaccionQuiosco { get; set; }

        [DataMember]
        public bool EsEntrada { get; set; }

        [DataMember]
        public string NombreQuiosco { get; set; }

        [DataMember]
        public short IdQuiosco { get; set; }

        [DataMember]
        public string IpQuiosco { get; set; }

        [DataMember]
        public string NombreProceso { get; set; }

        [DataMember]
        public string Mensaje { get; set; }

        [DataMember]
        public string Notas { get; set; }

        [DataMember]
        public TipoTicket Tipo { get; set; }

        [DataMember]
        public string TipoTransaccion { get; set; }

        [DataMember]
        public DateTime FechaCreacion { get; set; }

        [DataMember]
        public DateTime FechaAsignacion { get; set; }

        [DataMember]
        public DateTime? FechaAceptacion { get; set; }

        [DataMember]
        public DateTime? FechaFinalizacion { get; set; }

        [DataMember]
        public string Zona { get; set; }

        [DataMember]
        public short IdZona { get; set; }

        [DataMember]
        public bool EstaSuspendido { get; set; }

        [DataMember]
        public long? IdSesionUsuario { get; set; }

        [DataMember]
        public string Responsable { get; set; }

        [DataMember]
        public Nullable<int> IdMotive { get; set; }

        [DataMember]
        public Nullable<int> IdSubMotive { get; set; }
    }

    public enum TipoTicket
    {
        Proceso,
        Auto,
        Mobile,
        Tecnico
    }

    [DataContract]
    public class TicketReporte : Ticket
    {
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string CedulaChofer { get; set; }

        [DataMember]
        public string PlacaVehiculo { get; set; }

        [DataMember]
        public string Contenedores { get; set; }

        [DataMember]
        public string MotivoAutoTicket { get; set; }

        [DataMember]
        public long IdPreGate { get; set; }

        [DataMember]
        public bool EstaCancelado { get; set; }

        [DataMember]
        public string UsuarioCancelacion { get; set; }

        [DataMember]
        public DateTime? FechaCancelacion { get; set; }
    }
}

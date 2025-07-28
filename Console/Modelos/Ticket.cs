namespace Console.ServicioConsole
{
    public partial class Ticket
    {
        public string ImagenEstado
        {
            get
            {
                if (!FechaAceptacion.HasValue)
                    return "/Imagenes/New.png";
                else if (!FechaFinalizacion.HasValue)
                    return "/Imagenes/Process.png";
                else
                    return "/Imagenes/Finish.png";
            }
        }

        public string NombreEntradaSalida
        {
            get
            {
                return EsEntrada ? "Entrada" : Tipo == TipoTicket.Proceso ? "Salida" : "";
            }
        }

        public string Estado
        {
            get
            {
                if (!FechaAceptacion.HasValue)
                    return "Nuevo";
                else if (FechaFinalizacion.HasValue)
                    return "Finalizado";
                else
                    return "En Proceso";
            }
        }

        public string NombreTipo { get { return Tipo.ToString(); } }

        public bool EstaChequeado { get; set; }
    }
}

using System;

namespace ControlesAccesoQR.Models
{
    public class PaseProcesoModel
    {
        public string NombreChofer { get; set; }
        public string Placa { get; set; }
        public DateTime FechaHoraLlegada { get; set; }
        public DateTime FechaHoraSalida { get; set; }
        public string NumeroPase { get; set; }
        public EstadoProceso Estado { get; set; }
    }
}

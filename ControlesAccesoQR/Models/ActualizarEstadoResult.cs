using System;

namespace ControlesAccesoQR.Models
{
    public class ActualizarEstadoResult
    {
        public int PasePuertaID { get; set; }
        public string NumeroPase { get; set; }
        public string Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
    }
}

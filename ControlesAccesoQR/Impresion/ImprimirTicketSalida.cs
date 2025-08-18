using System;
using System.Drawing;
using System.Drawing.Printing;

namespace ControlesAccesoQR.Impresion
{
    internal class DatosTicketSalida
    {
        public DateTime FechaSalida { get; set; }
        public string Transportista { get; set; }
        public string Compania { get; set; }
    }

    internal class ImprimirTicketSalida : ImprimirTicket
    {
        public ImprimirTicketSalida(DatosTicketSalida datos) : base(string.Empty, datos)
        {
        }

        protected override void EventoImprimir(object sender, PrintPageEventArgs ev)
        {
            var datos = (DatosTicketSalida)DatosExtras;
            ev.Graphics.DrawString("SALIDA", Negrita12, Brushes.Black, 20, 20);
            ev.Graphics.DrawString($"Fecha Salida: {datos.FechaSalida:yyyy-MM-dd HH:mm}", Negrita8, Brushes.Black, 20, 60);
            ev.Graphics.DrawString($"Transportista: {datos.Transportista}", Negrita8, Brushes.Black, 20, 80);
            ev.Graphics.DrawString($"Compañía: {datos.Compania}", Negrita8, Brushes.Black, 20, 100);
            ev.HasMorePages = false;
        }
    }
}

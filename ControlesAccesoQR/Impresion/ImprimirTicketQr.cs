using System;
using System.Drawing;
using System.Drawing.Printing;
using QRCoder;

namespace ControlesAccesoQR.Impresion
{
    internal abstract class ImprimirTicket : IDisposable
    {
        private bool _disposed;
        protected readonly string Codigo;
        protected readonly object DatosExtras;
        protected readonly Font Negrita8;
        protected readonly Font Negrita12;
        protected readonly Font Normal8;
        protected PrintDocument PrintDocument;

        protected ImprimirTicket(string codigo, object datosExtras)
        {
            Codigo = codigo;
            DatosExtras = datosExtras;
            Negrita8 = new Font("Arial", 8, FontStyle.Bold);
            Negrita12 = new Font("Arial", 12, FontStyle.Bold);
            Normal8 = new Font("Arial", 8);
            PrintDocument = new PrintDocument();
        }

        internal void Imprimir()
        {
            PrintDocument.PrintPage += EventoImprimir;
            PrintDocument.Print();
        }

        protected abstract void EventoImprimir(object sender, PrintPageEventArgs ev);

        public void Dispose()
        {
            if (_disposed)
                return;
            Negrita8?.Dispose();
            Negrita12?.Dispose();
            Normal8?.Dispose();
            PrintDocument?.Dispose();
            _disposed = true;
        }
    }

    internal class DatosTicketQr
    {
        public string Cliente { get; set; }
        public string Chofer { get; set; }
    }

    internal class ImprimirTicketSalidaQr : ImprimirTicket
    {
        public ImprimirTicketSalidaQr(string codigoQr, DatosTicketQr datos) : base(codigoQr, datos)
        {
        }

        protected override void EventoImprimir(object sender, PrintPageEventArgs ev)
        {
            var datos = (DatosTicketQr)DatosExtras;
            ev.Graphics.DrawString("TICKET", Negrita12, Brushes.Black, 20, 20);

            using (var generator = new QRCodeGenerator())
            using (var data = generator.CreateQrCode(Codigo, QRCodeGenerator.ECCLevel.Q))
            using (var qrCode = new QRCode(data))
            using (var qrImage = qrCode.GetGraphic(5))
            {
                ev.Graphics.DrawImage(qrImage, 20, 50, 100, 100);
            }

            ev.Graphics.DrawString($"CodigoQR: {Codigo}", Negrita8, Brushes.Black, 130, 60);
            ev.Graphics.DrawString($"Cliente: {datos.Cliente}", Negrita8, Brushes.Black, 130, 80);
            ev.Graphics.DrawString($"Chofer: {datos.Chofer}", Negrita8, Brushes.Black, 130, 100);

            ev.HasMorePages = false;
        }
    }
}

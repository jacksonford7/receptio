using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;

namespace RECEPTIO.CapaPresentacion.UI.MVVM
{
    public class Pdf : IDisposable
    {
        private readonly PdfDocument _documento;
        private readonly PdfPage _pagina;
        private readonly XGraphics _grafico;
        private XFont _font;
        private bool _disposed;

        public Pdf()
        {
            _documento = new PdfDocument();
            _pagina = _documento.AddPage();
            _grafico = XGraphics.FromPdfPage(_pagina);
            _font = new XFont("Verdana", 20, XFontStyle.Bold);
        }

        public void EscribirTexto(string texto, int x, int y)
        {
            _grafico.DrawString(texto, _font, XBrushes.Black, new XRect(x, y, _pagina.Width, 0));
        }

        public void GuardarArchivo(string ruta)
        {
            _documento.Save(ruta);
        }

        public void EstablecerFont(string nombreFont, int tamano, bool esNegrita)
        {
            _font = new XFont(nombreFont, tamano, esNegrita ? XFontStyle.Bold : XFontStyle.Regular);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
                _documento.Dispose();
            _disposed = true;
        }
    }
}

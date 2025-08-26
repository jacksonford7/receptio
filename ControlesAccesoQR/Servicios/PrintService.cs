using System.Diagnostics;

namespace ControlesAccesoQR.Servicios
{
    public class PrintService : IPrintService
    {
        public void Print(string contenido)
        {
            Debug.WriteLine(contenido);
        }
    }
}

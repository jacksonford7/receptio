using System;
using System.Configuration;
using System.IO;
using System.ServiceModel;

namespace RECEPTIO.CapaServiciosDistribuidos.Nucleo.Servicios
{
    public class Base
    {
        protected void LoguearError(string mensaje)
        {
            try
            {
                var ruta = Path.Combine(ConfigurationManager.AppSettings["RutaLog"], ConfigurationManager.AppSettings["NombreArchivoLog"]);
                var sw = new StreamWriter(ruta, true);
                sw.WriteLine($"Fecha: {DateTime.Now}. {mensaje}");
                sw.Close();
            }
            catch (Exception)
            {
                throw new FaultException($"Ha ocurrido un inconveniente y no se pudo loguearlo.Contactarse con el Departamento de IT.{Environment.NewLine}{mensaje}");
            }
            throw new FaultException("Ha ocurrido un inconveniente.Contactarse con el Departamento de IT.");
        }
    }
}

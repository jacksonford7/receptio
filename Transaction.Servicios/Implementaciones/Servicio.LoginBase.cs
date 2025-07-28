using RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Interfaces;
using Spring.Context.Support;
using System;
using System.ServiceModel;

namespace RECEPTIO.CapaServiciosDistribuidos.Transaction.Servicios
{
    public partial class ServicioTransaction
    {
        public Tuple<bool, string> AutenticarAccion(string usuario, string contrasena)
        {
            ILoginBase administradorLoginBase = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringLoginBase.xml");
                administradorLoginBase = (ILoginBase)ctx["AdministradorLoginBase"];
                var resultado = administradorLoginBase.AutenticarAccion(usuario, contrasena);
                return resultado;
            }
            catch (FaultException)
            {
                throw;
            }
            catch (Exception ex)
            {
                LoguearError($"Mensaje : {ex.Message}///Excepción Interna : {ex.InnerException}///Pila de Seguimiento : {ex.StackTrace}///Fuente : {ex.Source}///Link : {ex.HelpLink}");
                throw;
            }
            finally
            {
                if (administradorLoginBase != null)
                    administradorLoginBase.LiberarRecursos();
            }
        }
    }
}
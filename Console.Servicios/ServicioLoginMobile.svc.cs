using System;
using System.ServiceModel;
using RECEPTIO.CapaAplicacion.Console.Aplicacion.Contratos;
using RECEPTIO.CapaAplicacion.Console.Aplicacion.Interfaces;
using RECEPTIO.CapaServiciosDistribuidos.Nucleo.Servicios;
using Spring.Context.Support;

namespace RECEPTIO.CapaServiciosDistribuidos.Console.Servicios
{
    public class ServicioLoginMobile : Base, IServicioLoginMobile
    {
        public DatosLogin Autenticar(string usuario, string contrasena, string ip)
        {
            ILogin administradorLogin = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Springs/SpringLoginMobile.xml");
                administradorLogin = (ILogin)ctx["AdministradorLoginMobile"];
                var login = administradorLogin.Autenticar(usuario, contrasena, ip);
                return login;
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
                if (administradorLogin != null)
                    administradorLogin.LiberarRecursos();
            }
        }

        public void CerrarSesion(long idSesion)
        {
            ILogin administradorLogin = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Springs/SpringLoginMobile.xml");
                administradorLogin = (ILogin)ctx["AdministradorLoginMobile"];
                administradorLogin.CerrarSesion(idSesion);
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
                if (administradorLogin != null)
                    administradorLogin.LiberarRecursos();
            }
        }

        public void LiberarRecursos()
        {
        }
    }
}

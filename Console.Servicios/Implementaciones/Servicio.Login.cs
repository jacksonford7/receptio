using RECEPTIO.CapaAplicacion.Console.Aplicacion.Contratos;
using RECEPTIO.CapaAplicacion.Console.Aplicacion.Interfaces;
using Spring.Context.Support;
using System;
using System.ServiceModel;

namespace RECEPTIO.CapaServiciosDistribuidos.Console.Servicios
{
    public partial class ServicioConsole
    {
        public DatosLogin Autenticar(string usuario, string contrasena, string ip)
        {
            ILogin administradorLogin = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringLogin.xml");
                administradorLogin = (ILogin)ctx["AdministradorLogin"];
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
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringLogin.xml");
                administradorLogin = (ILogin)ctx["AdministradorLogin"];
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
    }
}
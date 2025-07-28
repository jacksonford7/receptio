using RECEPTIO.CapaAplicacion.Mobile.Aplicacion.Interfaces;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using Spring.Context.Support;
using System;
using System.ServiceModel;

namespace RECEPTIO.CapaServiciosDistribuidos.Mobile.Servicios
{
    public partial class ServicioMobile
    {
        public ZONE ObtenerZonaConTiposTransacciones(string ip)
        {
            IPrincipal administradorPrincipal = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringPrincipal.xml");
                administradorPrincipal = (IPrincipal)ctx["AdministradorPrincipal"];
                return administradorPrincipal.ObtenerZonaConTiposTransacciones(ip);
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
                if (administradorPrincipal != null)
                    administradorPrincipal.LiberarRecursos();
            }
        }

        public DEVICE ObtenerDevice(string ip)
        {
            IPrincipal administradorPrincipal = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringPrincipal.xml");
                administradorPrincipal = (IPrincipal)ctx["AdministradorPrincipal"];
                return administradorPrincipal.ObtenerDevice(ip);
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
                if (administradorPrincipal != null)
                    administradorPrincipal.LiberarRecursos();
            }
        }
    }
}
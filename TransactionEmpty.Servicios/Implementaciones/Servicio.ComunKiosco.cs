using RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Interfaces;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using Spring.Context.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace RECEPTIO.CapaServiciosDistribuidos.TransactionEmpty.Servicios
{
    public partial class ServicioTransactionEmpty
    {
        public APPLICATION ObtenerAplicacion(int idAplicacion)
        {
            IComunKiosco administradorComunKiosco = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringComunKiosco.xml");
                administradorComunKiosco = (IComunKiosco)ctx["AdministradorComunKiosco"];
                var aplicacion = administradorComunKiosco.ObtenerAplicacion(idAplicacion);
                return aplicacion;
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
                if (administradorComunKiosco != null)
                    administradorComunKiosco.LiberarRecursos();
            }
        }

        public IEnumerable<MESSAGE> ObtenerMensajesErrores()
        {
            IComunKiosco administradorComunKiosco = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringComunKiosco.xml");
                administradorComunKiosco = (IComunKiosco)ctx["AdministradorComunKiosco"];
                var errores = administradorComunKiosco.ObtenerMensajesErrores();
                return errores.ToList();
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
                if (administradorComunKiosco != null)
                    administradorComunKiosco.LiberarRecursos();
            }
        }

        public KIOSK ObtenerQuiosco(string ip)
        {
            IComunKiosco administradorComunKiosco = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringComunKiosco.xml");
                administradorComunKiosco = (IComunKiosco)ctx["AdministradorComunKiosco"];
                var quiosco = administradorComunKiosco.ObtenerQuiosco(ip);
                return quiosco;
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
                if (administradorComunKiosco != null)
                    administradorComunKiosco.LiberarRecursos();
            }
        }

        public DEPOT ObtenerDepot(int id)
        {
            IComunKiosco administradorComunKiosco = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringComunKiosco.xml");
                administradorComunKiosco = (IComunKiosco)ctx["AdministradorComunKiosco"];
                var depot = administradorComunKiosco.ObtenerDepot(id);
                return depot;
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
                if (administradorComunKiosco != null)
                    administradorComunKiosco.LiberarRecursos();
            }
        }
    }
}
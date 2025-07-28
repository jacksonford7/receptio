using RECEPTIO.CapaAplicacion.Console.Aplicacion.Contratos;
using RECEPTIO.CapaAplicacion.Console.Aplicacion.Interfaces;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using Spring.Context.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace RECEPTIO.CapaServiciosDistribuidos.Console.Servicios
{
    public partial class ServicioConsole
    {
        public IEnumerable<Ticket> ObtenerTickets(long idSesionUsuario)
        {
            ITroubleDesk administradorTroubleDesk = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringTroubleDesk.xml");
                administradorTroubleDesk = (ITroubleDesk)ctx["AdministradorTroubleDesk"];
                var tickets = administradorTroubleDesk.ObtenerTickets(idSesionUsuario);
                return tickets;
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
                if (administradorTroubleDesk != null)
                    administradorTroubleDesk.LiberarRecursos();
            }
        }

        public DetalleTicket ObtenerDetallesTicket(int idTransaccionQuiosco)
        {
            ITroubleDesk administradorTroubleDesk = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringTroubleDesk.xml");
                administradorTroubleDesk = (ITroubleDesk)ctx["AdministradorTroubleDesk"];
                var detalles = administradorTroubleDesk.ObtenerDetallesTicket(idTransaccionQuiosco);
                return detalles;
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
                if (administradorTroubleDesk != null)
                    administradorTroubleDesk.LiberarRecursos();
            }
        }

        public void AceptarTicket(long idTicket)
        {
            ITroubleDesk administradorTroubleDesk = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringTroubleDesk.xml");
                administradorTroubleDesk = (ITroubleDesk)ctx["AdministradorTroubleDesk"];
                administradorTroubleDesk.AceptarTicket(idTicket);
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
                if (administradorTroubleDesk != null)
                    administradorTroubleDesk.LiberarRecursos();
            }
        }

        public void CerrarTicket(long idTicket, string notas, int motivo , int submotivo)
        {
            ITroubleDesk administradorTroubleDesk = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringTroubleDesk.xml");
                administradorTroubleDesk = (ITroubleDesk)ctx["AdministradorTroubleDesk"];
                administradorTroubleDesk.CerrarTicket(idTicket, notas, motivo, submotivo);
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
                if (administradorTroubleDesk != null)
                    administradorTroubleDesk.LiberarRecursos();
            }
        }

        public IEnumerable<AUTO_TROUBLE_REASON> ObtenerMotivosAutoTickets()
        {
            ITroubleDesk administradorTroubleDesk = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringTroubleDesk.xml");
                administradorTroubleDesk = (ITroubleDesk)ctx["AdministradorTroubleDesk"];
                var motivos = administradorTroubleDesk.ObtenerMotivosAutoTickets();
                return motivos;
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
                if (administradorTroubleDesk != null)
                    administradorTroubleDesk.LiberarRecursos();
            }
        }

        public void CrearAutoTicket(int idMotivo, long idSesionUsuario)
        {
            ITroubleDesk administradorTroubleDesk = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringTroubleDesk.xml");
                administradorTroubleDesk = (ITroubleDesk)ctx["AdministradorTroubleDesk"];
                administradorTroubleDesk.CrearAutoTicket(idMotivo, idSesionUsuario);
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
                if (administradorTroubleDesk != null)
                    administradorTroubleDesk.LiberarRecursos();
            }
        }

        public void RegistrarAccion(ACTION accion)
        {
            ITroubleDesk administradorTroubleDesk = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringTroubleDesk.xml");
                administradorTroubleDesk = (ITroubleDesk)ctx["AdministradorTroubleDesk"];
                administradorTroubleDesk.RegistrarAccion(accion);
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
                if (administradorTroubleDesk != null)
                    administradorTroubleDesk.LiberarRecursos();
            }
        }

        public IEnumerable<mb_get_ecuapass_message_pass_Result> ObtenerMensajesSmdtAduana(string numeroTransaccion)
        {
            ITroubleDesk administradorTroubleDesk = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringTroubleDesk.xml");
                administradorTroubleDesk = (ITroubleDesk)ctx["AdministradorTroubleDesk"];
                var mensajes = administradorTroubleDesk.ObtenerMensajesSmdtAduana(numeroTransaccion).ToList();
                return mensajes;
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
                if (administradorTroubleDesk != null)
                    administradorTroubleDesk.LiberarRecursos();
            }
        }

        public mb_add_ecuapass_transaccion_Result AgregarTransaccionManual(DatosTransaccionManual datosTransaccionManual)
        {
            ITroubleDesk administradorTroubleDesk = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringTroubleDesk.xml");
                administradorTroubleDesk = (ITroubleDesk)ctx["AdministradorTroubleDesk"];
                var resultado  = administradorTroubleDesk.AgregarTransaccionManual(datosTransaccionManual);
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
                if (administradorTroubleDesk != null)
                    administradorTroubleDesk.LiberarRecursos();
            }
        }

        public byte? CambiarEstadoSmdt(string numeroTransaccion, string userName)
        {
            ITroubleDesk administradorTroubleDesk = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringTroubleDesk.xml");
                administradorTroubleDesk = (ITroubleDesk)ctx["AdministradorTroubleDesk"];
                var resultado = administradorTroubleDesk.CambiarEstadoSmdt(numeroTransaccion, userName);
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
                if (administradorTroubleDesk != null)
                    administradorTroubleDesk.LiberarRecursos();
            }
        }

        public void SuspenderTicket(long idTicket)
        {
            ITroubleDesk administradorTroubleDesk = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringTroubleDesk.xml");
                administradorTroubleDesk = (ITroubleDesk)ctx["AdministradorTroubleDesk"];
                administradorTroubleDesk.SuspenderTicket(idTicket);
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
                if (administradorTroubleDesk != null)
                    administradorTroubleDesk.LiberarRecursos();
            }
        }

        public IEnumerable<BREAK_TYPE> ObtenerTiposDescansos()
        {
            ITroubleDesk administradorTroubleDesk = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringTroubleDesk.xml");
                administradorTroubleDesk = (ITroubleDesk)ctx["AdministradorTroubleDesk"];
                var tiposDescansos = administradorTroubleDesk.ObtenerTiposDescansos();
                return tiposDescansos;
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
                if (administradorTroubleDesk != null)
                    administradorTroubleDesk.LiberarRecursos();
            }
        }

        public int RegistrarDescanso(BREAK descanso)
        {
            ITroubleDesk administradorTroubleDesk = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringTroubleDesk.xml");
                administradorTroubleDesk = (ITroubleDesk)ctx["AdministradorTroubleDesk"];
                return administradorTroubleDesk.RegistrarDescanso(descanso);
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
                if (administradorTroubleDesk != null)
                    administradorTroubleDesk.LiberarRecursos();
            }
        }

        public void FinalizarDescanso(int idDescanso)
        {
            ITroubleDesk administradorTroubleDesk = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringTroubleDesk.xml");
                administradorTroubleDesk = (ITroubleDesk)ctx["AdministradorTroubleDesk"];
                administradorTroubleDesk.FinalizarDescanso(idDescanso);
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
                if (administradorTroubleDesk != null)
                    administradorTroubleDesk.LiberarRecursos();
            }
        }
    }
}
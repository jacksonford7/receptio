using RECEPTIO.CapaAplicacion.Console.Aplicacion.Contratos;
using RECEPTIO.CapaAplicacion.Console.Aplicacion.Interfaces;
using RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Contratos;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using Spring.Context.Support;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace RECEPTIO.CapaServiciosDistribuidos.Console.Servicios
{
    public partial class ServicioConsole
    {
        public IEnumerable<Ticket> ObtenerTicketsNoAsignados()
        {
            ISupervisor administradorSupervisor = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringSupervisor.xml");
                administradorSupervisor = (ISupervisor)ctx["AdministradorSupervisor"];
                var tickets = administradorSupervisor.ObtenerTicketsNoAsignados();
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
                if (administradorSupervisor != null)
                    administradorSupervisor.LiberarRecursos();
            }
        }

        public short ReasignarTickets(Dictionary<long, Tuple<TipoTicket, short>> tickets)
        {
            ISupervisor administradorSupervisor = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringSupervisor.xml");
                administradorSupervisor = (ISupervisor)ctx["AdministradorSupervisor"];
                return administradorSupervisor.ReasignarTickets(tickets);
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
                if (administradorSupervisor != null)
                    administradorSupervisor.LiberarRecursos();
            }
        }

        public void CancelarTickets(IEnumerable<long> idTickets, string usuario)
        {
            ISupervisor administradorSupervisor = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringSupervisor.xml");
                administradorSupervisor = (ISupervisor)ctx["AdministradorSupervisor"];
                administradorSupervisor.CancelarTickets(idTickets, usuario);
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
                if (administradorSupervisor != null)
                    administradorSupervisor.LiberarRecursos();
            }
        }

        public IEnumerable<Ticket> ObtenerTicketsSuspendidos()
        {
            ISupervisor administradorSupervisor = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringSupervisor.xml");
                administradorSupervisor = (ISupervisor)ctx["AdministradorSupervisor"];
                var tickets = administradorSupervisor.ObtenerTicketsSuspendidos();
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
                if (administradorSupervisor != null)
                    administradorSupervisor.LiberarRecursos();
            }
        }

        public short ReasignarTicketsSuspendidos(Dictionary<long, Tuple<long, short>> tickets, int idMotivo, string usuario)
        {
            ISupervisor administradorSupervisor = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringSupervisor.xml");
                administradorSupervisor = (ISupervisor)ctx["AdministradorSupervisor"];
                return administradorSupervisor.ReasignarTicketsSuspendidos(tickets, idMotivo, usuario);
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
                if (administradorSupervisor != null)
                    administradorSupervisor.LiberarRecursos();
            }
        }

        public short ReasignarTicketsSuspendidosEspecifico(Dictionary<long, Tuple<long, short>> tickets, int idMotivo, string usuario, TROUBLE_DESK_USER usuarioSeleccionado)
        {
            ISupervisor administradorSupervisor = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringSupervisor.xml");
                administradorSupervisor = (ISupervisor)ctx["AdministradorSupervisor"];
                return administradorSupervisor.ReasignarTicketsSuspendidosEspecifico(tickets, idMotivo, usuario,usuarioSeleccionado);
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
                if (administradorSupervisor != null)
                    administradorSupervisor.LiberarRecursos();
            }
        }

        public IEnumerable<TicketReporte> ObtenerTicketsParaReporte(Dictionary<BusquedaTicketReporte, string> filtros)
        {
            ISupervisor administradorSupervisor = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringSupervisor.xml");
                administradorSupervisor = (ISupervisor)ctx["AdministradorSupervisor"];
                var tickets = administradorSupervisor.ObtenerTicketsParaReporte(filtros);
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
                if (administradorSupervisor != null)
                    administradorSupervisor.LiberarRecursos();
            }
        }

        public IEnumerable<ACTION> ObtenerAccionesTicket(long idTicket)
        {
            ISupervisor administradorSupervisor = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringSupervisor.xml");
                administradorSupervisor = (ISupervisor)ctx["AdministradorSupervisor"];
                var acciones = administradorSupervisor.ObtenerAccionesTicket(idTicket);
                return acciones;
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
                if (administradorSupervisor != null)
                    administradorSupervisor.LiberarRecursos();
            }
        }

        public IEnumerable<REASSIGNMENT_MOTIVE> ObtenerMotivosReasignacion()
        {
            ISupervisor administradorSupervisor = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringSupervisor.xml");
                administradorSupervisor = (ISupervisor)ctx["AdministradorSupervisor"];
                var motivos = administradorSupervisor.ObtenerMotivosReasignacion();
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
                if (administradorSupervisor != null)
                    administradorSupervisor.LiberarRecursos();
            }
        }

        public IEnumerable<KIOSK> ObtenerKioscosActivos()
        {
            ISupervisor administradorSupervisor = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringSupervisor.xml");
                administradorSupervisor = (ISupervisor)ctx["AdministradorSupervisor"];
                var quioscos = administradorSupervisor.ObtenerKioscosActivos();
                return quioscos;
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
                if (administradorSupervisor != null)
                    administradorSupervisor.LiberarRecursos();
            }
        }

        public void RegistrarAperturaBarrera(LIFT_UP_BARRIER objecto)
        {
            ISupervisor administradorSupervisor = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringSupervisor.xml");
                administradorSupervisor = (ISupervisor)ctx["AdministradorSupervisor"];
                administradorSupervisor.RegistrarAperturaBarrera(objecto);
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
                if (administradorSupervisor != null)
                    administradorSupervisor.LiberarRecursos();
            }
        }

        public IEnumerable<USER_SESSION> ObtenerSesionesUsuarios()
        {
            ISupervisor administradorSupervisor = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringSupervisor.xml");
                administradorSupervisor = (ISupervisor)ctx["AdministradorSupervisor"];
                var sesionesUsuarios = administradorSupervisor.ObtenerSesionesUsuarios();
                return sesionesUsuarios;
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
                if (administradorSupervisor != null)
                    administradorSupervisor.LiberarRecursos();
            }
        }

        public IEnumerable<PRE_GATE> ObtenerTransaccionesKiosco(Dictionary<BusquedaReporteTransacciones, string> filtros)
        {
            ISupervisor administradorSupervisor = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringSupervisor.xml");
                administradorSupervisor = (ISupervisor)ctx["AdministradorSupervisor"];
                var transacciones = administradorSupervisor.ObtenerTransaccionesKiosco(filtros);
                return transacciones;
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
                if (administradorSupervisor != null)
                    administradorSupervisor.LiberarRecursos();
            }
        }

        public IEnumerable<DEVICE> ObtenerTablets()
        {
            ISupervisor administradorSupervisor = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringSupervisor.xml");
                administradorSupervisor = (ISupervisor)ctx["AdministradorSupervisor"];
                var tablets = administradorSupervisor.ObtenerTablets();
                return tablets;
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
                if (administradorSupervisor != null)
                    administradorSupervisor.LiberarRecursos();
            }
        }

        public IEnumerable<ZONE> ObtenerZonas()
        {
            ISupervisor administradorSupervisor = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringSupervisor.xml");
                administradorSupervisor = (ISupervisor)ctx["AdministradorSupervisor"];
                var zonas = administradorSupervisor.ObtenerZonas();
                return zonas;
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
                if (administradorSupervisor != null)
                    administradorSupervisor.LiberarRecursos();
            }
        }

        public IEnumerable<TRANSACTION_TYPE> ObtenerTiposTransacciones()
        {
            ISupervisor administradorSupervisor = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringSupervisor.xml");
                administradorSupervisor = (ISupervisor)ctx["AdministradorSupervisor"];
                var tiposTransaciones = administradorSupervisor.ObtenerTiposTransacciones();
                return tiposTransaciones;
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
                if (administradorSupervisor != null)
                    administradorSupervisor.LiberarRecursos();
            }
        }

        public Tuple<bool, string, PRE_GATE> ObtenerInformacionParaReimpresionTicket(long idPreGate, bool esEntrada)
        {
            ISupervisor administradorSupervisor = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringSupervisor.xml");
                administradorSupervisor = (ISupervisor)ctx["AdministradorSupervisor"];
                var info = administradorSupervisor.ObtenerInformacionParaReimpresionTicket(idPreGate, esEntrada);
                return info;
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
                if (administradorSupervisor != null)
                    administradorSupervisor.LiberarRecursos();
            }
        }

        public void RegistrarReimpresion(REPRINT objecto)
        {
            ISupervisor administradorSupervisor = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringSupervisor.xml");
                administradorSupervisor = (ISupervisor)ctx["AdministradorSupervisor"];
                administradorSupervisor.RegistrarReimpresion(objecto);
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
                if (administradorSupervisor != null)
                    administradorSupervisor.LiberarRecursos();
            }
        }

        public void ActualizarrByPass(BY_PASS byPass, int idUsuario)
        {
            ISupervisor administradorSupervisor = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringSupervisor.xml");
                administradorSupervisor = (ISupervisor)ctx["AdministradorSupervisor"];
                administradorSupervisor.ActualizarrByPass(byPass, idUsuario);
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
                if (administradorSupervisor != null)
                    administradorSupervisor.LiberarRecursos();
            }
        }

        public void CrearByPass(BY_PASS byPass, int idUsuario)
        {
            ISupervisor administradorSupervisor = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringSupervisor.xml");
                administradorSupervisor = (ISupervisor)ctx["AdministradorSupervisor"];
                administradorSupervisor.CrearByPass(byPass, idUsuario);
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
                if (administradorSupervisor != null)
                    administradorSupervisor.LiberarRecursos();
            }
        }

        public BY_PASS ObtenerByPass(long idPreGate)
        {
            ISupervisor administradorSupervisor = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringSupervisor.xml");
                administradorSupervisor = (ISupervisor)ctx["AdministradorSupervisor"];
                var byPass = administradorSupervisor.ObtenerByPass(idPreGate);
                return byPass;
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
                if (administradorSupervisor != null)
                    administradorSupervisor.LiberarRecursos();
            }
        }

        public Respuesta ValidarIdPreGate(long idPreGate)
        {
            ISupervisor administradorSupervisor = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringSupervisor.xml");
                administradorSupervisor = (ISupervisor)ctx["AdministradorSupervisor"];
                var respuesta = administradorSupervisor.ValidarIdPreGate(idPreGate);
                return respuesta;
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
                if (administradorSupervisor != null)
                    administradorSupervisor.LiberarRecursos();
            }
        }

        public Respuesta ValidarIdPreGateParaCancelar(long idPreGate)
        {
            ISupervisor administradorSupervisor = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringSupervisor.xml");
                administradorSupervisor = (ISupervisor)ctx["AdministradorSupervisor"];
                var respuesta = administradorSupervisor.ValidarIdPreGateParaCancelar(idPreGate);
                return respuesta;
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
                if (administradorSupervisor != null)
                    administradorSupervisor.LiberarRecursos();
            }
        }

        public void CrearByPassCancelPregate(BY_PASS byPass, int idUsuario)
        {
            ISupervisor administradorSupervisor = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringSupervisor.xml");
                administradorSupervisor = (ISupervisor)ctx["AdministradorSupervisor"];
                administradorSupervisor.CrearByPassCancelPregate(byPass, idUsuario);
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
                if (administradorSupervisor != null)
                    administradorSupervisor.LiberarRecursos();
            }
        }

        public void ActualizarStatusPregate(long idPreGate, string Status)
        {
            ISupervisor administradorSupervisor = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringSupervisor.xml");
                administradorSupervisor = (ISupervisor)ctx["AdministradorSupervisor"];
                administradorSupervisor.ActualizarStatusPregate(idPreGate, Status);
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
                if (administradorSupervisor != null)
                    administradorSupervisor.LiberarRecursos();
            }
        }

        public void ActualizarStatusStockRegister(long idPreGate, bool Status)
        {
            ISupervisor administradorSupervisor = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringSupervisor.xml");
                administradorSupervisor = (ISupervisor)ctx["AdministradorSupervisor"];
                administradorSupervisor.ActualizarStatusStockRegister(idPreGate, Status);
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
                if (administradorSupervisor != null)
                    administradorSupervisor.LiberarRecursos();
            }
        }

        public IEnumerable<MOTIVE> ObtenerMotivos(int Type)
        {
            ISupervisor administradorSupervisor = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringSupervisor.xml");
                administradorSupervisor = (ISupervisor)ctx["AdministradorSupervisor"];
                var motivos = administradorSupervisor.ObtenerMotivos(Type);
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
                if (administradorSupervisor != null)
                    administradorSupervisor.LiberarRecursos();
            }
        }

        public IEnumerable<SUB_MOTIVE> ObtenerSubMotivos(int idMotivo)
        {
            ISupervisor administradorSupervisor = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringSupervisor.xml");
                administradorSupervisor = (ISupervisor)ctx["AdministradorSupervisor"];
                var motivos = administradorSupervisor.ObtenerSubMotivos(idMotivo);
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
                if (administradorSupervisor != null)
                    administradorSupervisor.LiberarRecursos();
            }
        }

        public string ObtenerValidacionesGenerales(string _opcion, string _StrValor, int _IntValor, long _BigintValor)
        {
            ISupervisor administradorSupervisor = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringSupervisor.xml");
                administradorSupervisor = (ISupervisor)ctx["AdministradorSupervisor"];
                return administradorSupervisor.ObtenerValidacionesGenerales(_opcion, _StrValor, _IntValor, _BigintValor);
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
                if (administradorSupervisor != null)
                    administradorSupervisor.LiberarRecursos();
            }
        }

    }
}
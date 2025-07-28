using RECEPTIO.CapaAplicacion.Console.Aplicacion.Contratos;
using RECEPTIO.CapaAplicacion.Console.Aplicacion.Interfaces;
using RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Contratos;
using RECEPTIO.CapaDominio.Console.Dominio.InterfacesRepositorios;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Filtros;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Repositorio;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using RECEPTIO.CapaDominio.Nucleo.ServiciosDominio.Otros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Transactions;

namespace RECEPTIO.CapaDominio.Console.ServiciosDominio
{
    public class ServicioSupervisor : AsignacionProblema, ISupervisor
    {
        internal readonly IRepositorioTroubleTicket RepositorioTroubleTicket;
        internal readonly IRepositorioZone RepositorioZone;
        internal readonly IRepositorioUserSession RepositorioUserSession;
        private readonly IRepositorioReassignmentMotive _repositorioReassignmentMotive;
        private readonly IRepositorioQuiosco _repositorioQuiosco;
        private readonly IRepositorioLiftBarrier _repositorioLiftBarrier;
        private readonly IRepositorioAction _repositorioAction;
        private readonly IRepositorioPreGate _repositorioPreGate;
        private readonly IRepositorioDevice _repositorioDevice;
        private readonly IRepositorioTransactionType _repositorioTransactionType;
        private readonly IRepositorioReprinter _repositorioReprinter;
        private readonly IRepositorioByPass _repositorioPass;
        private readonly IRepositorioStockRegister _repositorioStockRegister;
        private readonly IRepositorioMotive _repositorioMotive;
        private readonly IRepositorioSubMotive _repositorioSubMotive;
        internal List<Tuple<short, List<USER_SESSION>>> SesionesUsuarios;
        internal Dictionary<long, Tuple<TipoTicket, short>> Tickets;
        internal short Resultado;
        internal bool EsReasignacionSuspendidos;
        internal Dictionary<long, Tuple<long, short>> TicketsSuspendidos;
        internal REASSIGNMENT Reasignacion;
        private EstadoReasignacionTicket _estado;

        public ServicioSupervisor(IRepositorioTroubleTicket repositorioTroubleTicket, IRepositorioZone repositorioZone, IRepositorioUserSession repositorioUserSession, IRepositorioReassignmentMotive repositorioReassignmentMotive, IRepositorioQuiosco repositorioQuiosco, IRepositorioLiftBarrier repositorioLiftBarrier, IRepositorioAction repositorioAction, IRepositorioPreGate repositorioPreGate, IRepositorioDevice repositorioDevice, IRepositorioTransactionType repositorioTransactionType, IRepositorioReprinter repositorioReprinter, IRepositorioByPass repositorioPass, IRepositorioStockRegister repositorioStockRegister, IRepositorioMotive repositorioMotive, IRepositorioSubMotive repositorioSubMotive)
        {
            RepositorioTroubleTicket = repositorioTroubleTicket;
            RepositorioZone = repositorioZone;
            RepositorioUserSession = repositorioUserSession;
            _repositorioReassignmentMotive = repositorioReassignmentMotive;
            _repositorioQuiosco = repositorioQuiosco;
            _repositorioLiftBarrier = repositorioLiftBarrier;
            _repositorioAction = repositorioAction;
            _repositorioPreGate = repositorioPreGate;
            _repositorioDevice = repositorioDevice;
            _repositorioTransactionType = repositorioTransactionType;
            _repositorioReprinter = repositorioReprinter;
            _repositorioPass = repositorioPass;
            _repositorioStockRegister = repositorioStockRegister;
            _repositorioMotive = repositorioMotive;
            _repositorioSubMotive = repositorioSubMotive;

        }

        public IEnumerable<Ticket> ObtenerTicketsNoAsignados()
        {
            var tickets = new List<Ticket>();
            AgregarItemsProceso(tickets);
            AgregarItemsMobile(tickets);
            AgregarItemsTecnico(tickets);
            return tickets.OrderByDescending(t => t.IdTicket);
        }

        private void AgregarItemsProceso(List<Ticket> tickets)
        {
            var ticketsProceso = RepositorioTroubleTicket.ObtenerTicketProcesos(new FiltroTicketProcesoNoAsignados());
            foreach (var item in ticketsProceso)
                tickets.Add(new Ticket
                {
                    EsEntrada = item.PROCESS.KIOSK_TRANSACTION.KIOSK.IS_IN,
                    IdQuiosco = item.PROCESS.KIOSK_TRANSACTION.KIOSK.KIOSK_ID,
                    IpQuiosco = item.PROCESS.KIOSK_TRANSACTION.KIOSK.IP,
                    IdTicket = item.TT_ID,
                    IdTransaccionQuiosco = item.PROCESS.TRANSACTION_ID,
                    Mensaje = item.PROCESS.MESSAGE.TROUBLE_DESK_MESSAGE,
                    Notas = item.NOTES,
                    NombreProceso = item.PROCESS.STEP,
                    NombreQuiosco = item.PROCESS.KIOSK_TRANSACTION.KIOSK.NAME,
                    Tipo = TipoTicket.Proceso,
                    TipoTransaccion = item.PROCESS.KIOSK_TRANSACTION.PRE_GATE == null ? "DESC" : item.PROCESS.KIOSK_TRANSACTION.PRE_GATE.PRE_GATE_DETAILS.FirstOrDefault().TRANSACTION_TYPE.CODE,
                    FechaCreacion = item.CREATION_DATE,
                    Zona = item.PROCESS.KIOSK_TRANSACTION.KIOSK.ZONE.NAME,
                    IdZona = item.PROCESS.KIOSK_TRANSACTION.KIOSK.ZONE_ID,
                    EstaSuspendido = item.IS_SUSPENDED
                });
        }

        private void AgregarItemsMobile(List<Ticket> tickets)
        {
            var ticketsMobile = RepositorioTroubleTicket.ObtenerTicketMobile(new FiltroTicketMobileNoAsignados());
            foreach (var item in ticketsMobile)
                tickets.Add(new Ticket
                {
                    IdTicket = item.TT_ID,
                    Mensaje = item.MESSAGE,
                    Tipo = TipoTicket.Mobile,
                    Notas = item.NOTES,
                    FechaCreacion = item.CREATION_DATE,
                    IdZona = item.ZONE_ID
                });
        }

        private void AgregarItemsTecnico(List<Ticket> tickets)
        {
            var ticketsTecnico = RepositorioTroubleTicket.ObtenerTicketTecnico(new FiltroTicketTecnicoNoAsignados());
            foreach (var item in ticketsTecnico)
                tickets.Add(new Ticket
                {
                    IdTicket = item.TT_ID,
                    Mensaje = item.ERROR == null ? "Ha ocurrido un inconveniente.Contactarse con el Departamento de IT." : item.ERROR.MESSAGE,
                    Tipo = TipoTicket.Tecnico,
                    Notas = item.NOTES,
                    FechaCreacion = item.CREATION_DATE,
                    Zona = item.ZONE.NAME,
                    IdZona = item.ZONE_ID
                });
        }

        public short ReasignarTickets(Dictionary<long, Tuple<TipoTicket, short>> tickets)
        {
            Tickets = tickets;
            using (var transaccion = new TransactionScope())
            {
                _estado = new EstadoObtenerSesionesUsuariosPorZonas();
                _estado.Ejecutar(this);
                transaccion.Complete();
            }
            return Resultado;
        }

        public void CancelarTickets(IEnumerable<long> idTickets, string userName)
        {
            using (var transaccion = new TransactionScope())
            {
                foreach (var idTicket in idTickets)
                {
                    var ticket = RepositorioTroubleTicket.ObtenerObjetos(new FiltroTroubleTicketPoId(idTicket)).FirstOrDefault();
                    if (ticket == null)
                        throw new ApplicationException($"No existe ticket cuyo id es {idTicket}");
                    ticket.IS_CANCEL = true;
                    ticket.CANCEL_DATE = DateTime.Now;
                    ticket.CANCEL_USER = userName;
                    RepositorioTroubleTicket.Actualizar(ticket);
                }
                transaccion.Complete();
            }
        }

        public IEnumerable<Ticket> ObtenerTicketsSuspendidos()
        {
            var tickets = new List<Ticket>();
            AgregarItems(tickets);
            return tickets.OrderByDescending(t => t.IdTicket);
        }

        private void AgregarItems(List<Ticket> tickets)
        {
            var ticketsProceso = RepositorioTroubleTicket.ObtenerTicketProcesos(new FiltroTicketsSuspendidos());
            foreach (var item in ticketsProceso)
                tickets.Add(new Ticket
                {
                    EsEntrada = item.PROCESS.KIOSK_TRANSACTION.KIOSK.IS_IN,
                    IdQuiosco = item.PROCESS.KIOSK_TRANSACTION.KIOSK.KIOSK_ID,
                    IpQuiosco = item.PROCESS.KIOSK_TRANSACTION.KIOSK.IP,
                    IdTicket = item.TT_ID,
                    IdTransaccionQuiosco = item.PROCESS.TRANSACTION_ID,
                    Mensaje = item.PROCESS.MESSAGE.TROUBLE_DESK_MESSAGE,
                    Notas = item.NOTES,
                    NombreProceso = item.PROCESS.STEP,
                    NombreQuiosco = item.PROCESS.KIOSK_TRANSACTION.KIOSK.NAME,
                    Tipo = TipoTicket.Proceso,
                    TipoTransaccion = item.PROCESS.KIOSK_TRANSACTION.PRE_GATE == null ? "DESC" : item.PROCESS.KIOSK_TRANSACTION.PRE_GATE.PRE_GATE_DETAILS.FirstOrDefault().TRANSACTION_TYPE.CODE,
                    FechaCreacion = item.CREATION_DATE,
                    Zona = item.PROCESS.KIOSK_TRANSACTION.KIOSK.ZONE.NAME,
                    IdZona = item.PROCESS.KIOSK_TRANSACTION.KIOSK.ZONE_ID,
                    EstaSuspendido = item.IS_SUSPENDED,
                    IdSesionUsuario = item.USER_SESSION_ID,
                    Responsable = item.USER_SESSION == null ? "" : item.USER_SESSION.TROUBLE_DESK_USER.TTU_ID + " - " +item.USER_SESSION.TROUBLE_DESK_USER.USER_NAME
                });
        }

        public short ReasignarTicketsSuspendidos(Dictionary<long, Tuple<long, short>> tickets, int idMotivo, string usuario)
        {
            TicketsSuspendidos = tickets;
            EsReasignacionSuspendidos = true;
            Reasignacion = new REASSIGNMENT { DATE = DateTime.Now, MOTIVE_ID = idMotivo, USER = usuario };
            using (var transaccion = new TransactionScope())
            {
                _estado = new EstadoObtenerSesionesUsuariosPorZonas();
                _estado.Ejecutar(this);
                transaccion.Complete();
            }
            return Resultado;
        }
                     
        public short ReasignarTicketsSuspendidosEspecifico(Dictionary<long, Tuple<long, short>> tickets, int idMotivo, string usuario, TROUBLE_DESK_USER usuarioSeleccionado)
        {
            EsReasignacionSuspendidos = true;
            Reasignacion = new REASSIGNMENT { DATE = DateTime.Now, MOTIVE_ID = idMotivo, USER = usuario };
            using (var transaccion = new TransactionScope())
            {
                USER_SESSION sesionUsuario = usuarioSeleccionado.USER_SESSIONS.FirstOrDefault();
                this.RepositorioTroubleTicket.ReasignarTicketProceso(tickets.FirstOrDefault().Key, sesionUsuario.ID, this.Reasignacion);
                transaccion.Complete();
            }

            if (this.Resultado == 0)
            {
                this.Resultado = 2;
            }
               
            return Resultado;
        }

        public IEnumerable<TicketReporte> ObtenerTicketsParaReporte(Dictionary<BusquedaTicketReporte, string> filtros)
        {
            var tickets = new List<TicketReporte>();
            AgregarTicketsSegunFiltro(tickets, filtros);
            var datos = tickets.AsEnumerable();
            foreach (var item in filtros.Where(f => f.Key != BusquedaTicketReporte.Id && f.Key != BusquedaTicketReporte.Fecha))
            {
                var manejadorBusqueda = new ManejadorBusquedaPorContenedor();
                manejadorBusqueda.ManejarFiltros(item, ref datos);
            }
            return datos.OrderByDescending(rt => rt.IdTicket);
        }

        private void AgregarTicketsSegunFiltro(List<TicketReporte> tickets, Dictionary<BusquedaTicketReporte, string> filtros)
        {
            var valorFiltro = filtros.FirstOrDefault(f => f.Key == BusquedaTicketReporte.TipoTicket).Value;
            if (valorFiltro == "PR" || valorFiltro == "TO")
                AgregarItemsProceso(tickets, filtros);
            if (valorFiltro == "MB" || valorFiltro == "TO")
                AgregarItemsMobile(tickets, filtros);
            if (valorFiltro == "TC" || valorFiltro == "TO")
                AgregarItemsTecnico(tickets, filtros);
            if (valorFiltro == "AT" || valorFiltro == "TO")
                AgregarItemsAuto(tickets, filtros);
        }

        private void AgregarItemsProceso(List<TicketReporte> tickets, Dictionary<BusquedaTicketReporte, string> filtros)
        {
            IEnumerable<PROCESS_TROUBLE_TICKET> ticketsProceso;
            if (filtros.ContainsKey(BusquedaTicketReporte.Id))
                ticketsProceso = RepositorioTroubleTicket.ObtenerTicketProcesos(new FiltroTicketProcesoPorIdYFecha(filtros.FirstOrDefault(f => f.Key == BusquedaTicketReporte.Id).Value, filtros.FirstOrDefault(f => f.Key == BusquedaTicketReporte.Fecha).Value));
            else
                ticketsProceso = RepositorioTroubleTicket.ObtenerTicketProcesos(new FiltroTicketProcesoPorFecha(filtros.FirstOrDefault(f => f.Key == BusquedaTicketReporte.Fecha).Value));
            foreach (var item in ticketsProceso)
            {
                StringBuilder contenedores = new StringBuilder();
                if (item.PROCESS.KIOSK_TRANSACTION.PRE_GATE != null)
                {
                    foreach (var detalle in item.PROCESS.KIOSK_TRANSACTION.PRE_GATE.PRE_GATE_DETAILS)
                        contenedores.Append(detalle.CONTAINERS == null ? "" : detalle.CONTAINERS.Aggregate("", (actual, i) => (actual == "" ? "" : actual + ",") + i.NUMBER + ", "));
                }
                tickets.Add(new TicketReporte
                {
                    EsEntrada = item.PROCESS.KIOSK_TRANSACTION.KIOSK.IS_IN,
                    IdQuiosco = item.PROCESS.KIOSK_TRANSACTION.KIOSK.KIOSK_ID,
                    IpQuiosco = item.PROCESS.KIOSK_TRANSACTION.KIOSK.IP,
                    IdTicket = item.TT_ID,
                    IdTransaccionQuiosco = item.PROCESS.TRANSACTION_ID,
                    Mensaje = item.PROCESS.MESSAGE.TROUBLE_DESK_MESSAGE,
                    Notas = item.NOTES,
                    NombreProceso = item.PROCESS.STEP,
                    NombreQuiosco = item.PROCESS.KIOSK_TRANSACTION.KIOSK.NAME,
                    Tipo = TipoTicket.Proceso,
                    TipoTransaccion = item.PROCESS.KIOSK_TRANSACTION.PRE_GATE == null ? "DESC" : item.PROCESS.KIOSK_TRANSACTION.PRE_GATE.PRE_GATE_DETAILS.FirstOrDefault().TRANSACTION_TYPE.CODE,
                    FechaCreacion = item.CREATION_DATE,
                    Zona = item.PROCESS.KIOSK_TRANSACTION.KIOSK.ZONE.NAME,
                    IdZona = item.PROCESS.KIOSK_TRANSACTION.KIOSK.ZONE_ID,
                    EstaSuspendido = item.IS_SUSPENDED,
                    CedulaChofer = item.PROCESS.KIOSK_TRANSACTION.PRE_GATE == null ? "" : item.PROCESS.KIOSK_TRANSACTION.PRE_GATE.DRIVER_ID,
                    Contenedores = item.PROCESS.KIOSK_TRANSACTION.PRE_GATE == null || string.IsNullOrWhiteSpace(contenedores.ToString()) ? "" : contenedores.ToString().Substring(0, contenedores.Length - 3),
                    FechaAceptacion = item.ACCEPTANCE_DATE,
                    FechaAsignacion = item.ASSIGNMENT_DATE ?? new DateTime(),
                    FechaFinalizacion = item.FINISH_DATE,
                    IdSesionUsuario = item.USER_SESSION_ID,
                    PlacaVehiculo = item.PROCESS.KIOSK_TRANSACTION.PRE_GATE == null ? "" : item.PROCESS.KIOSK_TRANSACTION.PRE_GATE.TRUCK_LICENCE,
                    UserName = item.USER_SESSION == null ? "" : item.USER_SESSION.TROUBLE_DESK_USER.USER_NAME,
                    EstaCancelado = item.IS_CANCEL,
                    FechaCancelacion = item.CANCEL_DATE,
                    UsuarioCancelacion = item.CANCEL_USER,
                    IdPreGate = item.PROCESS.KIOSK_TRANSACTION.PRE_GATE_ID ?? 0
                });
            }
        }

        private void AgregarItemsMobile(List<TicketReporte> tickets, Dictionary<BusquedaTicketReporte, string> filtros)
        {
            IEnumerable<MOBILE_TROUBLE_TICKET> ticketsMobile;
            if (filtros.ContainsKey(BusquedaTicketReporte.Id))
                ticketsMobile = RepositorioTroubleTicket.ObtenerTicketMobile(new FiltroTicketMobilePorIdYFecha(filtros.FirstOrDefault(f => f.Key == BusquedaTicketReporte.Id).Value, filtros.FirstOrDefault(f => f.Key == BusquedaTicketReporte.Fecha).Value));
            else
                ticketsMobile = RepositorioTroubleTicket.ObtenerTicketMobile(new FiltroTicketMobilePorFecha(filtros.FirstOrDefault(f => f.Key == BusquedaTicketReporte.Fecha).Value));
            foreach (var item in ticketsMobile)
            {
                tickets.Add(new TicketReporte
                {
                    IdTicket = item.TT_ID,
                    Notas = item.NOTES,
                    Tipo = TipoTicket.Mobile,
                    Mensaje = item.MESSAGE,
                    FechaCreacion = item.CREATION_DATE,
                    Zona = item.ZONE.NAME,
                    IdZona = item.ZONE_ID,
                    FechaAceptacion = item.ACCEPTANCE_DATE,
                    FechaAsignacion = item.ASSIGNMENT_DATE ?? new DateTime(),
                    FechaFinalizacion = item.FINISH_DATE,
                    IdSesionUsuario = item.USER_SESSION_ID,
                    UserName = item.USER_SESSION == null ? "" : item.USER_SESSION.TROUBLE_DESK_USER.USER_NAME,
                    EstaCancelado = item.IS_CANCEL,
                    FechaCancelacion = item.CANCEL_DATE,
                    UsuarioCancelacion = item.CANCEL_USER
                });
            }
        }

        private void AgregarItemsTecnico(List<TicketReporte> tickets, Dictionary<BusquedaTicketReporte, string> filtros)
        {
            IEnumerable<CLIENT_APP_TRANSACTION_TROUBLE_TICKET> ticketsTecnico;
            if (filtros.ContainsKey(BusquedaTicketReporte.Id))
                ticketsTecnico = RepositorioTroubleTicket.ObtenerTicketTecnico(new FiltroTicketTecnicoPorIdYFecha(filtros.FirstOrDefault(f => f.Key == BusquedaTicketReporte.Id).Value, filtros.FirstOrDefault(f => f.Key == BusquedaTicketReporte.Fecha).Value));
            else
                ticketsTecnico = RepositorioTroubleTicket.ObtenerTicketTecnico(new FiltroTicketTecnicoPorFecha(filtros.FirstOrDefault(f => f.Key == BusquedaTicketReporte.Fecha).Value));
            foreach (var item in ticketsTecnico)
            {
                tickets.Add(new TicketReporte
                {
                    IdTicket = item.TT_ID,
                    Notas = item.NOTES,
                    Tipo = TipoTicket.Tecnico,
                    FechaCreacion = item.CREATION_DATE,
                    Zona = item.ZONE.NAME,
                    IdZona = item.ZONE_ID,
                    FechaAceptacion = item.ACCEPTANCE_DATE,
                    FechaAsignacion = item.ASSIGNMENT_DATE ?? new DateTime(),
                    FechaFinalizacion = item.FINISH_DATE,
                    IdSesionUsuario = item.USER_SESSION_ID,
                    UserName = item.USER_SESSION == null ? "" : item.USER_SESSION.TROUBLE_DESK_USER.USER_NAME,
                    EstaCancelado = item.IS_CANCEL,
                    FechaCancelacion = item.CANCEL_DATE,
                    UsuarioCancelacion = item.CANCEL_USER,
                });
            }
        }

        private void AgregarItemsAuto(List<TicketReporte> tickets, Dictionary<BusquedaTicketReporte, string> filtros)
        {
            IEnumerable<AUTO_TROUBLE_TICKET> ticketsAuto;
            if (filtros.ContainsKey(BusquedaTicketReporte.Id))
                ticketsAuto = RepositorioTroubleTicket.ObtenerAutoTicket(new FiltroTicketAutoPorIdYFecha(filtros.FirstOrDefault(f => f.Key == BusquedaTicketReporte.Id).Value, filtros.FirstOrDefault(f => f.Key == BusquedaTicketReporte.Fecha).Value));
            else
                ticketsAuto = RepositorioTroubleTicket.ObtenerAutoTicket(new FiltroTicketAutoPorFecha(filtros.FirstOrDefault(f => f.Key == BusquedaTicketReporte.Fecha).Value));
            foreach (var item in ticketsAuto)
            {
                tickets.Add(new TicketReporte
                {
                    IdTicket = item.TT_ID,
                    Notas = item.NOTES,
                    Tipo = TipoTicket.Auto,
                    FechaCreacion = item.CREATION_DATE,
                    Zona = item.USER_SESSION.DEVICE.ZONE.NAME,
                    IdZona = item.USER_SESSION.DEVICE.ZONE_ID,
                    FechaAceptacion = item.ACCEPTANCE_DATE,
                    FechaAsignacion = item.ASSIGNMENT_DATE.Value,
                    FechaFinalizacion = item.FINISH_DATE,
                    IdSesionUsuario = item.USER_SESSION_ID,
                    UserName = item.USER_SESSION.TROUBLE_DESK_USER.USER_NAME,
                    MotivoAutoTicket = item.AUTO_TROUBLE_REASON.CAPTION,
                    EstaCancelado = item.IS_CANCEL,
                    FechaCancelacion = item.CANCEL_DATE,
                    UsuarioCancelacion = item.CANCEL_USER,
                });
            }
        }

        public IEnumerable<ACTION> ObtenerAccionesTicket(long idTicket)
        {
            return _repositorioAction.ObtenerObjetos(new FiltroAccionPorIdTicket(idTicket)).OrderBy(a => a.ACTION_DATE).ToList();
        }

        public IEnumerable<REASSIGNMENT_MOTIVE> ObtenerMotivosReasignacion()
        {
            return _repositorioReassignmentMotive.ObtenerObjetos(new FiltroMotivoReasignacionActivos()).OrderBy(rm => rm.CAPTION).ToList();
        }

        public IEnumerable<KIOSK> ObtenerKioscosActivos()
        {
            return _repositorioQuiosco.ObtenerObjetos(new FiltroQuioscoActivos()).OrderBy(k => k.NAME).ToList();
        }

        public void RegistrarAperturaBarrera(LIFT_UP_BARRIER objecto)
        {
            objecto.DATE = DateTime.Now;
            _repositorioLiftBarrier.Agregar(objecto);
        }

        public IEnumerable<USER_SESSION> ObtenerSesionesUsuarios()
        {
            return RepositorioUserSession.ObtenerSesionUsuarioConDispositivoZonaUsuario(new FiltroSesionUsuarioAbiertasParaCerrarlas()).OrderBy(us => us.START_SESSION__DATE).ToList();
        }

        public IEnumerable<PRE_GATE> ObtenerTransaccionesKiosco(Dictionary<BusquedaReporteTransacciones, string> filtros)
        {
            var resultado = _repositorioPreGate.ObtenerPreGateConDetalleParaSupervisor(new FiltroPreGatePorFecha(filtros.FirstOrDefault(f => f.Key == BusquedaReporteTransacciones.Fecha).Value)).AsEnumerable();
            foreach (var item in filtros.Where(f => f.Key != BusquedaReporteTransacciones.Fecha))
            {
                var manejadorBusqueda = new ManejadorBusquedaPorGosTv();
                manejadorBusqueda.ManejarFiltros(item, ref resultado);
            }
            return resultado.OrderByDescending(r => r.PRE_GATE_ID).ToList();
        }

        public IEnumerable<DEVICE> ObtenerTablets()
        {
            return _repositorioDevice.ObtenerObjetos(new FiltroDispositivoTablets()).OrderBy(d => d.NAME).ToList();
        }

        public IEnumerable<ZONE> ObtenerZonas()
        {
            return RepositorioZone.ObtenerObjetos(new FiltroZonasActivas()).OrderBy(z => z.NAME).ToList();
        }

        public IEnumerable<TRANSACTION_TYPE> ObtenerTiposTransacciones()
        {
            return _repositorioTransactionType.ObtenerObjetos(new FiltroTipoTransaccion()).OrderBy(tt => tt.DESCRIPTION).ToList();
        }

        public Tuple<bool, string, PRE_GATE> ObtenerInformacionParaReimpresionTicket(long idPreGate, bool esEntrada)
        {
            var pregate = _repositorioPreGate.ObtenerPreGateConDetalle(new FiltroPreGatePorId(idPreGate)).FirstOrDefault();
            if (pregate == null)
                return new Tuple<bool, string, PRE_GATE>(false, "No existe el código ingresado.", null);
            if (esEntrada)
            {
                if (pregate.KIOSK_TRANSACTIONS.Any(kt => kt.KIOSK.IS_IN && kt.PROCESSES.Any(p => p.IS_OK && p.STEP == "PROCESO_N4")))
                    return new Tuple<bool, string, PRE_GATE>(true, "", pregate);
                return new Tuple<bool, string, PRE_GATE>(false, "No exite un proceso de N4 exitoso", null);
            }
            if (pregate.KIOSK_TRANSACTIONS.Any(kt => !kt.KIOSK.IS_IN && kt.PROCESSES.Any(p => p.IS_OK && p.STEP == "PROCESO_N4")))
                return new Tuple<bool, string, PRE_GATE>(true, "", pregate);
            return new Tuple<bool, string, PRE_GATE>(false, "No exite un proceso de N4 exitoso", null);
        }

        public void RegistrarReimpresion(REPRINT objecto)
        {
            objecto.DATE = DateTime.Now;
            _repositorioReprinter.Agregar(objecto);
        }

        public Respuesta ValidarIdPreGate(long idPreGate)
        {
            var preGate = _repositorioPreGate.ObtenerPreGateConDetalle(new FiltroPreGatePorId(idPreGate)).FirstOrDefault();
            if (preGate == null)
                return new Respuesta { Mensaje = "No Existe id" };
            else if (preGate.STATUS == "O")
                return new Respuesta { Mensaje = "No se puede utilizar el id porque ya existe una transacción de salida exitosa." };
            else if (preGate.BY_PASS != null)
                return new Respuesta { Mensaje = "Ya existe un bypass para este Id." };
            else
                return new Respuesta { FueOk = true, Mensaje = "" };
        }

        public void CrearByPass(BY_PASS byPass, int idUsuario)
        {
            byPass.BY_PASS_AUDITS = new List<BY_PASS_AUDIT>
            {
                new BY_PASS_AUDIT
                {
                    DATE = DateTime.Now,
                    FIELD = "REASON",
                    NEW_VALUE = byPass.REASON,
                    OLD_VALUE = "",
                    TTU_ID = idUsuario
                },
                new BY_PASS_AUDIT
                {
                    DATE = DateTime.Now,
                    FIELD = "IS_ENABLED",
                    NEW_VALUE = "1",
                    OLD_VALUE = "",
                    TTU_ID = idUsuario
                }
            };
            _repositorioPass.InsertarRegistro(byPass);
        }

        public BY_PASS ObtenerByPass(long idPreGate)
        {
            var preGate = _repositorioPreGate.ObtenerPreGateConDetalle(new FiltroPreGatePorId(idPreGate)).FirstOrDefault();
            if (preGate == null)
                return null;
            return preGate.BY_PASS;
        }

        public void ActualizarrByPass(BY_PASS byPass, int idUsuario)
        {
            var byPassAModificar = _repositorioPass.ObtenerObjetos(new FiltroByPassPorId(byPass.ID)).FirstOrDefault();
            if (byPassAModificar.IS_ENABLED != byPass.IS_ENABLED)
                byPassAModificar.BY_PASS_AUDITS.Add(new BY_PASS_AUDIT
                {
                    DATE = DateTime.Now,
                    FIELD = "IS_ENABLED",
                    NEW_VALUE = byPass.IS_ENABLED ? "1" : "0",
                    OLD_VALUE = byPassAModificar.IS_ENABLED ? "1" : "0",
                    TTU_ID = idUsuario
                });
            if (byPassAModificar.REASON != byPass.REASON)
                byPassAModificar.BY_PASS_AUDITS.Add(new BY_PASS_AUDIT
                {
                    DATE = DateTime.Now,
                    FIELD = "REASON",
                    NEW_VALUE = byPass.REASON,
                    OLD_VALUE = byPassAModificar.REASON,
                    TTU_ID = idUsuario
                });
            byPassAModificar.REASON = byPass.REASON;
            byPassAModificar.IS_ENABLED = byPass.IS_ENABLED;
            _repositorioPass.Actualizar(byPassAModificar);
        }

        public Respuesta ValidarIdPreGateParaCancelar(long idPreGate)
        {
            var preGate = _repositorioPreGate.ObtenerPreGateConDetalle(new FiltroPreGatePorId(idPreGate)).FirstOrDefault();
            if (preGate == null)
                return new Respuesta { Mensaje = "No Existe id" };
            else if (preGate.STATUS != "N" && preGate.STATUS != "P")
                return new Respuesta { Mensaje = "No se puede utilizar el id porque estado es diferente a nuevo o en proceso" };
            else
                return new Respuesta { FueOk = true, Mensaje = "" };
        }

        public void CrearByPassCancelPregate(BY_PASS byPass, int idUsuario)
        {
            var preGate = _repositorioPreGate.ObtenerPreGateConDetalle(new FiltroPreGatePorId(byPass.PRE_GATE.PRE_GATE_ID)).FirstOrDefault();

            if (preGate.BY_PASS != null)
            {
                var byPassAModificar = _repositorioPass.ObtenerObjetos(new FiltroByPassPorId(preGate.BY_PASS.ID)).FirstOrDefault();
                
                    byPassAModificar.BY_PASS_AUDITS.Add(new BY_PASS_AUDIT
                    {
                        DATE = DateTime.Now,
                        FIELD = "REASON",
                        NEW_VALUE = byPass.REASON,
                        OLD_VALUE = "",
                        TTU_ID = idUsuario
                    });
                
                    byPassAModificar.BY_PASS_AUDITS.Add(new BY_PASS_AUDIT
                    {
                        DATE = DateTime.Now,
                        FIELD = "PREGATE STATUS",
                        NEW_VALUE = "C",
                        OLD_VALUE = preGate.STATUS,
                        TTU_ID = idUsuario
                    });

                byPassAModificar.REASON = byPass.REASON;
                byPassAModificar.IS_ENABLED = byPass.IS_ENABLED;
                _repositorioPass.Actualizar(byPassAModificar);
            }
            else
            {
                byPass.BY_PASS_AUDITS = new List<BY_PASS_AUDIT>
                {
                    new BY_PASS_AUDIT
                    {
                        DATE = DateTime.Now,
                        FIELD = "REASON",
                        NEW_VALUE = byPass.REASON,
                        OLD_VALUE = "",
                        TTU_ID = idUsuario
                    },
                    new BY_PASS_AUDIT
                    {
                        DATE = DateTime.Now,
                        FIELD = "PREGATE STATUS",
                        NEW_VALUE ="C",
                        OLD_VALUE = preGate.STATUS,
                        TTU_ID = idUsuario
                    }
                };
                _repositorioPass.InsertarRegistro(byPass);        
            }
        }

        public void ActualizarStatusPregate(long idPreGate, string Status)
        {
            using (var transaccion = new TransactionScope())
            {
                var v_preGate = _repositorioPreGate.ObtenerObjetos(new FiltroPreGatePorId(idPreGate)).FirstOrDefault();
                v_preGate.STATUS = Status;
                _repositorioPreGate.Actualizar(v_preGate);

                if (Status =="C")
                {
                    IEnumerable<STOCK_REGISTER> v_stockReg = _repositorioStockRegister.ObtenerObjetos(new FiltroStockRegisterPorPregate(idPreGate));
                    if (v_stockReg != null)
                    {
                        foreach (var item in v_stockReg)
                        {
                            STOCK_REGISTER v_registro = _repositorioStockRegister.ObtenerObjetos(new FiltroDirecto<STOCK_REGISTER>(sr => sr.ID == item.ID)).FirstOrDefault();

                            v_registro.ACTIVE = false;
                            _repositorioStockRegister.Actualizar(v_registro);
                        }
                    }
                }
                transaccion.Complete();
            }
        }

        public void ActualizarStatusStockRegister(long idPreGate, bool  Status)
        {
            IEnumerable<STOCK_REGISTER> v_stockReg = _repositorioStockRegister.ObtenerObjetos(new FiltroStockRegisterPorPregate(idPreGate));

            if (v_stockReg != null)
            {
                using (var transaccion = new TransactionScope())
                {
                   
                    foreach (var item in v_stockReg)
                    {
                        STOCK_REGISTER  v_registro =  _repositorioStockRegister.ObtenerObjetos (new  FiltroDirecto<STOCK_REGISTER>(sr => sr.ID == item.ID)).FirstOrDefault();

                        v_registro.ACTIVE = Status;
                        _repositorioStockRegister.Actualizar(v_registro);
                    }
                    transaccion.Complete();
                }
            }
        }

        public IEnumerable<MOTIVE> ObtenerMotivos(int Type)
        {
            return _repositorioMotive.ObtenerObjetos(new FiltroMotivoActivos(Type)).OrderBy(rm => rm.NAME).ToList();
        }

        public IEnumerable<SUB_MOTIVE> ObtenerSubMotivos(int idMotive)
        {
            return _repositorioSubMotive.ObtenerObjetos(new FiltroSubMotivoActivos(idMotive)).OrderBy(rm => rm.NAME).ToList();
        }

        public string ObtenerValidacionesGenerales(string _opcion  , string _StrValor, int _IntValor, long _BigintValor)
        {
            return _repositorioPreGate.ObtenerValidacionesGenerales(_opcion,  _StrValor,  _IntValor,  _BigintValor);
        }


        public void LiberarRecursos()
        {
            RepositorioTroubleTicket.LiberarRecursos();
            RepositorioZone.LiberarRecursos();
            RepositorioUserSession.LiberarRecursos();
            _repositorioReassignmentMotive.LiberarRecursos();
            _repositorioQuiosco.LiberarRecursos();
            _repositorioLiftBarrier.LiberarRecursos();
            _repositorioAction.LiberarRecursos();
            _repositorioPreGate.LiberarRecursos();
            _repositorioDevice.LiberarRecursos();
            _repositorioTransactionType.LiberarRecursos();
            _repositorioReprinter.LiberarRecursos();
            _repositorioPass.LiberarRecursos();
            _repositorioStockRegister.LiberarRecursos();
            _repositorioMotive.LiberarRecursos();
            _repositorioSubMotive.LiberarRecursos();
        }

        internal void SetearEstado(EstadoReasignacionTicket estado)
        {
            _estado = estado;
            _estado.Ejecutar(this);
        }

        internal USER_SESSION AlgoritmoSesionUsuario(List<USER_SESSION> sesionesUsuarios)
        {
            return ObtenerSesionUsuarioAlgoritmo(sesionesUsuarios);
        }
    }

    public class FiltroPreGatePorFecha : IFiltros<PRE_GATE>
    {
        private readonly string _fechas;

        public FiltroPreGatePorFecha(string fechas)
        {
            _fechas = fechas;
        }

        public Expression<Func<PRE_GATE, bool>> SastifechoPor()
        {
            var fechas = _fechas.Split(':');
            var fechaDesde = fechas[0].Split('-');
            var fechaHasta = fechas[1].Split('-');
            var fechaInicio = new DateTime(Convert.ToInt32(fechaDesde[0]), Convert.ToInt32(fechaDesde[1]), Convert.ToInt32(fechaDesde[2]));
            var fechaFinal = new DateTime(Convert.ToInt32(fechaHasta[0]), Convert.ToInt32(fechaHasta[1]), Convert.ToInt32(fechaHasta[2]), 23, 59, 59);
            var filtro = new FiltroDirecto<PRE_GATE>(pg => pg.CREATION_DATE >= fechaInicio && pg.CREATION_DATE <= fechaFinal);
            return filtro.SastifechoPor();
        }
    }

    public class FiltroTicketProcesoNoAsignados : IFiltros<PROCESS_TROUBLE_TICKET>
    {
        public Expression<Func<PROCESS_TROUBLE_TICKET, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<PROCESS_TROUBLE_TICKET>(ptt => !ptt.IS_CANCEL && !ptt.ASSIGNMENT_DATE.HasValue);
            return filtro.SastifechoPor();
        }
    }

    public class FiltroTicketMobileNoAsignados : IFiltros<MOBILE_TROUBLE_TICKET>
    {
        public Expression<Func<MOBILE_TROUBLE_TICKET, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<MOBILE_TROUBLE_TICKET>(mtt => !mtt.IS_CANCEL && !mtt.ASSIGNMENT_DATE.HasValue);
            return filtro.SastifechoPor();
        }
    }

    public class FiltroTicketTecnicoNoAsignados : IFiltros<CLIENT_APP_TRANSACTION_TROUBLE_TICKET>
    {
        public Expression<Func<CLIENT_APP_TRANSACTION_TROUBLE_TICKET, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<CLIENT_APP_TRANSACTION_TROUBLE_TICKET>(ttt => !ttt.IS_CANCEL && !ttt.ASSIGNMENT_DATE.HasValue);
            return filtro.SastifechoPor();
        }
    }

    public class FiltroTicketsSuspendidos : IFiltros<PROCESS_TROUBLE_TICKET>
    {
        public Expression<Func<PROCESS_TROUBLE_TICKET, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<PROCESS_TROUBLE_TICKET>(ptt => /*ptt.CREATION_DATE >= DateTime.Now.AddDays(-1) &&*/ !ptt.IS_CANCEL && ptt.IS_SUSPENDED  && ptt.FINISH_DATE == null);
            //var filtro = new FiltroDirecto<PROCESS_TROUBLE_TICKET>(ptt => (!ptt.IS_CANCEL && ptt.IS_SUSPENDED) || (ptt.ACCEPTANCE_DATE == null && ptt.IS_CANCEL == false && (ptt.PROCESS.STEP != "HUELLA" && ptt.PROCESS.STEP != "TECNICO" && ptt.PROCESS.STEP != "RFID") && ptt.USER_SESSION != null));
            return filtro.SastifechoPor();
        }
    }

    public class FiltroSesionUsuarioAbiertasParaCerrarlas : IFiltros<USER_SESSION>
    {
        public Expression<Func<USER_SESSION, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<USER_SESSION>(us => !us.FINISH_SESSION_DATE.HasValue && us.TROUBLE_DESK_USER.IS_TD);
            return filtro.SastifechoPor();
        }
    }

    internal abstract class EstadoReasignacionTicket
    {
        internal abstract void Ejecutar(ServicioSupervisor servicio);
    }

    internal class EstadoObtenerSesionesUsuariosPorZonas : EstadoReasignacionTicket
    {
        internal override void Ejecutar(ServicioSupervisor servicio)
        {
            var zonas = servicio.RepositorioZone.ObtenerObjetos(new FiltroZona());
            servicio.SesionesUsuarios = new List<Tuple<short, List<USER_SESSION>>>();
            foreach (var zona in zonas)
            {
                var sesionesUsuarios = servicio.RepositorioUserSession.ObtenerSesionUsuarioConDispositivo(new FiltroSesionUsuarioAbiertas(zona.ZONE_ID)).GroupBy(us => us.DEVICE_ID).Select(us => us.FirstOrDefault()).ToList();
                servicio.SesionesUsuarios.Add(new Tuple<short, List<USER_SESSION>>(zona.ZONE_ID, sesionesUsuarios));
            }
            if (servicio.SesionesUsuarios.Count() != 0 && servicio.SesionesUsuarios.Any(su => su.Item2.Count() > 0))
            {
                if(servicio.EsReasignacionSuspendidos)
                    servicio.SetearEstado(new EstadoReasignarSuspendidos());
                else
                    servicio.SetearEstado(new EstadoReasignar());
            }
        }
    }

    public class FiltroZona : IFiltros<ZONE>
    {
        public Expression<Func<ZONE, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<ZONE>(z => z.IS_ACTIVE);
            return filtro.SastifechoPor();
        }
    }

    internal class EstadoReasignarSuspendidos : EstadoReasignacionTicket
    {
        internal override void Ejecutar(ServicioSupervisor servicio)
        {
            foreach (var ticket in servicio.TicketsSuspendidos)
            {
                var sesionesUsuarios = servicio.SesionesUsuarios.FirstOrDefault(su => su.Item1 == ticket.Value.Item2);
                if (sesionesUsuarios == null || sesionesUsuarios.Item2.Count(su => su.ID != ticket.Value.Item1) == 0)
                    servicio.Resultado = 1;
                else
                {
                    var sesionUsuario = servicio.AlgoritmoSesionUsuario(sesionesUsuarios.Item2.Where(su => su.ID != ticket.Value.Item1).ToList());
                    servicio.RepositorioTroubleTicket.ReasignarTicketProceso(ticket.Key, sesionUsuario.ID, servicio.Reasignacion);
                }
            }
            if (servicio.Resultado == 0)
                servicio.Resultado = 2;
        }
    }

    internal class EstadoReasignar : EstadoReasignacionTicket
    {
        internal override void Ejecutar(ServicioSupervisor servicio)
        {
            foreach (var ticket in servicio.Tickets)
            {
                var cadenaResponsabilidad = new ReasignarTicketProceso();
                cadenaResponsabilidad.Procesar(servicio, ticket);
            }
            if (servicio.Resultado == 0)
                servicio.Resultado = 2;
        }
    }

    internal abstract class CadenaResponsabilidad
    {
        protected CadenaResponsabilidad Siguiente { get; set; }

        protected void EstablecerSiguiente(CadenaResponsabilidad siguiente)
        {
            Siguiente = siguiente;
        }

        internal abstract void Procesar(ServicioSupervisor servicio, KeyValuePair<long, Tuple<TipoTicket, short>> ticket);
    }

    internal class ReasignarTicketProceso : CadenaResponsabilidad
    {
        internal override void Procesar(ServicioSupervisor servicio, KeyValuePair<long, Tuple<TipoTicket, short>> ticket)
        {
            if (ticket.Value.Item1 == TipoTicket.Proceso)
            {
                var sesionesUsuarios = servicio.SesionesUsuarios.FirstOrDefault(su => su.Item1 == ticket.Value.Item2);
                if (sesionesUsuarios == null || sesionesUsuarios.Item2.Count() == 0)
                    servicio.Resultado = 1;
                else
                {
                    var sesionUsuario = servicio.AlgoritmoSesionUsuario(sesionesUsuarios.Item2);
                    servicio.RepositorioTroubleTicket.AgregarSesionUsuarioATicketProceso(ticket.Key, sesionUsuario.ID);
                }
            }
            else
            {
                EstablecerSiguiente(new ReasignarTicketMobile());
                Siguiente.Procesar(servicio, ticket);
            }
        }
    }

    internal class ReasignarTicketMobile : CadenaResponsabilidad
    {
        internal override void Procesar(ServicioSupervisor servicio, KeyValuePair<long, Tuple<TipoTicket, short>> ticket)
        {
            if (ticket.Value.Item1 == TipoTicket.Mobile)
            {
                var sesionesUsuarios = servicio.SesionesUsuarios.FirstOrDefault(su => su.Item1 == ticket.Value.Item2);
                if (sesionesUsuarios == null || sesionesUsuarios.Item2.Count() == 0)
                    servicio.Resultado = 1;
                else
                {
                    var sesionUsuario = servicio.AlgoritmoSesionUsuario(sesionesUsuarios.Item2);
                    servicio.RepositorioTroubleTicket.AgregarSesionUsuarioATicketMobile(ticket.Key, sesionUsuario.ID);
                }
            }
            else
            {
                EstablecerSiguiente(new ReasignarTicketTecnico());
                Siguiente.Procesar(servicio, ticket);
            }
        }
    }

    internal class ReasignarTicketTecnico : CadenaResponsabilidad
    {
        internal override void Procesar(ServicioSupervisor servicio, KeyValuePair<long, Tuple<TipoTicket, short>> ticket)
        {
            if (ticket.Value.Item1 == TipoTicket.Tecnico)
            {
                var sesionesUsuarios = servicio.SesionesUsuarios.FirstOrDefault(su => su.Item1 == ticket.Value.Item2);
                if (sesionesUsuarios == null || sesionesUsuarios.Item2.Count() == 0)
                    servicio.Resultado = 1;
                else
                {
                    var sesionUsuario = servicio.AlgoritmoSesionUsuario(sesionesUsuarios.Item2);
                    servicio.RepositorioTroubleTicket.AgregarSesionUsuarioATicketTecnico(ticket.Key, sesionUsuario.ID);
                }
            }
        }
    }

    public class FiltroTicketProcesoPorIdYFecha : IFiltros<PROCESS_TROUBLE_TICKET>
    {
        private readonly string _id;
        private readonly string _fechas;

        public FiltroTicketProcesoPorIdYFecha(string id, string fechas)
        {
            _id = id;
            _fechas = fechas;
        }

        public Expression<Func<PROCESS_TROUBLE_TICKET, bool>> SastifechoPor()
        {
            var fechas = _fechas.Split(':');
            var fechaDesde = fechas[0].Split('-');
            var fechaHasta = fechas[1].Split('-');
            var fechaInicio = new DateTime(Convert.ToInt32(fechaDesde[0]), Convert.ToInt32(fechaDesde[1]), Convert.ToInt32(fechaDesde[2]));
            var fechaFinal = new DateTime(Convert.ToInt32(fechaHasta[0]), Convert.ToInt32(fechaHasta[1]), Convert.ToInt32(fechaHasta[2]), 23, 59, 59);
            var idTicket = Convert.ToInt64(_id);
            var filtro = new FiltroDirecto<PROCESS_TROUBLE_TICKET>(ptt => ptt.CREATION_DATE >= fechaInicio && ptt.CREATION_DATE <= fechaFinal && ptt.TT_ID == idTicket);
            return filtro.SastifechoPor();
        }
    }

    public class FiltroTicketProcesoPorFecha : IFiltros<PROCESS_TROUBLE_TICKET>
    {
        private readonly string _fechas;

        public FiltroTicketProcesoPorFecha(string fechas)
        {
            _fechas = fechas;
        }

        public Expression<Func<PROCESS_TROUBLE_TICKET, bool>> SastifechoPor()
        {
            var fechas = _fechas.Split(':');
            var fechaDesde = fechas[0].Split('-');
            var fechaHasta = fechas[1].Split('-');
            var fechaInicio = new DateTime(Convert.ToInt32(fechaDesde[0]), Convert.ToInt32(fechaDesde[1]), Convert.ToInt32(fechaDesde[2]));
            var fechaFinal = new DateTime(Convert.ToInt32(fechaHasta[0]), Convert.ToInt32(fechaHasta[1]), Convert.ToInt32(fechaHasta[2]), 23, 59, 59);
            var filtro = new FiltroDirecto<PROCESS_TROUBLE_TICKET>(ptt => ptt.CREATION_DATE >= fechaInicio && ptt.CREATION_DATE <= fechaFinal);
            return filtro.SastifechoPor();
        }
    }

    public class FiltroTicketMobilePorIdYFecha : IFiltros<MOBILE_TROUBLE_TICKET>
    {
        private readonly string _id;
        private readonly string _fechas;

        public FiltroTicketMobilePorIdYFecha(string id, string fechas)
        {
            _id = id;
            _fechas = fechas;
        }

        public Expression<Func<MOBILE_TROUBLE_TICKET, bool>> SastifechoPor()
        {
            var fechas = _fechas.Split(':');
            var fechaDesde = fechas[0].Split('-');
            var fechaHasta = fechas[1].Split('-');
            var fechaInicio = new DateTime(Convert.ToInt32(fechaDesde[0]), Convert.ToInt32(fechaDesde[1]), Convert.ToInt32(fechaDesde[2]));
            var fechaFinal = new DateTime(Convert.ToInt32(fechaHasta[0]), Convert.ToInt32(fechaHasta[1]), Convert.ToInt32(fechaHasta[2]), 23, 59, 59);
            var idTicket = Convert.ToInt64(_id);
            var filtro = new FiltroDirecto<MOBILE_TROUBLE_TICKET>(mtt => mtt.CREATION_DATE >= fechaInicio && mtt.CREATION_DATE <= fechaFinal && mtt.TT_ID == idTicket);
            return filtro.SastifechoPor();
        }
    }

    public class FiltroTicketMobilePorFecha : IFiltros<MOBILE_TROUBLE_TICKET>
    {
        private readonly string _fechas;

        public FiltroTicketMobilePorFecha(string fechas)
        {
            _fechas = fechas;
        }

        public Expression<Func<MOBILE_TROUBLE_TICKET, bool>> SastifechoPor()
        {
            var fechas = _fechas.Split(':');
            var fechaDesde = fechas[0].Split('-');
            var fechaHasta = fechas[1].Split('-');
            var fechaInicio = new DateTime(Convert.ToInt32(fechaDesde[0]), Convert.ToInt32(fechaDesde[1]), Convert.ToInt32(fechaDesde[2]));
            var fechaFinal = new DateTime(Convert.ToInt32(fechaHasta[0]), Convert.ToInt32(fechaHasta[1]), Convert.ToInt32(fechaHasta[2]), 23, 59, 59);
            var filtro = new FiltroDirecto<MOBILE_TROUBLE_TICKET>(mtt => mtt.CREATION_DATE >= fechaInicio && mtt.CREATION_DATE <= fechaFinal);
            return filtro.SastifechoPor();
        }
    }

    public class FiltroTicketTecnicoPorIdYFecha : IFiltros<CLIENT_APP_TRANSACTION_TROUBLE_TICKET>
    {
        private readonly string _id;
        private readonly string _fechas;

        public FiltroTicketTecnicoPorIdYFecha(string id, string fechas)
        {
            _id = id;
            _fechas = fechas;
        }

        public Expression<Func<CLIENT_APP_TRANSACTION_TROUBLE_TICKET, bool>> SastifechoPor()
        {
            var fechas = _fechas.Split(':');
            var fechaDesde = fechas[0].Split('-');
            var fechaHasta = fechas[1].Split('-');
            var fechaInicio = new DateTime(Convert.ToInt32(fechaDesde[0]), Convert.ToInt32(fechaDesde[1]), Convert.ToInt32(fechaDesde[2]));
            var fechaFinal = new DateTime(Convert.ToInt32(fechaHasta[0]), Convert.ToInt32(fechaHasta[1]), Convert.ToInt32(fechaHasta[2]), 23, 59, 59);
            var idTicket = Convert.ToInt64(_id);
            var filtro = new FiltroDirecto<CLIENT_APP_TRANSACTION_TROUBLE_TICKET>(ttt => ttt.CREATION_DATE >= fechaInicio && ttt.CREATION_DATE <= fechaFinal && ttt.TT_ID == idTicket);
            return filtro.SastifechoPor();
        }
    }

    public class FiltroTicketTecnicoPorFecha : IFiltros<CLIENT_APP_TRANSACTION_TROUBLE_TICKET>
    {
        private readonly string _fechas;

        public FiltroTicketTecnicoPorFecha(string fechas)
        {
            _fechas = fechas;
        }

        public Expression<Func<CLIENT_APP_TRANSACTION_TROUBLE_TICKET, bool>> SastifechoPor()
        {
            var fechas = _fechas.Split(':');
            var fechaDesde = fechas[0].Split('-');
            var fechaHasta = fechas[1].Split('-');
            var fechaInicio = new DateTime(Convert.ToInt32(fechaDesde[0]), Convert.ToInt32(fechaDesde[1]), Convert.ToInt32(fechaDesde[2]));
            var fechaFinal = new DateTime(Convert.ToInt32(fechaHasta[0]), Convert.ToInt32(fechaHasta[1]), Convert.ToInt32(fechaHasta[2]), 23, 59, 59);
            var filtro = new FiltroDirecto<CLIENT_APP_TRANSACTION_TROUBLE_TICKET>(ttt => ttt.CREATION_DATE >= fechaInicio && ttt.CREATION_DATE <= fechaFinal);
            return filtro.SastifechoPor();
        }
    }

    public class FiltroTicketAutoPorIdYFecha : IFiltros<AUTO_TROUBLE_TICKET>
    {
        private readonly string _id;
        private readonly string _fechas;

        public FiltroTicketAutoPorIdYFecha(string id, string fechas)
        {
            _id = id;
            _fechas = fechas;
        }

        public Expression<Func<AUTO_TROUBLE_TICKET, bool>> SastifechoPor()
        {
            var fechas = _fechas.Split(':');
            var fechaDesde = fechas[0].Split('-');
            var fechaHasta = fechas[1].Split('-');
            var fechaInicio = new DateTime(Convert.ToInt32(fechaDesde[0]), Convert.ToInt32(fechaDesde[1]), Convert.ToInt32(fechaDesde[2]));
            var fechaFinal = new DateTime(Convert.ToInt32(fechaHasta[0]), Convert.ToInt32(fechaHasta[1]), Convert.ToInt32(fechaHasta[2]), 23, 59, 59);
            var idTicket = Convert.ToInt64(_id);
            var filtro = new FiltroDirecto<AUTO_TROUBLE_TICKET>(att => att.CREATION_DATE >= fechaInicio && att.CREATION_DATE <= fechaFinal && att.TT_ID == idTicket);
            return filtro.SastifechoPor();
        }
    }

    public class FiltroTicketAutoPorFecha : IFiltros<AUTO_TROUBLE_TICKET>
    {
        private readonly string _fechas;

        public FiltroTicketAutoPorFecha(string fechas)
        {
            _fechas = fechas;
        }

        public Expression<Func<AUTO_TROUBLE_TICKET, bool>> SastifechoPor()
        {
            var fechas = _fechas.Split(':');
            var fechaDesde = fechas[0].Split('-');
            var fechaHasta = fechas[1].Split('-');
            var fechaInicio = new DateTime(Convert.ToInt32(fechaDesde[0]), Convert.ToInt32(fechaDesde[1]), Convert.ToInt32(fechaDesde[2]));
            var fechaFinal = new DateTime(Convert.ToInt32(fechaHasta[0]), Convert.ToInt32(fechaHasta[1]), Convert.ToInt32(fechaHasta[2]), 23, 59, 59);
            var filtro = new FiltroDirecto<AUTO_TROUBLE_TICKET>(att => att.CREATION_DATE >= fechaInicio && att.CREATION_DATE <= fechaFinal);
            return filtro.SastifechoPor();
        }
    }

    internal abstract class ManejadorBusqueda
    {
        protected ManejadorBusqueda Siguiente;

        protected ManejadorBusqueda EstablecerSiguiente(ManejadorBusqueda siguiente)
        {
            Siguiente = siguiente;
            return Siguiente;
        }

        internal abstract void ManejarFiltros(KeyValuePair<BusquedaTicketReporte, string> item, ref IEnumerable<TicketReporte> datos);
    }

    internal class ManejadorBusquedaPorContenedor : ManejadorBusqueda
    {
        internal override void ManejarFiltros(KeyValuePair<BusquedaTicketReporte, string> item, ref IEnumerable<TicketReporte> datos)
        {
            if (item.Key == BusquedaTicketReporte.Contenedor)
                datos = datos.Where(d => d.Contenedores.Contains(item.Value));
            else
            {
                Siguiente = new ManejadorBusquedaPorPlaca();
                Siguiente.ManejarFiltros(item, ref datos);
            }
        }
    }

    internal class ManejadorBusquedaPorPlaca : ManejadorBusqueda
    {
        internal override void ManejarFiltros(KeyValuePair<BusquedaTicketReporte, string> item, ref IEnumerable<TicketReporte> datos)
        {
            if (item.Key == BusquedaTicketReporte.Placa)
                datos = datos.Where(d => d.PlacaVehiculo == item.Value);
            else
            {
                Siguiente = new ManejadorBusquedaPorCedulaChofer();
                Siguiente.ManejarFiltros(item, ref datos);
            }
        }
    }

    internal class ManejadorBusquedaPorCedulaChofer : ManejadorBusqueda
    {
        internal override void ManejarFiltros(KeyValuePair<BusquedaTicketReporte, string> item, ref IEnumerable<TicketReporte> datos)
        {
            if (item.Key == BusquedaTicketReporte.CedulaChofer)
                datos = datos.Where(d => d.CedulaChofer == item.Value);
            else
            {
                Siguiente = new ManejadorBusquedaPorKiosco();
                Siguiente.ManejarFiltros(item, ref datos);
            }
        }
    }

    internal class ManejadorBusquedaPorKiosco : ManejadorBusqueda
    {
        internal override void ManejarFiltros(KeyValuePair<BusquedaTicketReporte, string> item, ref IEnumerable<TicketReporte> datos)
        {
            if (item.Key == BusquedaTicketReporte.IdKiosco)
            {
                var id = Convert.ToInt16(item.Value);
                datos = datos.Where(d => d.IdQuiosco == id);
            }
            else
            {
                Siguiente = new ManejadorBusquedaPorEstado();
                Siguiente.ManejarFiltros(item, ref datos);
            }
        }
    }

    internal class ManejadorBusquedaPorEstado : ManejadorBusqueda
    {
        internal override void ManejarFiltros(KeyValuePair<BusquedaTicketReporte, string> item, ref IEnumerable<TicketReporte> datos)
        {
            if (item.Key == BusquedaTicketReporte.Estado && item.Value != "T")
            {
                if (item.Value == "NT")
                    datos = datos.Where(d => !d.FechaAceptacion.HasValue);
                else if (item.Value == "ER")
                    datos = datos.Where(d => d.FechaAceptacion.HasValue && !d.FechaFinalizacion.HasValue);
                else
                    datos = datos.Where(d => d.FechaFinalizacion.HasValue);
            }
            else
            {
                Siguiente = new ManejadorBusquedaPorUsuario();
                Siguiente.ManejarFiltros(item, ref datos);
            }
        }
    }

    internal class ManejadorBusquedaPorUsuario : ManejadorBusqueda
    {
        internal override void ManejarFiltros(KeyValuePair<BusquedaTicketReporte, string> item, ref IEnumerable<TicketReporte> datos)
        {
            if (item.Key == BusquedaTicketReporte.Usuario)
                datos = datos.Where(d => d.UserName.ToLower() == item.Value);
        }
    }

    public class FiltroAccionPorIdTicket : IFiltros<ACTION>
    {
        private long _idTicket;

        public FiltroAccionPorIdTicket(long idTicket)
        {
            _idTicket = idTicket;
        }

        public Expression<Func<ACTION, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<ACTION>(a => a.TT_ID == _idTicket);
            return filtro.SastifechoPor();
        }
    }

    public class FiltroMotivoReasignacionActivos : IFiltros<REASSIGNMENT_MOTIVE>
    {
        public Expression<Func<REASSIGNMENT_MOTIVE, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<REASSIGNMENT_MOTIVE>(rm => rm.IS_ACTIVE);
            return filtro.SastifechoPor();
        }
    }

    public class FiltroQuioscoActivos : IFiltros<KIOSK>
    {
        public Expression<Func<KIOSK, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<KIOSK>(k => k.IS_ACTIVE);
            return filtro.SastifechoPor();
        }
    }

    internal abstract class ManejadorBusquedaReporteTransacciones
    {
        protected ManejadorBusquedaReporteTransacciones Siguiente;

        protected ManejadorBusquedaReporteTransacciones EstablecerSiguiente(ManejadorBusquedaReporteTransacciones siguiente)
        {
            Siguiente = siguiente;
            return Siguiente;
        }

        internal abstract void ManejarFiltros(KeyValuePair<BusquedaReporteTransacciones, string> item, ref IEnumerable<PRE_GATE> datos);
    }

    internal class ManejadorBusquedaPorGosTv : ManejadorBusquedaReporteTransacciones
    {
        internal override void ManejarFiltros(KeyValuePair<BusquedaReporteTransacciones, string> item, ref IEnumerable<PRE_GATE> datos)
        {
            if (item.Key == BusquedaReporteTransacciones.GosTv)
            {
                var id = Convert.ToInt64(item.Value);
                datos = datos.Where(d => d.PRE_GATE_ID == id);
            }
            else
            {
                Siguiente = new ManejadorBusquedaPorUsuarioRt();
                Siguiente.ManejarFiltros(item, ref datos);
            }
        }
    }

    internal class ManejadorBusquedaPorUsuarioRt : ManejadorBusquedaReporteTransacciones
    {
        internal override void ManejarFiltros(KeyValuePair<BusquedaReporteTransacciones, string> item, ref IEnumerable<PRE_GATE> datos)
        {
            if (item.Key == BusquedaReporteTransacciones.Usuario)
                datos = datos.Where(d => d.USER.ToLower() == item.Value);
            else
            {
                Siguiente = new ManejadorBusquedaPorCedulaChoferRt();
                Siguiente.ManejarFiltros(item, ref datos);
            }
        }
    }

    internal class ManejadorBusquedaPorCedulaChoferRt : ManejadorBusquedaReporteTransacciones
    {
        internal override void ManejarFiltros(KeyValuePair<BusquedaReporteTransacciones, string> item, ref IEnumerable<PRE_GATE> datos)
        {
            if (item.Key == BusquedaReporteTransacciones.CedulaChofer)
                datos = datos.Where(d => d.DRIVER_ID == item.Value);
            else
            {
                Siguiente = new ManejadorBusquedaPorPlacaRt();
                Siguiente.ManejarFiltros(item, ref datos);
            }
        }
    }

    internal class ManejadorBusquedaPorPlacaRt : ManejadorBusquedaReporteTransacciones
    {
        internal override void ManejarFiltros(KeyValuePair<BusquedaReporteTransacciones, string> item, ref IEnumerable<PRE_GATE> datos)
        {
            if (item.Key == BusquedaReporteTransacciones.Placa)
                datos = datos.Where(d => d.TRUCK_LICENCE.ToUpper() == item.Value);
            else
            {
                Siguiente = new ManejadorBusquedaPorIdDispositivo();
                Siguiente.ManejarFiltros(item, ref datos);
            }
        }
    }

    internal class ManejadorBusquedaPorIdDispositivo : ManejadorBusquedaReporteTransacciones
    {
        internal override void ManejarFiltros(KeyValuePair<BusquedaReporteTransacciones, string> item, ref IEnumerable<PRE_GATE> datos)
        {
            if (item.Key == BusquedaReporteTransacciones.IdDispositivo)
            {
                var id = Convert.ToInt32(item.Value);
                datos = datos.Where(d => d.DEVICE_ID == id);
            }
            else
            {
                Siguiente = new ManejadorBusquedaPorEstadoRt();
                Siguiente.ManejarFiltros(item, ref datos);
            }
        }
    }

    internal class ManejadorBusquedaPorEstadoRt : ManejadorBusquedaReporteTransacciones
    {
        internal override void ManejarFiltros(KeyValuePair<BusquedaReporteTransacciones, string> item, ref IEnumerable<PRE_GATE> datos)
        {
            if (item.Key == BusquedaReporteTransacciones.Estado)
                datos = datos.Where(d => d.STATUS == item.Value);
            else
            {
                Siguiente = new ManejadorBusquedaPorIdTipoTransaccion();
                Siguiente.ManejarFiltros(item, ref datos);
            }
        }
    }

    internal class ManejadorBusquedaPorIdTipoTransaccion : ManejadorBusquedaReporteTransacciones
    {
        internal override void ManejarFiltros(KeyValuePair<BusquedaReporteTransacciones, string> item, ref IEnumerable<PRE_GATE> datos)
        {
            if (item.Key == BusquedaReporteTransacciones.IdTipoTransaccion)
            {
                var id = Convert.ToInt16(item.Value);
                datos = datos.Where(d => d.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == id));
            }
            else
            {
                Siguiente = new ManejadorBusquedaPorIdKiosco();
                Siguiente.ManejarFiltros(item, ref datos);
            }
        }
    }

    internal class ManejadorBusquedaPorIdKiosco : ManejadorBusquedaReporteTransacciones
    {
        internal override void ManejarFiltros(KeyValuePair<BusquedaReporteTransacciones, string> item, ref IEnumerable<PRE_GATE> datos)
        {
            if (item.Key == BusquedaReporteTransacciones.IdKiosco)
            {
                var id = Convert.ToInt16(item.Value);
                datos = datos.Where(d => d.KIOSK_TRANSACTIONS.Any(kt => kt.KIOSK_ID == id));
            }
            else
            {
                Siguiente = new ManejadorBusquedaPorIdZona();
                Siguiente.ManejarFiltros(item, ref datos);
            }
        }
    }

    internal class ManejadorBusquedaPorIdZona : ManejadorBusquedaReporteTransacciones
    {
        internal override void ManejarFiltros(KeyValuePair<BusquedaReporteTransacciones, string> item, ref IEnumerable<PRE_GATE> datos)
        {
            if (item.Key == BusquedaReporteTransacciones.IdZona)
            {
                var id = Convert.ToInt16(item.Value);
                datos = datos.Where(d => d.DEVICE.ZONE_ID == id);
            }
        }
    }

    public class FiltroZonasActivas : IFiltros<ZONE>
    {
        public Expression<Func<ZONE, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<ZONE>(z => z.IS_ACTIVE);
            return filtro.SastifechoPor();
        }
    }

    public class FiltroDispositivoTablets : IFiltros<DEVICE>
    {
        public Expression<Func<DEVICE, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<DEVICE>(d => d.IS_ACTIVE);
            return filtro.SastifechoPor();
        }
    }

    public class FiltroTipoTransaccion : IFiltros<TRANSACTION_TYPE>
    {
        public Expression<Func<TRANSACTION_TYPE, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<TRANSACTION_TYPE>(tt => tt.ID > 0);
            return filtro.SastifechoPor();
        }
    }

    public class FiltroPreGatePorId : IFiltros<PRE_GATE>
    {
        private readonly long _idPreGate;

        public FiltroPreGatePorId(long idPreGate)
        {
            _idPreGate = idPreGate;
        }

        public Expression<Func<PRE_GATE, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<PRE_GATE>(pg => pg.PRE_GATE_ID == _idPreGate);
            return filtro.SastifechoPor();
        }
    }

    public class FiltroByPassPorId : IFiltros<BY_PASS>
    {
        private readonly int _id;

        public FiltroByPassPorId(int id)
        {
            _id = id;
        }

        public Expression<Func<BY_PASS, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<BY_PASS>(bp => bp.ID == _id);
            return filtro.SastifechoPor();
        }
    }

    public class FiltroStockRegisterPorPregate : IFiltros<STOCK_REGISTER>
    {
        private readonly long _id;

        public FiltroStockRegisterPorPregate(long id)
        {
            _id = id;
        }

        public Expression<Func<STOCK_REGISTER, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<STOCK_REGISTER>(bp => bp.PRE_GATE_ID  == _id);
            return filtro.SastifechoPor();
        }
    }

    public class FiltroMotivoActivos : IFiltros<MOTIVE>
    {
        private readonly int _type;

        public FiltroMotivoActivos(int type)
        {
            _type = type;
        }

        public Expression<Func<MOTIVE, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<MOTIVE>(rm => rm.ACTIVE && rm.TYPE == _type);
            return filtro.SastifechoPor();
        }
    }

    public class FiltroSubMotivoActivos : IFiltros<SUB_MOTIVE>
    {
        private readonly int _idMotivo;

        public FiltroSubMotivoActivos(int idMotivo)
        {
            _idMotivo = idMotivo;
        }

        public Expression<Func<SUB_MOTIVE, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<SUB_MOTIVE>(rm => rm.ACTIVE && rm.MOTIVES_ID  == _idMotivo);
            return filtro.SastifechoPor();
        }
    }
}

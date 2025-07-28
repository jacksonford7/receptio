using RECEPTIO.CapaAplicacion.Console.Aplicacion.Contratos;
using RECEPTIO.CapaAplicacion.Console.Aplicacion.Interfaces;
using RECEPTIO.CapaDominio.Console.Dominio.InterfacesRepositorios;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Filtros;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace RECEPTIO.CapaDominio.Console.ServiciosDominio
{
    public class ServicioTroubleDesk : ITroubleDesk
    {
        private readonly IRepositorioUserSession _repositorioUserSession;
        private readonly IRepositorioKioskTransaction _repositorioTransaccionQuiosco;
        private readonly IRepositorioTroubleTicket _repositorioTroubleTicket;
        private readonly IRepositorioAutoTroubleReason _repositorioAutoTroubleReason;
        private readonly IRepositorioAction _repositorioAction;
        private readonly IRepositorioAduana _repositorioAduana;
        private readonly IRepositorioBreakType _repositorioBreakType;
        private readonly IRepositorioBreak _repositorioBreak;

        public ServicioTroubleDesk(IRepositorioUserSession repositorioUserSession, IRepositorioKioskTransaction repositorioTransaccionQuiosco, IRepositorioTroubleTicket repositorioTroubleTicket, IRepositorioAutoTroubleReason repositorioAutoTroubleReason, IRepositorioAction repositorioAction, IRepositorioAduana repositorioAduana, IRepositorioBreakType repositorioBreakType, IRepositorioBreak repositorioBreak)
        {
            _repositorioUserSession = repositorioUserSession;
            _repositorioTransaccionQuiosco = repositorioTransaccionQuiosco;
            _repositorioTroubleTicket = repositorioTroubleTicket;
            _repositorioAutoTroubleReason = repositorioAutoTroubleReason;
            _repositorioAction = repositorioAction;
            _repositorioAduana = repositorioAduana;
            _repositorioBreakType = repositorioBreakType;
            _repositorioBreak = repositorioBreak;
        }

        public IEnumerable<Ticket> ObtenerTickets(long idSesionUsuario)
        {
            var sesionUsuario = _repositorioUserSession.ObtenerSesionUsuarioConTickets(new FiltroSesionUsuarioConTickets(idSesionUsuario)).FirstOrDefault();
            if (sesionUsuario == null)
                throw new ApplicationException($"No existe sesión de usuario con id : {idSesionUsuario}");
            return ObtenerTicketsDeSesionUsuario(sesionUsuario).OrderByDescending(t => t.IdTicket);
        }

        private IEnumerable<Ticket> ObtenerTicketsDeSesionUsuario(USER_SESSION sesionUsuario)
        {
            var tickets = new List<Ticket>();
            AgregarItemsProceso(tickets, sesionUsuario);
            AgregarItemsAuto(tickets, sesionUsuario);
            AgregarItemsMobile(tickets, sesionUsuario);
            AgregarItemsTecnico(tickets, sesionUsuario);
            return tickets;
        }

        private void AgregarItemsProceso(List<Ticket> tickets, USER_SESSION sesionUsuario)
        {
            foreach (var item in sesionUsuario.PROCESS_TROUBLE_TICKETS.Where(ptt => !ptt.IS_SUSPENDED))
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
                    FechaAceptacion = item.ACCEPTANCE_DATE,
                    FechaAsignacion = item.ASSIGNMENT_DATE.Value,
                    FechaFinalizacion = item.FINISH_DATE,
                    FechaCreacion = item.CREATION_DATE,
                    EstaSuspendido = item.IS_SUSPENDED,
                   /*IdMotive = item.MOTIVE_ID,
                    IdSubMotive = item.SUBMOTIVE_ID*/
                });
        }

        private void AgregarItemsAuto(List<Ticket> tickets, USER_SESSION sesionUsuario)
        {
            foreach (var item in sesionUsuario.AUTO_TROUBLE_TICKETS)
                tickets.Add(new Ticket
                {
                    IdTicket = item.TT_ID,
                    Mensaje = item.AUTO_TROUBLE_REASON.CAPTION,
                    Notas = item.NOTES,
                    Tipo = TipoTicket.Auto,
                    FechaAceptacion = item.ACCEPTANCE_DATE,
                    FechaAsignacion = item.ASSIGNMENT_DATE.Value,
                    FechaFinalizacion = item.FINISH_DATE,
                    FechaCreacion = item.CREATION_DATE,
                    /*IdMotive = item.MOTIVE_ID,
                    IdSubMotive = item.SUBMOTIVE_ID*/
                });
        }

        private void AgregarItemsMobile(List<Ticket> tickets, USER_SESSION sesionUsuario)
        {
            foreach (var item in sesionUsuario.MOBILE_TROUBLE_TICKETS)
                tickets.Add(new Ticket
                {
                    IdTicket = item.TT_ID,
                    Mensaje = item.MESSAGE,
                    Tipo = TipoTicket.Mobile,
                    Notas = item.NOTES,
                    FechaAceptacion = item.ACCEPTANCE_DATE,
                    FechaAsignacion = item.ASSIGNMENT_DATE.Value,
                    FechaFinalizacion = item.FINISH_DATE,
                    FechaCreacion = item.CREATION_DATE,
                    IdZona = item.ZONE_ID,
                    /*IdMotive = item.MOTIVE_ID,
                    IdSubMotive = item.SUBMOTIVE_ID*/
                });
        }

        private void AgregarItemsTecnico(List<Ticket> tickets, USER_SESSION sesionUsuario)
        {
            foreach (var item in sesionUsuario.CLIENT_APP_TRANSACTION_TROUBLE_TICKETS)
                tickets.Add(new Ticket
                {
                    IdTicket = item.TT_ID,
                    Mensaje = item.ERROR == null ? "Ha ocurrido un inconveniente.Contactarse con el Departamento de IT." : item.ERROR.MESSAGE,
                    Tipo = TipoTicket.Tecnico,
                    Notas = item.NOTES,
                    FechaAceptacion = item.ACCEPTANCE_DATE,
                    FechaAsignacion = item.ASSIGNMENT_DATE.Value,
                    FechaFinalizacion = item.FINISH_DATE,
                    FechaCreacion = item.CREATION_DATE,
                    /*IdMotive = item.MOTIVE_ID,
                    IdSubMotive = item.SUBMOTIVE_ID*/
                });
        }

        public DetalleTicket ObtenerDetallesTicket(int idTransaccionQuiosco)
        {
            var transaccionQuiosco = _repositorioTransaccionQuiosco.ObtenerTransacionQuioscoConProcesosYDatos(new FiltroTransaccionQuioscoPorId(idTransaccionQuiosco)).FirstOrDefault();
            if (transaccionQuiosco == null)
                throw new ApplicationException($"No existe transaccion quiosco con id : {idTransaccionQuiosco}");
            return ObtenerDetalleTicketsDeTransaccionQuiosco(transaccionQuiosco);
        }

        private DetalleTicket ObtenerDetalleTicketsDeTransaccionQuiosco(KIOSK_TRANSACTION transaccionQuiosco)
        {
            var procesos = ObtenerProcesos(transaccionQuiosco);
            var transaccionContenedores = ObtenerTransaccionContenedores(transaccionQuiosco);
            StringBuilder contenedores = new StringBuilder();
            if(transaccionQuiosco.PRE_GATE != null)
            {
                foreach (var detalle in transaccionQuiosco.PRE_GATE.PRE_GATE_DETAILS)
                    contenedores.Append(detalle.CONTAINERS == null ? "" : detalle.CONTAINERS.Aggregate("", (actual, item) => (actual == "" ? "" : actual + ",") + item.NUMBER + ", "));
            }
            return new DetalleTicket
            {
                CedulaChofer = transaccionQuiosco.PRE_GATE == null ? "" : transaccionQuiosco.PRE_GATE.DRIVER_ID,
                PlacaCamion = transaccionQuiosco.PRE_GATE == null ? "" : transaccionQuiosco.PRE_GATE.TRUCK_LICENCE,
                Procesos = procesos,
                TipoTransaccion = transaccionQuiosco.PRE_GATE == null ? "Desconocida" : transaccionQuiosco.PRE_GATE.PRE_GATE_DETAILS.FirstOrDefault().TRANSACTION_TYPE.DESCRIPTION,
                Contenedores = transaccionQuiosco.PRE_GATE == null || contenedores.ToString() == "" ? "" : contenedores.ToString().Substring(0, contenedores.Length - 3),
                Containers = transaccionQuiosco.PRE_GATE == null || contenedores.ToString() == "" ? "" : contenedores.ToString(),
                PregateID = transaccionQuiosco.PRE_GATE == null  ? 0 : transaccionQuiosco.PRE_GATE.PRE_GATE_ID,
                TransaccionContenedores = transaccionContenedores
            };
        }

        private List<Proceso> ObtenerProcesos(KIOSK_TRANSACTION transaccionQuiosco)
        {
            var procesos = new List<Proceso>();
            foreach (var item in transaccionQuiosco.PROCESSES)
                procesos.Add(new Proceso
                {
                    FechaProceso = item.STEP_DATE,
                    FueOk = item.IS_OK,
                    MensajeEspecifico = item.MESSAGE.DETAILS,
                    MensajeTecnico = item.MESSAGE.TROUBLE_DESK_MESSAGE,
                    MensajeUsuario = item.MESSAGE.USER_MESSAGE,
                    Paso = item.STEP,
                    Respuesta = item.RESPONSE
                });
            return procesos;
        }

        private List<TransaccionContenedor> ObtenerTransaccionContenedores(KIOSK_TRANSACTION transaccionQuiosco)
        {
            var items = new List<TransaccionContenedor>();
            if (transaccionQuiosco.PRE_GATE != null)
            {
                foreach (var item in transaccionQuiosco.PRE_GATE.PRE_GATE_DETAILS.Where(pgd => pgd.TRANSACTION_TYPE_ID == 1 || pgd.TRANSACTION_TYPE_ID == 2 || pgd.TRANSACTION_TYPE_ID == 6 || pgd.TRANSACTION_TYPE_ID == 11 || pgd.TRANSACTION_TYPE_ID == 17))
                    items.Add(new TransaccionContenedor
                    {
                        NumeroTransaccion = item.TRANSACTION_NUMBER.Contains("-") ? (transaccionQuiosco.PRE_GATE_ID * -1).ToString() : item.TRANSACTION_NUMBER,
                        Contenedor = item.CONTAINERS.FirstOrDefault() == null ? "" : item.CONTAINERS.FirstOrDefault().NUMBER
                    });
            }
            return items;
        }

        public void AceptarTicket(long idTicket)
        {
            var ticket = _repositorioTroubleTicket.ObtenerObjetos(new FiltroTroubleTicketPoId(idTicket)).FirstOrDefault();
            if (ticket == null)
                throw new ApplicationException($"No existe ticket cuyo id es {idTicket}");
            ticket.ACCEPTANCE_DATE = DateTime.Now;
            _repositorioTroubleTicket.Actualizar(ticket);
        }

        public void CerrarTicket(long idTicket, string notas, int motivo, int submotivo)
        {
            var ticket = _repositorioTroubleTicket.ObtenerObjetos(new FiltroTroubleTicketPoId(idTicket)).FirstOrDefault();
            if (ticket == null)
                throw new ApplicationException($"No existe ticket cuyo id es {idTicket}");
            ticket.FINISH_DATE = DateTime.Now;
            ticket.NOTES = notas;
            ticket.MOTIVE_ID = motivo;
            ticket.SUBMOTIVE_ID = submotivo;
            _repositorioTroubleTicket.Actualizar(ticket);
        }

        public IEnumerable<AUTO_TROUBLE_REASON> ObtenerMotivosAutoTickets()
        {
            return _repositorioAutoTroubleReason.ObtenerObjetos(new FiltroAutoTicketReasonActivos()).OrderByDescending(m => m.CAPTION).ToList();
        }

        public void CrearAutoTicket(int idMotivo, long idSesionUsuario)
        {
            _repositorioTroubleTicket.Agregar(new AUTO_TROUBLE_TICKET
            {
                ACCEPTANCE_DATE = DateTime.Now,
                ASSIGNMENT_DATE = DateTime.Now,
                CREATION_DATE = DateTime.Now,
                REASON_ID = idMotivo,
                USER_SESSION_ID = idSesionUsuario
            });
        }

        public void RegistrarAccion(ACTION accion)
        {
            accion.ACTION_DATE = DateTime.Now;
            _repositorioAction.Agregar(accion);
        }

        public IEnumerable<mb_get_ecuapass_message_pass_Result> ObtenerMensajesSmdtAduana(string numeroTransaccion)
        {
            return _repositorioAduana.ObtenerMensajesSmdtAduana(numeroTransaccion);
        }

        public byte? CambiarEstadoSmdt(string numeroTransaccion, string userName)
        {
            return _repositorioAduana.CambiarEstadoSmdt(numeroTransaccion, userName);
        }

        public mb_add_ecuapass_transaccion_Result AgregarTransaccionManual(DatosTransaccionManual datosTransaccionManual)
        {
            return _repositorioAduana.AgregarTransaccionManual(datosTransaccionManual.GKeyUnidad, datosTransaccionManual.TipoCarga, datosTransaccionManual.ObjetoSolicita, datosTransaccionManual.UsuarioSolicita, datosTransaccionManual.Contenedor, datosTransaccionManual.Mrn, datosTransaccionManual.Msn, datosTransaccionManual.Hsn, datosTransaccionManual.NumeroEntrega, datosTransaccionManual.Comentarios);
        }

        public void SuspenderTicket(long idTicket)
        {
            var ticket = _repositorioTroubleTicket.ObtenerTicketProcesos(new FiltroTroubleTicketProcesoPoId(idTicket)).FirstOrDefault();
            if (ticket == null)
                throw new ApplicationException($"No existe ticket cuyo id es {idTicket}");
            ticket.IS_SUSPENDED = true;
            _repositorioTroubleTicket.Actualizar(ticket);
        }

        public IEnumerable<BREAK_TYPE> ObtenerTiposDescansos()
        {
            return _repositorioBreakType.ObtenerObjetos(new FiltroBreakType()).OrderByDescending(b => b.NAME).ToList();
        }

        public int RegistrarDescanso(BREAK descanso)
        {
            descanso.START_BREAK_DATE = DateTime.Now;
            _repositorioBreak.Agregar(descanso);
            return descanso.BREAK_ID;
        }

        public void FinalizarDescanso(int idDescanso)
        {
            var descanso = _repositorioBreak.ObtenerObjetos(new FiltroBreakPorId(idDescanso)).FirstOrDefault();
            descanso.FINISH_BREAK_DATE = DateTime.Now;
            _repositorioBreak.Actualizar(descanso);
        }

        public void LiberarRecursos()
        {
            _repositorioUserSession.LiberarRecursos();
            _repositorioTransaccionQuiosco.LiberarRecursos();
            _repositorioTroubleTicket.LiberarRecursos();
            _repositorioAutoTroubleReason.LiberarRecursos();
            _repositorioAction.LiberarRecursos();
            _repositorioAduana.LiberarRecursos();
            _repositorioBreakType.LiberarRecursos();
            _repositorioBreak.LiberarRecursos();
        }
    }

    public class FiltroAutoTicketReasonActivos : IFiltros<AUTO_TROUBLE_REASON>
    {
        public Expression<Func<AUTO_TROUBLE_REASON, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<AUTO_TROUBLE_REASON>(atr => atr.IS_ACTIVE);
            return filtro.SastifechoPor();
        }
    }

    public class FiltroSesionUsuarioConTickets : IFiltros<USER_SESSION>
    {
        private readonly long _idSesionUsuario;

        public FiltroSesionUsuarioConTickets(long idSesionUsuario)
        {
            _idSesionUsuario = idSesionUsuario;
        }

        public Expression<Func<USER_SESSION, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<USER_SESSION>(us => us.ID == _idSesionUsuario);
            return filtro.SastifechoPor();
        }
    }

    public class FiltroTransaccionQuioscoPorId : IFiltros<KIOSK_TRANSACTION>
    {
        private readonly int _id;

        public FiltroTransaccionQuioscoPorId(int id)
        {
            _id = id;
        }

        public Expression<Func<KIOSK_TRANSACTION, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<KIOSK_TRANSACTION>(kt => kt.TRANSACTION_ID == _id);
            return filtro.SastifechoPor();
        }
    }

    public class FiltroTroubleTicketPoId : IFiltros<TROUBLE_TICKET>
    {
        private readonly long _idTicket;

        public FiltroTroubleTicketPoId(long idTicket)
        {
            _idTicket = idTicket;
        }

        public Expression<Func<TROUBLE_TICKET, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<TROUBLE_TICKET>(tt => tt.TT_ID == _idTicket);
            return filtro.SastifechoPor();
        }
    }

    public class FiltroTroubleTicketProcesoPoId : IFiltros<PROCESS_TROUBLE_TICKET>
    {
        private readonly long _idTicket;

        public FiltroTroubleTicketProcesoPoId(long idTicket)
        {
            _idTicket = idTicket;
        }

        public Expression<Func<PROCESS_TROUBLE_TICKET, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<PROCESS_TROUBLE_TICKET>(ptt => ptt.TT_ID == _idTicket);
            return filtro.SastifechoPor();
        }
    }

    public class FiltroBreakType : IFiltros<BREAK_TYPE>
    {
        public Expression<Func<BREAK_TYPE, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<BREAK_TYPE>(bt => bt.IS_ACTIVE);
            return filtro.SastifechoPor();
        }
    }

    public class FiltroBreakPorId : IFiltros<BREAK>
    {
        private readonly int _idDescanso;

        public FiltroBreakPorId(int idDescanso)
        {
            _idDescanso = idDescanso;
        }

        public Expression<Func<BREAK, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<BREAK>(b => b.BREAK_ID == _idDescanso);
            return filtro.SastifechoPor();
        }
    }
}

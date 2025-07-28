using RECEPTIO.CapaDominio.Console.Dominio.InterfacesRepositorios;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Filtros;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using RECEPTIO.CapaInfraestructura.Nucleo.Infraestructura;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace RECEPTIO.CapaInfraestructura.Console.SqlEntityFramework.Repositorios
{
    public class RepositorioTroubleTicket : Repositorio<TROUBLE_TICKET>, IRepositorioTroubleTicket
    {
        public IEnumerable<PROCESS_TROUBLE_TICKET> ObtenerTicketProcesos(IFiltros<PROCESS_TROUBLE_TICKET> filtro)
        {
            return Contexto.TROUBLE_TICKETS.OfType<PROCESS_TROUBLE_TICKET>().Include("ACTIONS").Include("REASSIGNMENTS").Include("USER_SESSION.TROUBLE_DESK_USER").Include("PROCESS.KIOSK_TRANSACTION.PRE_GATE.PRE_GATE_DETAILS.CONTAINERS").Include("PROCESS.KIOSK_TRANSACTION.PRE_GATE.PRE_GATE_DETAILS.TRANSACTION_TYPE").Include("PROCESS.MESSAGE").Include("PROCESS.KIOSK_TRANSACTION.KIOSK.ZONE").Where(filtro.SastifechoPor()).ToList();
        }

        public IEnumerable<AUTO_TROUBLE_TICKET> ObtenerAutoTicket(IFiltros<AUTO_TROUBLE_TICKET> filtro)
        {
            return Contexto.TROUBLE_TICKETS.OfType<AUTO_TROUBLE_TICKET>().Include("AUTO_TROUBLE_REASON").Include("USER_SESSION.DEVICE.ZONE").Include("USER_SESSION.TROUBLE_DESK_USER").Where(filtro.SastifechoPor()).ToList();
        }

        public IEnumerable<MOBILE_TROUBLE_TICKET> ObtenerTicketMobile(IFiltros<MOBILE_TROUBLE_TICKET> filtro)
        {
            return Contexto.TROUBLE_TICKETS.OfType<MOBILE_TROUBLE_TICKET>().Include("ZONE").Include("USER_SESSION.DEVICE.ZONE").Include("USER_SESSION.TROUBLE_DESK_USER").Where(filtro.SastifechoPor()).ToList();
        }

        public IEnumerable<CLIENT_APP_TRANSACTION_TROUBLE_TICKET> ObtenerTicketTecnico(IFiltros<CLIENT_APP_TRANSACTION_TROUBLE_TICKET> filtro)
        {
            return Contexto.TROUBLE_TICKETS.OfType<CLIENT_APP_TRANSACTION_TROUBLE_TICKET>().Include("ZONE").Include("ERROR").Include("USER_SESSION.DEVICE.ZONE").Include("USER_SESSION.TROUBLE_DESK_USER").Where(filtro.SastifechoPor()).ToList();
        }

        public void CrearTicketProceso(PROCESS_TROUBLE_TICKET ticket, long idProceso)
        {
            var proceso = Contexto.PROCESSES.FirstOrDefault(p => p.PROCESS_ID == idProceso);
            ticket.PROCESS = proceso ?? throw new Exception($"No existe proceso cuyo id es : {idProceso}");
            Contexto.TROUBLE_TICKETS.Add(ticket);
            Contexto.SaveChanges();
        }

        public void CrearTicketAppCliente(CLIENT_APP_TRANSACTION_TROUBLE_TICKET ticket, int idError)
        {
            var error = Contexto.ERRORS.FirstOrDefault(e => e.ERROR_ID == idError);
            ticket.ERROR = error ?? throw new Exception($"No existe error cuyo id es : {idError}");
            Contexto.TROUBLE_TICKETS.Add(ticket);
            Contexto.SaveChanges();
        }

        public void CrearMobileProceso(MOBILE_TROUBLE_TICKET ticket, long idTosProceso)
        {
            var proceso = Contexto.TOS_PROCCESSES.FirstOrDefault(tp => tp.ID == idTosProceso);
            ticket.TOS_PROCCESS = proceso ?? throw new Exception($"No existe proceso cuyo id es : {idTosProceso}");
            Contexto.TROUBLE_TICKETS.Add(ticket);
            Contexto.SaveChanges();
        }

        public void AgregarSesionUsuarioATicketProceso(long idTicket, long idSesionUsuario)
        {
            var ticket = Contexto.TROUBLE_TICKETS.OfType<PROCESS_TROUBLE_TICKET>().FirstOrDefault(tt => tt.TT_ID == idTicket);
            if (ticket == null)
                throw new ApplicationException($"No existe ticket cuyo id es {idTicket}");
            ticket.USER_SESSION_ID = idSesionUsuario;
            if (!ticket.ASSIGNMENT_DATE.HasValue)
                ticket.ASSIGNMENT_DATE = DateTime.Now;
            Contexto.Entry(ticket).State = EntityState.Modified;
            Contexto.SaveChanges();
        }

        public void AgregarSesionUsuarioATicketMobile(long idTicket, long idSesionUsuario)
        {
            var ticket = Contexto.TROUBLE_TICKETS.OfType<MOBILE_TROUBLE_TICKET>().FirstOrDefault(tt => tt.TT_ID == idTicket);
            if (ticket == null)
                throw new ApplicationException($"No existe ticket cuyo id es {idTicket}");
            ticket.USER_SESSION_ID = idSesionUsuario;
            if (!ticket.ASSIGNMENT_DATE.HasValue)
                ticket.ASSIGNMENT_DATE = DateTime.Now;
            Contexto.Entry(ticket).State = EntityState.Modified;
            Contexto.SaveChanges();
        }

        public void AgregarSesionUsuarioATicketTecnico(long idTicket, long idSesionUsuario)
        {
            var ticket = Contexto.TROUBLE_TICKETS.OfType<CLIENT_APP_TRANSACTION_TROUBLE_TICKET>().FirstOrDefault(tt => tt.TT_ID == idTicket);
            if (ticket == null)
                throw new ApplicationException($"No existe ticket cuyo id es {idTicket}");
            ticket.USER_SESSION_ID = idSesionUsuario;
            if (!ticket.ASSIGNMENT_DATE.HasValue)
                ticket.ASSIGNMENT_DATE = DateTime.Now;
            Contexto.Entry(ticket).State = EntityState.Modified;
            Contexto.SaveChanges();
        }

        public void AgregarSesionUsuarioAAutoTicket(long idTicket, long idSesionUsuario)
        {
            var ticket = Contexto.TROUBLE_TICKETS.OfType<AUTO_TROUBLE_TICKET>().FirstOrDefault(tt => tt.TT_ID == idTicket);
            if (ticket == null)
                throw new ApplicationException($"No existe ticket cuyo id es {idTicket}");
            ticket.USER_SESSION_ID = idSesionUsuario;
            if(!ticket.ASSIGNMENT_DATE.HasValue)
                ticket.ASSIGNMENT_DATE = DateTime.Now;
            Contexto.Entry(ticket).State = EntityState.Modified;
            Contexto.SaveChanges();
        }

        public void ReasignarTicketProceso(long idTicket, long idSesionUsuario, REASSIGNMENT reasignacion)
        {
            var ticket = Contexto.TROUBLE_TICKETS.OfType<PROCESS_TROUBLE_TICKET>().FirstOrDefault(tt => tt.TT_ID == idTicket);
            if (ticket == null)
                throw new ApplicationException($"No existe ticket cuyo id es {idTicket}");
            reasignacion.USER_SESSION_ID = ticket.USER_SESSION_ID.Value;
            ticket.USER_SESSION_ID = idSesionUsuario;
            ticket.IS_SUSPENDED = false;
            ticket.ACCEPTANCE_DATE = null;
            ticket.REASSIGNMENTS.Add(reasignacion);
            Contexto.Entry(ticket).State = EntityState.Modified;
            Contexto.SaveChanges();
        }
    }
}

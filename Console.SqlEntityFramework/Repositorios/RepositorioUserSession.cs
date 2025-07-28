using System.Collections.Generic;
using System.Linq;
using RECEPTIO.CapaDominio.Console.Dominio.InterfacesRepositorios;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Filtros;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using RECEPTIO.CapaInfraestructura.Nucleo.Infraestructura;

namespace RECEPTIO.CapaInfraestructura.Console.SqlEntityFramework.Repositorios
{
    public class RepositorioUserSession : Repositorio<USER_SESSION>, IRepositorioUserSession
    {
        public IEnumerable<USER_SESSION> ObtenerSesionUsuarioConTickets(IFiltros<USER_SESSION> filtro)
        {
            return Contexto.USER_SESSIONS.Include("AUTO_TROUBLE_TICKETS.AUTO_TROUBLE_REASON")
                .Include("PROCESS_TROUBLE_TICKETS.PROCESS.MESSAGE")
                .Include("PROCESS_TROUBLE_TICKETS.PROCESS.KIOSK_TRANSACTION.KIOSK")
                .Include("PROCESS_TROUBLE_TICKETS.PROCESS.KIOSK_TRANSACTION.PRE_GATE.PRE_GATE_DETAILS.TRANSACTION_TYPE")
                .Include("MOBILE_TROUBLE_TICKETS")
                .Include("CLIENT_APP_TRANSACTION_TROUBLE_TICKETS.ERROR")
                .Where(filtro.SastifechoPor()).ToList();
        }

        public IEnumerable<USER_SESSION> ObtenerSesionUsuarioConDispositivo(IFiltros<USER_SESSION> filtro)
        {
            return Contexto.USER_SESSIONS.Include("DEVICE").Where(filtro.SastifechoPor());
        }

        public IEnumerable<USER_SESSION> ObtenerSesionUsuarioConDispositivoZonaUsuario(IFiltros<USER_SESSION> filtro)
        {
            return Contexto.USER_SESSIONS.Include("DEVICE.ZONE").Include("TROUBLE_DESK_USER").Where(filtro.SastifechoPor());
        }
    }
}

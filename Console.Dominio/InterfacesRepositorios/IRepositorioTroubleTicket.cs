using RECEPTIO.CapaDominio.Nucleo.Dominio.Filtros;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Repositorio;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using System.Collections.Generic;

namespace RECEPTIO.CapaDominio.Console.Dominio.InterfacesRepositorios
{
    public interface IRepositorioTroubleTicket : IRepositorio<TROUBLE_TICKET>
    {
        IEnumerable<PROCESS_TROUBLE_TICKET> ObtenerTicketProcesos(IFiltros<PROCESS_TROUBLE_TICKET> filtro);
        IEnumerable<AUTO_TROUBLE_TICKET> ObtenerAutoTicket(IFiltros<AUTO_TROUBLE_TICKET> filtro);
        IEnumerable<MOBILE_TROUBLE_TICKET> ObtenerTicketMobile(IFiltros<MOBILE_TROUBLE_TICKET> filtro);
        IEnumerable<CLIENT_APP_TRANSACTION_TROUBLE_TICKET> ObtenerTicketTecnico(IFiltros<CLIENT_APP_TRANSACTION_TROUBLE_TICKET> filtro);
        void CrearTicketProceso(PROCESS_TROUBLE_TICKET ticket, long idProceso);
        void CrearTicketAppCliente(CLIENT_APP_TRANSACTION_TROUBLE_TICKET ticket, int idError);
        void CrearMobileProceso(MOBILE_TROUBLE_TICKET ticket, long idTosProceso);
        void AgregarSesionUsuarioATicketProceso(long idTicket, long idSesionUsuario);
        void AgregarSesionUsuarioATicketMobile(long idTicket, long idSesionUsuario);
        void AgregarSesionUsuarioATicketTecnico(long idTicket, long idSesionUsuario);
        void AgregarSesionUsuarioAAutoTicket(long idTicket, long idSesionUsuario);
        void ReasignarTicketProceso(long idTicket, long idSesionUsuario, REASSIGNMENT reasignacion);
    }
}

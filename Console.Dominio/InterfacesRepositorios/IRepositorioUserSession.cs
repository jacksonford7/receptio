using RECEPTIO.CapaDominio.Nucleo.Dominio.Filtros;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Repositorio;
using System.Collections.Generic;

namespace RECEPTIO.CapaDominio.Console.Dominio.InterfacesRepositorios
{
    public interface IRepositorioUserSession : IRepositorio<Nucleo.Entidades.USER_SESSION>
    {
        IEnumerable<Nucleo.Entidades.USER_SESSION> ObtenerSesionUsuarioConTickets(IFiltros<Nucleo.Entidades.USER_SESSION> filtro);
        IEnumerable<Nucleo.Entidades.USER_SESSION> ObtenerSesionUsuarioConDispositivo(IFiltros<Nucleo.Entidades.USER_SESSION> filtro);
        IEnumerable<Nucleo.Entidades.USER_SESSION> ObtenerSesionUsuarioConDispositivoZonaUsuario(IFiltros<Nucleo.Entidades.USER_SESSION> filtro);
    }
}
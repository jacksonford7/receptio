using RECEPTIO.CapaDominio.Nucleo.Dominio.Filtros;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Repositorio;
using System.Collections.Generic;

namespace RECEPTIO.CapaDominio.Mobile.Dominio.InterfacesRepositorios
{
    public interface IRepositorioZone : IRepositorio<Nucleo.Entidades.ZONE>
    {
        IEnumerable<Nucleo.Entidades.ZONE> ObtenerZonasConTipoTransaccion(IFiltros<Nucleo.Entidades.ZONE> filtro);
    }
}

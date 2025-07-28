using RECEPTIO.CapaDominio.Nucleo.Dominio.Filtros;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Repositorio;
using System.Collections.Generic;

namespace RECEPTIO.CapaDominio.Console.Dominio.InterfacesRepositorios
{
    public interface IRepositorioDevice : IRepositorio<Nucleo.Entidades.DEVICE>
    {
        IEnumerable<Nucleo.Entidades.DEVICE> ObtenerDispositivoConZona(IFiltros<Nucleo.Entidades.DEVICE> filtro);
    }
}
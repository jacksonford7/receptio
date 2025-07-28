using RECEPTIO.CapaDominio.Nucleo.Dominio.Filtros;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Repositorio;
using System.Collections.Generic;

namespace RECEPTIO.CapaDominio.Mobile.Dominio.InterfacesRepositorios
{
    public interface IRepositorioDevice : IRepositorio<Nucleo.Entidades.DEVICE>
    {
        IEnumerable<Nucleo.Entidades.DEVICE> ObtenerDevice(IFiltros<Nucleo.Entidades.DEVICE> filtro);
    }
}


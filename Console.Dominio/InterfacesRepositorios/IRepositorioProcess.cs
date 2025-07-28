using RECEPTIO.CapaDominio.Nucleo.Dominio.Filtros;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Repositorio;
using System.Collections.Generic;

namespace RECEPTIO.CapaDominio.Console.Dominio.InterfacesRepositorios
{
    public interface IRepositorioProcess : IRepositorio<Nucleo.Entidades.PROCESS>
    {
        IEnumerable<Nucleo.Entidades.PROCESS> ObtenerProcesoConMensaje(IFiltros<Nucleo.Entidades.PROCESS> filtro);
    }
}

using RECEPTIO.CapaDominio.Nucleo.Dominio.Filtros;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Repositorio;
using System.Collections.Generic;

namespace RECEPTIO.CapaDominio.Console.Dominio.InterfacesRepositorios
{
    public interface IRepositorioKioskTransaction : IRepositorio<Nucleo.Entidades.KIOSK_TRANSACTION>
    {
        IEnumerable<Nucleo.Entidades.KIOSK_TRANSACTION> ObtenerTransacionQuioscoConProcesosYDatos(IFiltros<Nucleo.Entidades.KIOSK_TRANSACTION> filtro);
    }
}

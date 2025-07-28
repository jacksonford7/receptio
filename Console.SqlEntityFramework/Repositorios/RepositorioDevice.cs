using RECEPTIO.CapaDominio.Console.Dominio.InterfacesRepositorios;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Filtros;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using RECEPTIO.CapaInfraestructura.Nucleo.Infraestructura;
using System.Collections.Generic;
using System.Linq;

namespace RECEPTIO.CapaInfraestructura.Console.SqlEntityFramework.Repositorios
{
    public class RepositorioDevice : Repositorio<DEVICE>, IRepositorioDevice
    {
        public IEnumerable<DEVICE> ObtenerDispositivoConZona(IFiltros<DEVICE> filtro)
        {
            return Contexto.DEVICES.Include("ZONE").Where(filtro.SastifechoPor()).ToList();
        }
    }
}

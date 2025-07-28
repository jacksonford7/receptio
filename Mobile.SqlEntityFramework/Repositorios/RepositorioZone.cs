using System.Collections.Generic;
using RECEPTIO.CapaDominio.Mobile.Dominio.InterfacesRepositorios;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Filtros;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using RECEPTIO.CapaInfraestructura.Nucleo.Infraestructura;
using System.Linq;

namespace RECEPTIO.CapaInfraestructura.Mobile.SqlEntityFramework.Repositorios
{
    public class RepositorioZone : Repositorio<ZONE>, IRepositorioZone
    {
        public IEnumerable<ZONE> ObtenerZonasConTipoTransaccion(IFiltros<ZONE> filtro)
        {
            return Contexto.ZONES.Include("TRANSACTION_TYPES").Where(filtro.SastifechoPor());
        }
    }
}

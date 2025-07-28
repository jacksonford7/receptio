using RECEPTIO.CapaDominio.Console.Dominio.InterfacesRepositorios;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Filtros;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using RECEPTIO.CapaInfraestructura.Nucleo.Infraestructura;
using System.Collections.Generic;
using System.Linq;

namespace RECEPTIO.CapaInfraestructura.Console.SqlEntityFramework.Repositorios
{
    public class RepositorioProcess : Repositorio<PROCESS>, IRepositorioProcess
    {
        public IEnumerable<PROCESS> ObtenerProcesoConMensaje(IFiltros<PROCESS> filtro)
        {
            return Contexto.PROCESSES.Include("MESSAGE").Include("KIOSK_TRANSACTION.KIOSK").Where(filtro.SastifechoPor());
        }
    }
}

using RECEPTIO.CapaDominio.Console.Dominio.InterfacesRepositorios;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Filtros;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using RECEPTIO.CapaInfraestructura.Nucleo.Infraestructura;
using System.Collections.Generic;
using System.Linq;

namespace RECEPTIO.CapaInfraestructura.Console.SqlEntityFramework.Repositorios
{
    public class RepositorioKioskTransaction : Repositorio<KIOSK_TRANSACTION>, IRepositorioKioskTransaction
    {
        public IEnumerable<KIOSK_TRANSACTION> ObtenerTransacionQuioscoConProcesosYDatos(IFiltros<KIOSK_TRANSACTION> filtro)
        {
            return Contexto.KIOSK_TRANSACTIONS
                .Include("PROCESSES.MESSAGE")
                .Include("PRE_GATE.PRE_GATE_DETAILS.TRANSACTION_TYPE")
                .Include("PRE_GATE.PRE_GATE_DETAILS.CONTAINERS.SEALS")
                .Include("PRE_GATE.PRE_GATE_DETAILS.CONTAINERS.DAMAGES")
                .Where(filtro.SastifechoPor()).ToList();
        }
    }
}

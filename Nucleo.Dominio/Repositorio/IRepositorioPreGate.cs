using RECEPTIO.CapaDominio.Nucleo.Dominio.Filtros;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using System.Collections.Generic;

namespace RECEPTIO.CapaDominio.Nucleo.Dominio.Repositorio
{
    public interface IRepositorioPreGate : IRepositorio<PRE_GATE>
    {
        IEnumerable<PRE_GATE> ObtenerPreGateConDetalle(IFiltros<PRE_GATE> filtro);
        IEnumerable<PRE_GATE> ObtenerPreGateConDetalleParaSupervisor(IFiltros<PRE_GATE> filtro);
        long ObtenerSecuenciaIdPreGate();
        string ObtenerValidacionesGenerales(string i_opcion, string _i_strValor, int i_IntValor, long i_bigintValor);
    }
}

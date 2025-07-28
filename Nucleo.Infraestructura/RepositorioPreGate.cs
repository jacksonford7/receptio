using RECEPTIO.CapaDominio.Nucleo.Dominio.Filtros;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Repositorio;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using RECEPTIO.CapaInfraestructura.Nucleo.Infraestructura.Modelo;
using System.Collections.Generic;
using System.Linq;

namespace RECEPTIO.CapaInfraestructura.Nucleo.Infraestructura
{
    public class RepositorioPreGate : Repositorio<PRE_GATE>, IRepositorioPreGate
    {
        public IEnumerable<PRE_GATE> ObtenerPreGateConDetalle(IFiltros<PRE_GATE> filtro)
        {
            return Contexto.PRE_GATES.Include("BY_PASS").Include("PRE_GATE_DETAILS.CONTAINERS").Include("KIOSK_TRANSACTIONS.PROCESSES.MESSAGE").Include("PRE_GATE_DETAILS.TRANSACTION_TYPE").Include("KIOSK_TRANSACTIONS.KIOSK.ZONE").Include("DEVICE.ZONE").Include("TOS_PROCCESSES").Where(filtro.SastifechoPor());
        }

        public IEnumerable<PRE_GATE> ObtenerPreGateConDetalleParaSupervisor(IFiltros<PRE_GATE> filtro)
        {
            return Contexto.PRE_GATES.AsNoTracking().Include("PRE_GATE_DETAILS.CONTAINERS").Include("KIOSK_TRANSACTIONS.PROCESSES.MESSAGE").Include("PRE_GATE_DETAILS.TRANSACTION_TYPE").Include("KIOSK_TRANSACTIONS.KIOSK.ZONE").Include("DEVICE.ZONE").Include("TOS_PROCCESSES").Where(filtro.SastifechoPor());
        }

        public long ObtenerSecuenciaIdPreGate()
        {
            return Contexto.mb_secuencia().FirstOrDefault().Value;
        }

        public string ObtenerValidacionesGenerales(string i_opcion, string _i_strValor,int i_IntValor,long i_bigintValor)
        {
            using (var contexto = new ModeloReceptioContainer())
            {
                var resultado = contexto.mb_get_validaciones(i_opcion ,_i_strValor,i_IntValor,i_bigintValor).FirstOrDefault(); 
                return  resultado;
            }
        }
    }
}

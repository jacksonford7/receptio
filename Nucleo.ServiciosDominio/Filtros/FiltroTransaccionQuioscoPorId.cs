using RECEPTIO.CapaDominio.Nucleo.Dominio.Filtros;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using System;
using System.Linq.Expressions;

namespace RECEPTIO.CapaDominio.Nucleo.ServiciosDominio.Filtros
{
    public class FiltroTransaccionQuioscoPorId : IFiltros<KIOSK_TRANSACTION>
    {
        private readonly int _id;

        public FiltroTransaccionQuioscoPorId(int id)
        {
            _id = id;
        }

        public Expression<Func<KIOSK_TRANSACTION, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<KIOSK_TRANSACTION>(t => t.TRANSACTION_ID == _id);
            return filtro.SastifechoPor();
        }
    }
}

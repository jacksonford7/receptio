using RECEPTIO.CapaDominio.Nucleo.Dominio.Repositorio;
using RECEPTIO.CapaInfraestructura.Nucleo.Infraestructura.Modelo;
using System.Linq;

namespace RECEPTIO.CapaInfraestructura.Nucleo.Infraestructura
{
    public class RepositorioValidaAduana : IRepositorioValidaAduana
    {
        public bool ValidaSmdt(string numeroTransaccion)
        {
            using (var contexto = new ModeloReceptioContainer())
            {
                return contexto.mb_get_valida_smdt(numeroTransaccion).FirstOrDefault().code == 0;
            }
        }

        public long ObtenerGKeyContenedor(string numeroContenedor)
        {
            using (var contexto = new ModeloReceptioContainer())
            {
                var resultado = contexto.mb_get_gkey_cont(numeroContenedor).FirstOrDefault();
                return resultado == null || !resultado.HasValue ? -1 : resultado.Value;
            }
        }

        public long ObtenerGKeyContenedorVacio(string numeroContenedor)
        {
            using (var contexto = new ModeloReceptioContainer())
            {
                var resultado = contexto.mb_get_gkey_cont_mty(numeroContenedor).FirstOrDefault();
                return resultado == null || !resultado.HasValue ? -1 : resultado.Value;
            }
        }
    }
}

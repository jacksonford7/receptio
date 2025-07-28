using RECEPTIO.CapaDominio.Nucleo.Dominio.Repositorio;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using RECEPTIO.CapaInfraestructura.Nucleo.Infraestructura.Modelo;
using System.Linq;

namespace RECEPTIO.CapaInfraestructura.Nucleo.Infraestructura
{
    public class RepositorioError : Repositorio<ERROR>, IRepositorioError
    {
        public string ObtenerValidacionesGenerales(string i_opcion, string _i_strValor, int i_IntValor, long i_bigintValor)
        {
            using (var contexto = new ModeloReceptioContainer())
            {
                var resultado = contexto.mb_get_validaciones(i_opcion, _i_strValor, i_IntValor, i_bigintValor).FirstOrDefault();
                return resultado;
            }
        }
    }
}

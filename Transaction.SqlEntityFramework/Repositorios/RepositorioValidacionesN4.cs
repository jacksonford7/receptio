using RECEPTIO.CapaDominio.Transaction.Dominio.InterfacesRepositorios;
using RECEPTIO.CapaInfraestructura.Nucleo.Infraestructura.Modelo;
using System.Linq;

namespace RECEPTIO.CapaInfraestructura.Transaction.SqlEntityFramework.Repositorios
{
    public class RepositorioValidacionesN4 : IRepositorioValidacionesN4
    {
        public bool EstaEnPatioContenedor(string contenedor)
        {
            using (var contexto = new ModeloReceptioContainer())
            {
                var resultado = contexto.mb_get_gkey_cont_yard(contenedor).FirstOrDefault();
                return resultado != null && resultado.HasValue;
            }
        }
    }
}

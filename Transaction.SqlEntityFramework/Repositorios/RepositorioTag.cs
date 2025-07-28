using RECEPTIO.CapaDominio.Transaction.Dominio.InterfacesRepositorios;
using RECEPTIO.CapaInfraestructura.Transaction.SqlEntityFramework.Modelos;
using System.Linq;

namespace RECEPTIO.CapaInfraestructura.Transaction.SqlEntityFramework.Repositorios
{
    public class RepositorioTag : IRepositorioTag
    {
        public string ObtenerTag(string placa)
        {
            using (var contexto = new OnlyAccessEntities())
            {
                return contexto.RECEPTIO_OBTENER_TAG(placa).FirstOrDefault();
            }
        }
    }
}

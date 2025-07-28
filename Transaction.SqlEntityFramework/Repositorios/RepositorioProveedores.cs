using RECEPTIO.CapaDominio.Transaction.Dominio.InterfacesRepositorios;

namespace RECEPTIO.CapaInfraestructura.Transaction.SqlEntityFramework.Repositorios
{
    public class RepositorioProveedores : IRepositorioProveedores
    {
        public bool TienePaseVip(string numeroPase, string cedula)
        {
            //using (var contexto = new ModeloReceptioContainer())

            //{
            //    var resultado = contexto.mb_get_gkey_cont_yard(contenedor).FirstOrDefault();
            //    var pp = Regex.Matches("", "");
            //    //pp.ge
            //    return resultado != null && resultado.HasValue;
            //}
            return true;
        }
    }
}

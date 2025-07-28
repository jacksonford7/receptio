namespace RECEPTIO.CapaDominio.Nucleo.Dominio.Repositorio
{
    public interface IRepositorioError : IRepositorio<Entidades.ERROR>
    {
        string ObtenerValidacionesGenerales(string i_opcion, string _i_strValor, int i_IntValor, long i_bigintValor);
    }
}

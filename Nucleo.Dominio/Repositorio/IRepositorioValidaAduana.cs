namespace RECEPTIO.CapaDominio.Nucleo.Dominio.Repositorio
{
    public interface IRepositorioValidaAduana
    {
        bool ValidaSmdt(string numeroTransaccion);
        long ObtenerGKeyContenedor(string numeroContenedor);
        long ObtenerGKeyContenedorVacio(string numeroContenedor);
    }
}

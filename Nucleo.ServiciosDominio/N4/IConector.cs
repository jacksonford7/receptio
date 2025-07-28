namespace RECEPTIO.CapaDominio.Nucleo.ServiciosDominio.N4
{
    public interface IConector
    {
        RespuestaServicioN4 Invocacion(string xml);
    }
}

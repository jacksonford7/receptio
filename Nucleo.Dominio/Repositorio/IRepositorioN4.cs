namespace RECEPTIO.CapaDominio.Nucleo.Dominio.Repositorio
{
    public interface IRepositorioN4
    {
        bool TieneTransaccionActiva(string gosTvKey);
        bool TieneTransaccionActivaPorPlaca(string placa);
        bool TieneTransaccionActivaDepotPorPlaca(string placa);
        bool EstaEnPatioContenedor(string contenedor);

    }
}

using RECEPTIO.CapaDominio.Nucleo.Dominio.Repositorio;

namespace RECEPTIO.CapaDominio.Console.Dominio.InterfacesRepositorios
{
    public interface IRepositorioByPass : IRepositorio<Nucleo.Entidades.BY_PASS>
    {
        void InsertarRegistro(Nucleo.Entidades.BY_PASS byPass);
    }
}

using RECEPTIO.CapaAplicacion.Transaction.Aplicacion.Interfaces;
using RECEPTIO.CapaDominio.Transaction.Dominio.InterfacesRepositorios;

namespace RECEPTIO.CapaDominio.Transaction.ServiciosDominio
{
    public class ServicioTag : ITag
    {
        private readonly IRepositorioTag _repositorio;

        public ServicioTag(IRepositorioTag repositorio)
        {
            _repositorio = repositorio;
        }

        public string ObtenerTag(string placa)
        {
            return _repositorio.ObtenerTag(placa);
        }
    }
}

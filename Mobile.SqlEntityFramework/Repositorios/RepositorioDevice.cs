using System;
using System.Collections.Generic;
using System.Linq;
using RECEPTIO.CapaDominio.Mobile.Dominio.InterfacesRepositorios;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Filtros;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using RECEPTIO.CapaInfraestructura.Nucleo.Infraestructura;
using System.Text;
using System.Threading.Tasks;

namespace RECEPTIO.CapaInfraestructura.Mobile.SqlEntityFramework.Repositorios
{
    class RepositorioDevice : Repositorio<DEVICE>, IRepositorioDevice
    {
        public IEnumerable<DEVICE> ObtenerDevice(IFiltros<DEVICE> filtro)
        {
            return Contexto.DEVICES.Where(filtro.SastifechoPor());
        }
    }
}
    
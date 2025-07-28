using RECEPTIO.CapaAplicacion.Mobile.Aplicacion.Interfaces;
using RECEPTIO.CapaDominio.Mobile.Dominio.InterfacesRepositorios;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Filtros;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace RECEPTIO.CapaDominio.Mobile.ServiciosDominio
{
    public class ServicioPrincipal : IPrincipal
    {
        private readonly IRepositorioZone _repositorioZone;
        private readonly IRepositorioDevice _repositorioDevice;

        public ServicioPrincipal(IRepositorioZone repositorioZone, IRepositorioDevice repositorioDevice)
        {
            _repositorioZone = repositorioZone;
            _repositorioDevice = repositorioDevice;
        }

        public ZONE ObtenerZonaConTiposTransacciones(string ip)
        {
            var zona = _repositorioZone.ObtenerZonasConTipoTransaccion(new FiltroZonaPorIp(ip)).FirstOrDefault();
            if (zona == null)
                throw new ApplicationException($"No existe zona cuyo ip es {ip}");
            return zona;
        }

        public DEVICE ObtenerDevice(string ip)
        {
            var dispositivo = _repositorioDevice.ObtenerDevice(new FiltroDevicePorIp(ip)).FirstOrDefault();
            if (dispositivo == null)
                throw new ApplicationException($"No existe dispositivo cuyo ip es {ip}");
            return dispositivo;
        }

        public void LiberarRecursos()
        {
            _repositorioZone.LiberarRecursos();
            _repositorioDevice.LiberarRecursos();
        }
    }

    public class FiltroZonaPorIp : IFiltros<ZONE>
    {
        private readonly string _ip;

        public FiltroZonaPorIp(string ip)
        {
            _ip = ip;
        }

        public Expression<Func<ZONE, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<ZONE>(z => z.DEVICES.Any(d => d.IP == _ip));
            return filtro.SastifechoPor();
        }
    }

    public class FiltroDevicePorIp : IFiltros<DEVICE>
    {
        private readonly string _ip;

        public FiltroDevicePorIp(string ip)
        {
            _ip = ip;
        }

        public Expression<Func<DEVICE, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<DEVICE>(z => z.IP == _ip);
            return filtro.SastifechoPor();
        }

    }

}

using RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Interfaces;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Filtros;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Repositorio;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace RECEPTIO.CapaDominio.Nucleo.ServiciosDominio.Entidades
{
    public class ServicioComunKiosco : IComunKiosco
    {
        private readonly IRepositorioApplication _repositorioApplication;
        private readonly IRepositorioMensaje _repositorioMensaje;
        private readonly IRepositorioQuiosco _repositorioQuiosco;
        private readonly IRepositorioDepot  _repositorioDepot;

        public ServicioComunKiosco(IRepositorioApplication repositorioApplication, IRepositorioMensaje repositorioMensaje, IRepositorioQuiosco repositorioQuiosco, IRepositorioDepot repositorioDepot)
        {
            _repositorioApplication = repositorioApplication;
            _repositorioMensaje = repositorioMensaje;
            _repositorioQuiosco = repositorioQuiosco;
            _repositorioDepot = repositorioDepot;
        }

        public APPLICATION ObtenerAplicacion(int idAplicacion)
        {
            return _repositorioApplication.ObtenerObjetos(new FiltroAplicacionPorId(idAplicacion)).FirstOrDefault();
        }

        public IEnumerable<MESSAGE> ObtenerMensajesErrores()
        {
            return _repositorioMensaje.ObtenerObjetos(new FiltroMensajesErrores()).ToList();
        }

        public KIOSK ObtenerQuiosco(string ip)
        {
            return _repositorioQuiosco.ObtenerObjetos(new FiltroQuioscoPorIp(ip)).FirstOrDefault();
        }

        public DEPOT ObtenerDepot(int id)
        {
            return _repositorioDepot.ObtenerObjetos(new FiltroDepotPorId(id)).FirstOrDefault();
        }

        public void LiberarRecursos()
        {
            _repositorioApplication.LiberarRecursos();
            _repositorioMensaje.LiberarRecursos();
            _repositorioQuiosco.LiberarRecursos();
        }
    }

    public class FiltroAplicacionPorId : IFiltros<APPLICATION>
    {
        private readonly int _idAplicacion;

        public FiltroAplicacionPorId(int idAplicacion)
        {
            _idAplicacion = idAplicacion;
        }

        public Expression<Func<APPLICATION, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<APPLICATION>(a => a.APPLICATION_ID == _idAplicacion);
            return filtro.SastifechoPor();
        }
    }

    public class FiltroMensajesErrores : IFiltros<MESSAGE>
    {
        public Expression<Func<MESSAGE, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<MESSAGE>(m => m.MESSAGE_ID > 0);
            return filtro.SastifechoPor();
        }
    }

    public class FiltroQuioscoPorIp : IFiltros<KIOSK>
    {
        private readonly string _ip;

        public FiltroQuioscoPorIp(string ip)
        {
            _ip = ip;
        }

        public Expression<Func<KIOSK, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<KIOSK>(q => q.IP == _ip);
            return filtro.SastifechoPor();
        }
    }

    public class FiltroDepotPorId : IFiltros<DEPOT>
    {
        private readonly int _id;

        public FiltroDepotPorId(int id)
        {
            _id = id;
        }

        public Expression<Func<DEPOT, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<DEPOT>(q => q.ID_DEPOT == _id);
            return filtro.SastifechoPor();
        }
    }
}

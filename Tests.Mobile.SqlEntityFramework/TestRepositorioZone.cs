using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Filtros;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using RECEPTIO.CapaInfraestructura.Mobile.SqlEntityFramework.Repositorios;

namespace RECEPTIO.CapaInfraestructura.Tests.Mobile.SqlEntityFramework
{
    [TestClass]
    public class TestRepositorioZone : IDisposable
    {
        private RepositorioZone _repositorio;
        private bool _disposed;

        [TestInitialize]
        public void Inicializar()
        {
            _repositorio = new RepositorioZone();
        }

        [TestMethod]
        public void TestObtenerZonas()
        {
            const int numeroMinimoItems = 1;
            var filtro = new FiltroPruebaZone();
            var items = _repositorio.ObtenerZonasConTipoTransaccion(filtro);
            Assert.IsTrue(items.Count() >= numeroMinimoItems);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
                _repositorio.Dispose();
            _disposed = true;
        }
    }

    internal class FiltroPruebaZone : Filtros<ZONE>
    {
        public override Expression<Func<ZONE, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<ZONE>(z => z.ZONE_ID > 0);
            return filtro.SastifechoPor();
        }
    }
}

using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Filtros;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using RECEPTIO.CapaInfraestructura.Console.SqlEntityFramework.Repositorios;

namespace RECEPTIO.CapaInfraestructura.Tests.Console.SqlEntityFramework
{
    [TestClass]
    public class TestRepositorioDevice : IDisposable
    {
        private RepositorioDevice _repositorio;
        private bool _disposed;

        [TestInitialize]
        public void Inicializar()
        {
            _repositorio = new RepositorioDevice();
        }

        [TestMethod]
        public void TestObtenerDispositivos()
        {
            const int numeroMinimoItems = 1;
            var filtro = new FiltroPruebaDevice();
            var items = _repositorio.ObtenerDispositivoConZona(filtro);
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

    internal class FiltroPruebaDevice : Filtros<DEVICE>
    {
        public override Expression<Func<DEVICE, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<DEVICE>(d => d.DEVICE_ID > 0);
            return filtro.SastifechoPor();
        }
    }
}

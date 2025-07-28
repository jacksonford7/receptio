using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Filtros;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using RECEPTIO.CapaInfraestructura.Nucleo.Infraestructura;

namespace RECEPTIO.CapaInfraestructura.Tests.Nucleo.Infraestructura
{
    [TestClass]
    public class TestRepositorioApplication : IDisposable
    {
        private RepositorioApplication _repositorio;
        private bool _disposed;

        [TestInitialize]
        public void Inicializar()
        {
            _repositorio = new RepositorioApplication();
        }

        [TestMethod]
        public void TestObtenerAplicaciones()
        {
            const int numeroMinimoItems = 3;
            var filtro = new FiltroPruebaAplicacion();
            var items = _repositorio.ObtenerObjetos(filtro);
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

    internal class FiltroPruebaAplicacion : Filtros<APPLICATION>
    {
        public override Expression<Func<APPLICATION, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<APPLICATION>(a => a.APPLICATION_ID > 0);
            return filtro.SastifechoPor();
        }
    }
}

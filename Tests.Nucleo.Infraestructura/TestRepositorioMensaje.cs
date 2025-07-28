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
    public class TestRepositorioMensaje : IDisposable
    {
        private RepositorioMensaje _repositorio;
        private bool _disposed;

        [TestInitialize]
        public void Inicializar()
        {
            _repositorio = new RepositorioMensaje();
        }

        [TestMethod]
        public void TestObtenerMensajes()
        {
            const int numeroMinimoItems = 1;
            var filtro = new FiltroPruebaMensaje();
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

    internal class FiltroPruebaMensaje : Filtros<MESSAGE>
    {
        public override Expression<Func<MESSAGE, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<MESSAGE>(m => m.MESSAGE_ID > 0);
            return filtro.SastifechoPor();
        }
    }
}

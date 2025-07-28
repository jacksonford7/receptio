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
    public class TestRepositorioProcess : IDisposable
    {
        private RepositorioProcess _repositorio;
        private bool _disposed;

        [TestInitialize]
        public void Inicializar()
        {
            _repositorio = new RepositorioProcess();
        }

        [TestMethod]
        public void TestObtenerProcesos()
        {
            const int numeroMinimoItems = 1;
            var filtro = new FiltroPruebaProcess();
            var items = _repositorio.ObtenerProcesoConMensaje(filtro);
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

    internal class FiltroPruebaProcess : Filtros<PROCESS>
    {
        public override Expression<Func<PROCESS, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<PROCESS>(p => p.TRANSACTION_ID > 0);
            return filtro.SastifechoPor();
        }
    }
}

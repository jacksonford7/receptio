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
    public class TestRepositorioBreakType : IDisposable
    {
        private RepositorioBreakType _repositorio;
        private bool _disposed;

        [TestInitialize]
        public void Inicializar()
        {
            _repositorio = new RepositorioBreakType();
        }

        [TestMethod]
        public void TestObtenerTiposDescansos()
        {
            const int numeroMinimoItems = 2;
            var filtro = new FiltroPruebaBreakType();
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

    internal class FiltroPruebaBreakType : Filtros<BREAK_TYPE>
    {
        public override Expression<Func<BREAK_TYPE, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<BREAK_TYPE>(bt => bt.ID > 0);
            return filtro.SastifechoPor();
        }
    }
}

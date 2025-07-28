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
    public class TestRepositorioTransactionType : IDisposable
    {
        private RepositorioTransactionType _repositorio;
        private bool _disposed;

        [TestInitialize]
        public void Inicializar()
        {
            _repositorio = new RepositorioTransactionType();
        }

        [TestMethod]
        public void TestObtenerAutoTroubleReason()
        {
            const int numeroMinimoItems = 1;
            var filtro = new FiltroPruebaRepositorioTransactionType();
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

    internal class FiltroPruebaRepositorioTransactionType : Filtros<TRANSACTION_TYPE>
    {
        public override Expression<Func<TRANSACTION_TYPE, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<TRANSACTION_TYPE>(tt => tt.ID > 0);
            return filtro.SastifechoPor();
        }
    }
}

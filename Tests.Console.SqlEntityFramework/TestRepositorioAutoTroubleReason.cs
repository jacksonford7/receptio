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
    public class TestRepositorioAutoTroubleReason : IDisposable
    {
        private RepositorioAutoTroubleReason _repositorio;
        private bool _disposed;

        [TestInitialize]
        public void Inicializar()
        {
            _repositorio = new RepositorioAutoTroubleReason();
        }

        [TestMethod]
        public void TestObtenerAutoTroubleReason()
        {
            const int numeroMinimoItems = 1;
            var filtro = new FiltroPruebaAutoTroubleReason();
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

    internal class FiltroPruebaAutoTroubleReason : Filtros<AUTO_TROUBLE_REASON>
    {
        public override Expression<Func<AUTO_TROUBLE_REASON, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<AUTO_TROUBLE_REASON>(atr => atr.ID > 0);
            return filtro.SastifechoPor();
        }
    }
}

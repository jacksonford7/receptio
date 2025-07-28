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
    public class TestRepositorioQuiosco : IDisposable
    {
        private RepositorioQuiosco _repositorio;
        private bool _disposed;

        [TestInitialize]
        public void Inicializar()
        {
            _repositorio = new RepositorioQuiosco();
        }

        [TestMethod]
        public void TestObtenerQuioscos()
        {
            const int numeroMinimoItems = 1;
            var filtro = new FiltroPruebaQuiosco();
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

    internal class FiltroPruebaQuiosco : Filtros<KIOSK>
    {
        public override Expression<Func<KIOSK, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<KIOSK>(q => q.KIOSK_ID > 0);
            return filtro.SastifechoPor();
        }
    }
}

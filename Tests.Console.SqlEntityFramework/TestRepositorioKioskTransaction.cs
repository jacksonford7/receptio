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
    public class TestRepositorioKioskTransaction : IDisposable
    {
        private RepositorioKioskTransaction _repositorio;
        private bool _disposed;

        [TestInitialize]
        public void Inicializar()
        {
            _repositorio = new RepositorioKioskTransaction();
        }

        [TestMethod]
        public void TestObtenerTransaccionesQuiosco()
        {
            const int numeroMinimoItems = 1;
            var filtro = new FiltroPruebaTransaccionQuiosco();
            var items = _repositorio.ObtenerTransacionQuioscoConProcesosYDatos(filtro);
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

    internal class FiltroPruebaTransaccionQuiosco : Filtros<KIOSK_TRANSACTION>
    {
        public override Expression<Func<KIOSK_TRANSACTION, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<KIOSK_TRANSACTION>(kt => kt.TRANSACTION_ID > 0);
            return filtro.SastifechoPor();
        }
    }
}

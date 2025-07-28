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
    public class TestRepositorioTransaccionQuiosco : IDisposable
    {
        private RepositorioTransaccionQuiosco _repositorio;
        private bool _disposed;

        [TestInitialize]
        public void Inicializar()
        {
            _repositorio = new RepositorioTransaccionQuiosco();
        }

        [TestMethod]
        public void TestObtenerTransacciones()
        {
            const int numeroMinimoItems = 1;
            var filtro = new FiltroPruebaTransaccion();
            var items = _repositorio.ObtenerObjetos(filtro);
            Assert.IsTrue(items.Count() >= numeroMinimoItems);
        }

        [TestMethod]
        public void TestCrearTransaccion()
        {
            var item = new KIOSK_TRANSACTION
            {
                END_DATE = DateTime.Now,
                KIOSK_ID = 1,
                START_DATE = DateTime.Now
            };
            try
            {
                _repositorio.Agregar(item);
                Assert.IsTrue(true);
            }
            catch (Exception ex)
            {
                Assert.Fail($"{ex.Message} {ex.InnerException} {ex.StackTrace}");
            }
        }

        [TestMethod]
        public void TestActualizarTransaccion()
        {
            const int id = 1;
            var filtro = new FiltroPruebaTransaccionActualizar(id);
            var item = _repositorio.ObtenerObjetos(filtro).FirstOrDefault();
            if (item == null)
                Assert.Fail("Test falló porque no existe ningún item a actualizar.");
            item.IS_OK = true;
            _repositorio.Actualizar(item);
            item = _repositorio.ObtenerObjetos(filtro).FirstOrDefault();
            if (item == null)
                Assert.Fail("Test falló porque no existe ningún item a actualizar.");
            Assert.IsTrue(item.IS_OK);
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

    internal class FiltroPruebaTransaccion : Filtros<KIOSK_TRANSACTION>
    {
        public override Expression<Func<KIOSK_TRANSACTION, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<KIOSK_TRANSACTION>(t => t.TRANSACTION_ID > 0 && t.TRANSACTION_ID < 5);
            return filtro.SastifechoPor();
        }
    }

    internal class FiltroPruebaTransaccionActualizar : Filtros<KIOSK_TRANSACTION>
    {
        private readonly int _id;

        internal FiltroPruebaTransaccionActualizar(int id)
        {
            _id = id;
        }

        public override Expression<Func<KIOSK_TRANSACTION, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<KIOSK_TRANSACTION>(t => t.TRANSACTION_ID == _id);
            return filtro.SastifechoPor();
        }
    }
}

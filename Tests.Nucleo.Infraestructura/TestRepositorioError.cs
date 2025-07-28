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
    public class TestRepositorioError : IDisposable
    {
        private RepositorioError _repositorio;
        private bool _disposed;

        [TestInitialize]
        public void Inicializar()
        {
            _repositorio = new RepositorioError();
        }

        [TestMethod]
        public void TestObtenerErrores()
        {
            const int numeroMinimoItems = 1;
            var filtro = new FiltroPruebaError();
            var items = _repositorio.ObtenerObjetos(filtro);
            Assert.IsTrue(items.Count() >= numeroMinimoItems);
        }

        [TestMethod]
        public void TestCrearError()
        {
            var item = new ERROR
            {
                APPLICATION_ID = 2,
                DETAILS = "PJHHJ",
                MESSAGE = "MANGA HIZO LA CASITA",
                THROW_ON = DateTime.Now,
                TYPE_ERROR_ID = 1
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

    internal class FiltroPruebaError : Filtros<ERROR>
    {
        public override Expression<Func<ERROR, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<ERROR>(e => e.ERROR_ID > 0);
            return filtro.SastifechoPor();
        }
    }
}

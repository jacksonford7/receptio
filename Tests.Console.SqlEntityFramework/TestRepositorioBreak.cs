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
    public class TestRepositorioBreak : IDisposable
    {
        private RepositorioBreak _repositorio;
        private bool _disposed;

        [TestInitialize]
        public void Inicializar()
        {
            _repositorio = new RepositorioBreak();
        }

        [TestMethod]
        public void TestDescanso()
        {
            try
            {
                var item = new BREAK
                {
                    BREAK_TYPE_ID = 2,
                    START_BREAK_DATE = DateTime.Now,
                    USER_SESSION_ID = 1
                };
                _repositorio.Agregar(item);
                Assert.IsTrue(true);
            }
            catch (Exception ex)
            {
                Assert.Fail($"{ex.Message} {ex.InnerException} {ex.StackTrace}");
            }
        }


        [TestMethod]
        public void TestActualizarDescanso()
        {
            const int id = 1;
            var filtro = new FiltroPruebaDescansoActualizar(id);
            var item = _repositorio.ObtenerObjetos(filtro).FirstOrDefault();
            if (item == null)
                Assert.Fail("Test falló porque no existe ningún item a actualizar.");
            item.FINISH_BREAK_DATE = DateTime.Now;
            _repositorio.Actualizar(item);
            item = _repositorio.ObtenerObjetos(filtro).FirstOrDefault();
            if (item == null)
                Assert.Fail("Test falló porque no existe ningún item a actualizar.");
            Assert.IsTrue(item.FINISH_BREAK_DATE.HasValue);
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

    internal class FiltroPruebaDescansoActualizar : Filtros<BREAK>
    {
        private readonly int _id;

        internal FiltroPruebaDescansoActualizar(int id)
        {
            _id = id;
        }

        public override Expression<Func<BREAK, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<BREAK>(b => b.BREAK_ID == _id);
            return filtro.SastifechoPor();
        }
    }
}

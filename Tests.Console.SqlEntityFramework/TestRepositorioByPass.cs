using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Filtros;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using RECEPTIO.CapaInfraestructura.Console.SqlEntityFramework.Repositorios;

namespace RECEPTIO.CapaInfraestructura.Tests.Console.SqlEntityFramework
{
    [TestClass]
    public class TestRepositorioByPass : IDisposable
    {
        private RepositorioByPass _repositorio;
        private bool _disposed;

        [TestInitialize]
        public void Inicializar()
        {
            _repositorio = new RepositorioByPass();
        }

        [TestMethod]
        public void TestCrearRegistro()
        {
            try
            {
                var item = new BY_PASS
                {
                    REASON = "Ejemplo",
                    IS_ENABLED = true,
                    BY_PASS_AUDITS = new List<BY_PASS_AUDIT>
                    {
                        new BY_PASS_AUDIT
                        {
                            DATE = DateTime.Now,
                            FIELD = "REASON",
                            NEW_VALUE = "Ejemplo",
                            OLD_VALUE = "",
                            TTU_ID = 1
                        },
                        new BY_PASS_AUDIT
                        {
                            DATE = DateTime.Now,
                            FIELD = "IS_ENABLED",
                            NEW_VALUE = "1",
                            OLD_VALUE = "",
                            TTU_ID = 1
                        }
                    },
                    PRE_GATE = new PRE_GATE { PRE_GATE_ID =  131}
                };
                _repositorio.InsertarRegistro(item);
                Assert.IsTrue(true);
            }
            catch (Exception ex)
            {
                Assert.Fail($"{ex.Message} {ex.InnerException} {ex.StackTrace}");
            }
        }

        [TestMethod]
        public void TestActualizarByPass()
        {
            const int id = 1;
            var filtro = new FiltroPruebaByPassActualizar(id);
            var item = _repositorio.ObtenerObjetos(filtro).FirstOrDefault();
            if (item == null)
                Assert.Fail("Test falló porque no existe ningún item a actualizar.");
            item.IS_ENABLED = false;
            _repositorio.Actualizar(item);
            item = _repositorio.ObtenerObjetos(filtro).FirstOrDefault();
            if (item == null)
                Assert.Fail("Test falló porque no existe ningún item a actualizar.");
            Assert.IsFalse(item.IS_ENABLED);
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

    internal class FiltroPruebaByPassActualizar : Filtros<BY_PASS>
    {
        private readonly int _id;

        public FiltroPruebaByPassActualizar(int id)
        {
            _id = id;
        }

        public override Expression<Func<BY_PASS, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<BY_PASS>(bp => bp.ID == _id);
            return filtro.SastifechoPor();
        }
    }
}

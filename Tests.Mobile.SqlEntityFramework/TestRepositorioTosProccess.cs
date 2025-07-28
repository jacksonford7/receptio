using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using RECEPTIO.CapaInfraestructura.Mobile.SqlEntityFramework.Repositorios;

namespace RECEPTIO.CapaInfraestructura.Tests.Mobile.SqlEntityFramework
{
    [TestClass]
    public class TestRepositorioTosProccess : IDisposable
    {
        private RepositorioTosProccess _repositorio;
        private bool _disposed;

        [TestInitialize]
        public void Inicializar()
        {
            _repositorio = new RepositorioTosProccess();
        }

        [TestMethod]
        public void TestCrearProceso()
        {
            try
            {
                var item = new TOS_PROCCESS
                {
                    PRE_GATE_ID = 1,
                    RESPONSE = "",
                    STEP = "",
                    STEP_DATE = DateTime.Now
                };
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
}

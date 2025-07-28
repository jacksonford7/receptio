using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using RECEPTIO.CapaInfraestructura.Console.SqlEntityFramework.Repositorios;

namespace RECEPTIO.CapaInfraestructura.Tests.Console.SqlEntityFramework
{
    [TestClass]
    public class TestRepositorioLiftBarrier : IDisposable
    {
        private RepositorioLiftBarrier _repositorio;
        private bool _disposed;

        [TestInitialize]
        public void Inicializar()
        {
            _repositorio = new RepositorioLiftBarrier();
        }

        [TestMethod]
        public void TestCrearRegistro()
        {
            try
            {
                var item = new LIFT_UP_BARRIER
                {
                    REASON = "MANGA LE DIO LA GANA.",
                    DATE = DateTime.Now,
                    KIOSK_ID = 1,
                    TTU_ID = 1
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

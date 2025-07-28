using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RECEPTIO.CapaInfraestructura.Console.SqlEntityFramework.Repositorios;

namespace RECEPTIO.CapaInfraestructura.Tests.Console.SqlEntityFramework
{
    [TestClass]
    public class TestRepositorioAduana : IDisposable
    {
        private RepositorioAduana _repositorio;
        private bool _disposed;

        [TestInitialize]
        public void Inicializar()
        {
            _repositorio = new RepositorioAduana();
        }

        [TestMethod]
        public void TestObtenerMensajesSmdtAduana()
        {
            const string numeroTransaccion = "59761";
            var resultado = _repositorio.ObtenerMensajesSmdtAduana(numeroTransaccion);
            Assert.AreEqual(4, resultado.Count());
        }

        [TestMethod]
        public void TestAgregarTransaccionManual()
        {
            var resultado = _repositorio.AgregarTransaccionManual(59767, "CNTR", "webservice", "manga", "XXX1234567", "CEC2018123456", "0025", "0023", "00575111111567P", "ERROR DE MANGA");
            Assert.AreEqual("Ok", resultado.message);
        }

        [TestMethod]
        public void TestCambiarEstadoSmdt()
        {
            var resultado = _repositorio.CambiarEstadoSmdt("321321", "manga");
            Assert.IsTrue(resultado.HasValue);
            Assert.AreEqual(0, resultado.Value);
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

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RECEPTIO.CapaPresentacion.UI.UsbRelay;

namespace RECEPTIO.CapaPresentacion.Tests.UI.Interfaces
{
    [TestClass]
    public class TestBarrera : IDisposable
    {
        private bool _disposed;
        private Barrera _servicio;

        [TestInitialize]
        public void Inicializar()
        {
            _servicio = new Barrera();
        }

        [TestMethod]
        public void TestConectarFalla()
        {
            var resultado = _servicio.Conectar();
            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void TestConectarOk()
        {
            var resultado = _servicio.Conectar();
            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void TestLevantarBarrera()
        {
            var resultado = _servicio.Conectar();
            _servicio.LevantarBarrera();
            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void TestBajarBarrera()
        {
            var resultado = _servicio.Conectar();
            _servicio.LevantarBarrera();
            _servicio.BajarBarrera();
            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void TestLevantaryBajarBarrera()
        {
            var resultado = _servicio.Conectar();
            _servicio.LevantarBarrera();
            _servicio.BajarBarrera();
            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void TestObtenerInputs()
        {
            _servicio.Conectar();
            var resultado = _servicio.ObtenerInputs();
            Assert.IsFalse(resultado[0]);
            Assert.IsFalse(resultado[1]);
            Assert.IsFalse(resultado[2]);
            Assert.IsFalse(resultado[3]);
        }

        [TestMethod]
        public void TestObtenerOutputs()
        {
            _servicio.Conectar();
            _servicio.LevantarBarrera();
            var resultado = _servicio.ObtenerOutputs();
            Assert.IsTrue(resultado[1]);
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
                _servicio.Dispose();
            _disposed = true;
        }
    }
}

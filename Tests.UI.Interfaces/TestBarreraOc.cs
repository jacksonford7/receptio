using System;
using System.Threading;
using System.Windows.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RECEPTIO.CapaPresentacion.UI.UsbRelay;

namespace RECEPTIO.CapaPresentacion.Tests.UI.Interfaces
{
    [TestClass]
    public class TestBarreraOc : IDisposable
    {
        private bool _disposed;
        private BarreraOc _servicio;

        [TestInitialize]
        public void Inicializar()
        {
            _servicio = new BarreraOc();
        }

        [TestMethod]
        public void TestConectarFalla()
        {
            var resultado = _servicio.Conectar();
            _servicio.DesconectarUsbRelay();
            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void TestConectarOk()
        {
            var resultado = _servicio.Conectar();
            _servicio.DesconectarUsbRelay();
            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void TestLevantaryBajarBarrera()
        {
            _servicio.Conectar();
            _servicio.LevantarBarrera();
            Thread.Sleep(4000);
            var resultado = _servicio.DesconectarUsbRelay();
            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void TestObtenerInputs()
        {
            _servicio.Conectar();
            var resultado = _servicio.ObtenerInputs();
            _servicio.DesconectarUsbRelay();
            Assert.IsFalse(resultado[0]);
            Assert.IsFalse(resultado[1]);
            Assert.IsFalse(resultado[2]);
            Assert.IsFalse(resultado[3]);
        }

        [TestMethod]
        public void TestObtenerOutputs()
        {
            _servicio.Conectar();
            var resultado = _servicio.ObtenerOutputs();
            _servicio.DesconectarUsbRelay();
            Assert.IsFalse(resultado[1]);
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

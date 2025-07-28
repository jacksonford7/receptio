using Microsoft.VisualStudio.TestTools.UnitTesting;
using RECEPTIO.CapaPresentacion.UI.Interfaces.RFID;
using System;
using System.Linq;

namespace RECEPTIO.CapaPresentacion.Tests.UI.Interfaces
{
    [TestClass]
    public class TestAntena : IDisposable
    {
        private bool _disposed;
        private IAntena _servicio;

        [TestInitialize]
        public void Inicializar()
        {
            //_servicio = new CapaPresentacion.UI.RFID.Antena();
            _servicio = new CapaPresentacion.UI.RFID_CHAFON.Antena();
        }

        [TestMethod]
        public void TestObtenerAntenaRfidOk()
        {
            var resultado = _servicio.ConectarAntena();
            _servicio.DesconectarAntena();
            _servicio.Dispose();
            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public void TestProcesoRfidCuandoNoLeeTag()
        {
            _servicio.ConectarAntena();
            _servicio.IniciarLectura();
            var resultado = _servicio.ObtenerTagsLeidos();
            _servicio.DesconectarAntena();
            _servicio.Dispose();
            Assert.AreEqual(0, resultado.Count());
        }

        [TestMethod]
        public void TestProcesoRfidOk()
        {
            _servicio.ConectarAntena();
            _servicio.IniciarLectura();
            var resultado = _servicio.TerminarLectura();
            _servicio.DesconectarAntena();
            _servicio.Dispose();
            Assert.IsNotNull(resultado);
            Assert.AreEqual(1, resultado.Count());
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

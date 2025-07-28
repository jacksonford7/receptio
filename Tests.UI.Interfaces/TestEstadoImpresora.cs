using Microsoft.VisualStudio.TestTools.UnitTesting;
using RECEPTIO.CapaPresentacion.UI.ImpresoraZebra;

namespace RECEPTIO.CapaPresentacion.Tests.UI.Interfaces
{
    [TestClass]
    public class TestEstadoImpresora
    {
        private EstadoImpresora _servicio;

        [TestInitialize]
        public void Inicializar()
        {
            _servicio = new EstadoImpresora();
        }

        [TestMethod]
        public void TestChequearEstadoImpresoraCuandoNoEstaConectada()
        {
            var resultado = _servicio.VerEstado();
            Assert.IsTrue(resultado.Item1.Count == 1);
            Assert.AreEqual("La impresora no está conectada.", resultado.Item1[0]);
        }

        [TestMethod]
        public void TestChequearEstadoImpresoraCuandoNoTienePapel()
        {
            var resultado = _servicio.VerEstado();
            Assert.IsTrue(resultado.Item1.Count > 1);
            Assert.IsTrue(resultado.Item1.Contains("No hay papel."));
        }

        [TestMethod]
        public void TestChequearEstadoImpresoraCuandoNoHayErrores()
        {
            var resultado = _servicio.VerEstado();
            Assert.IsTrue(resultado.Item1.Count == 0);
        }
    }
}

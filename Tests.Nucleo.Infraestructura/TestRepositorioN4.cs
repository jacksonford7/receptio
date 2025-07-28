using Microsoft.VisualStudio.TestTools.UnitTesting;
using RECEPTIO.CapaInfraestructura.Nucleo.Infraestructura;

namespace RECEPTIO.CapaInfraestructura.Tests.Nucleo.Infraestructura
{
    [TestClass]
    public class TestRepositorioN4
    {
        private RepositorioN4 _repositorio;

        [TestInitialize]
        public void Inicializar()
        {
            _repositorio = new RepositorioN4();
        }

        [TestMethod]
        public void TestTieneTransaccionActivaCuandoNoTiene()
        {
            var resultado = _repositorio.TieneTransaccionActiva("160");
            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void TestTieneTransaccionActivaCuandoTiene()
        {
            var resultado = _repositorio.TieneTransaccionActiva("140");
            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void TestTieneTransaccionActivaPlacaCuandoNoTiene()
        {
            var resultado = _repositorio.TieneTransaccionActivaPorPlaca("GBN2489");
            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void TestTieneTransaccionActivaPlacaCuandoTiene()
        {
            var resultado = _repositorio.TieneTransaccionActivaPorPlaca("XAH0740");
            Assert.IsTrue(resultado);
        }
    }
}

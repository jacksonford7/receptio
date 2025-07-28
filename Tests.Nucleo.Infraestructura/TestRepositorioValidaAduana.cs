using Microsoft.VisualStudio.TestTools.UnitTesting;
using RECEPTIO.CapaInfraestructura.Nucleo.Infraestructura;

namespace RECEPTIO.CapaInfraestructura.Tests.Nucleo.Infraestructura
{
    [TestClass]
    public class TestRepositorioValidaAduana
    {
        private RepositorioValidaAduana _repositorio;

        [TestInitialize]
        public void Inicializar()
        {
            _repositorio = new RepositorioValidaAduana();
        }

        [TestMethod]
        public void TestValidaSmdtNoOk()
        {
            const string numeroTransaccion = "sss";
            var resultado = _repositorio.ValidaSmdt(numeroTransaccion);
            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void TestValidaSmdtOk()
        {
            const string numeroTransaccion = "59766";
            var resultado = _repositorio.ValidaSmdt(numeroTransaccion);
            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void TestObtenerGKeyContenedor()
        {
            const string numeroContenedor = "KOSU2201661";
            var resultado = _repositorio.ObtenerGKeyContenedor(numeroContenedor);
            Assert.AreEqual(4290346, resultado);
        }

        [TestMethod]
        public void TestObtenerGKeyContenedorVacio()
        {
            const string numeroContenedor = "MNBU0065849";
            var resultado = _repositorio.ObtenerGKeyContenedorVacio(numeroContenedor);
            Assert.AreEqual(4285881, resultado);
        }
    }
}

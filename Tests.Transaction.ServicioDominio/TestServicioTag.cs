using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RECEPTIO.CapaDominio.Transaction.Dominio.InterfacesRepositorios;
using RECEPTIO.CapaDominio.Transaction.ServiciosDominio;

namespace RECEPTIO.CapaDominio.Tests.Transaction.ServicioDominio
{
    [TestClass]
    public class TestServicioTag
    {
        private ServicioTag _servicio;
        private Mock<IRepositorioTag> _mock;

        [TestInitialize]
        public void Inicializar()
        {
            _mock = new Mock<IRepositorioTag>();
            _servicio = new ServicioTag(_mock.Object);
        }

        [TestMethod]
        public void TestObtenerTag()
        {
            const string tag = "123";
            _mock.Setup(m => m.ObtenerTag(It.IsAny<string>())).Returns(tag);
            var resultado = _servicio.ObtenerTag("");
            Assert.AreEqual(tag, resultado);
        }
    }
}

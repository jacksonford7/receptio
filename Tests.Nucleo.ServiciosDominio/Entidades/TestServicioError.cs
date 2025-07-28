using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Repositorio;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using RECEPTIO.CapaDominio.Nucleo.ServiciosDominio.Entidades;

namespace RECEPTIO.CapaDominio.Tests.Nucleo.ServiciosDominio.Entidades
{
    [TestClass]
    public class TestServicioError
    {
        private ServicioError _servicio;
        private Mock<IRepositorioError> _mock;

        [TestInitialize]
        public void Inicializar()
        {
            _mock = new Mock<IRepositorioError>();
            _servicio = new ServicioError(_mock.Object);
        }

        [TestMethod]
        public void TestCrearError()
        {
            _mock.Setup(m => m.Agregar(It.IsAny<ERROR>()));
            var resultado = _servicio.CrearError(new ERROR());
            Assert.AreEqual(0, resultado);
        }

        [TestMethod]
        public void TestGrabarErrorTecnico()
        {
            _mock.Setup(m => m.Agregar(It.IsAny<ERROR>()));
            var resultado =_servicio.GrabarErrorTecnico(new ERROR());
            Assert.AreEqual(0, resultado);
        }
    }
}

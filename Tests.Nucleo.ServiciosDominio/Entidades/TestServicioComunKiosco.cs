using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Repositorio;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using RECEPTIO.CapaDominio.Nucleo.ServiciosDominio.Entidades;

namespace RECEPTIO.CapaDominio.Tests.Nucleo.ServiciosDominio.Entidades
{
    [TestClass]
    public class TestServicioComunKiosco
    {
        private ServicioComunKiosco _servicio;
        private Mock<IRepositorioApplication> _mockApplication;
        private Mock<IRepositorioMensaje> _mockMensaje;
        private Mock<IRepositorioQuiosco> _mockQuiosco;
        private Mock<IRepositorioDepot> _mockDepot;

        [TestInitialize]
        public void Inicializar()
        {
            _mockApplication = new Mock<IRepositorioApplication>();
            _mockMensaje = new Mock<IRepositorioMensaje>();
            _mockQuiosco = new Mock<IRepositorioQuiosco>();
            _mockDepot = new Mock<IRepositorioDepot>();
            _servicio = new ServicioComunKiosco(_mockApplication.Object, _mockMensaje.Object, _mockQuiosco.Object,_mockDepot.Object);
        }

        [TestMethod]
        public void TestObtenerAplicacion()
        {
            _mockApplication.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroAplicacionPorId>())).Returns(new List<APPLICATION>());
            var resultado = _servicio.ObtenerAplicacion(0);
            Assert.IsNull(resultado);
        }

        [TestMethod]
        public void TestObtenerMensaje()
        {
            _mockMensaje.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroMensajesErrores>())).Returns(new List<MESSAGE>());
            var resultado = _servicio.ObtenerMensajesErrores();
            Assert.AreEqual(0, resultado.Count());
        }

        [TestMethod]
        public void TestObtenerQuiosco()
        {
            _mockQuiosco.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroQuioscoPorIp>())).Returns(new List<KIOSK>());
            var resultado = _servicio.ObtenerQuiosco("");
            Assert.IsNull(resultado);
        }
    }
}

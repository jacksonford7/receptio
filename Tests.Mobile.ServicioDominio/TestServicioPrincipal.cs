using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RECEPTIO.CapaDominio.Mobile.Dominio.InterfacesRepositorios;
using RECEPTIO.CapaDominio.Mobile.ServiciosDominio;
using RECEPTIO.CapaDominio.Nucleo.Entidades;

namespace RECEPTIO.CapaDominio.Tests.Mobile.ServicioDominio
{
    [TestClass]
    public class TestServicioPrincipal
    {
        private ServicioPrincipal _servicio;
        private Mock<IRepositorioZone> _mockZone;
        private Mock<IRepositorioDevice> _mockDevice;

        [TestInitialize]
        public void Inicializar()
        {
            _mockZone = new Mock<IRepositorioZone>();
            _mockDevice = new Mock<IRepositorioDevice>();
            _servicio = new ServicioPrincipal(_mockZone.Object, _mockDevice.Object);
        }

        [TestMethod]
        public void TestObtenerZonaConTiposTransaccionesCuandoNoExisteIp()
        {
            _mockZone.Setup(m => m.ObtenerZonasConTipoTransaccion(It.IsAny<FiltroZonaPorIp>())).Returns(new List<ZONE>());
            try
            {
                var resultado = _servicio.ObtenerZonaConTiposTransacciones("");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("No existe zona cuyo ip es ", ex.Message);
                return;
            }
            Assert.Fail("Test Falló.");
        }

        [TestMethod]
        public void TestObtenerZonaConTiposTransaccionesOk()
        {
            _mockZone.Setup(m => m.ObtenerZonasConTipoTransaccion(It.IsAny<FiltroZonaPorIp>())).Returns(new List<ZONE> { new ZONE()});
            var resultado = _servicio.ObtenerZonaConTiposTransacciones("");
            Assert.IsNotNull(resultado);
        }
    }
}

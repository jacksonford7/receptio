using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Repositorio;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using RECEPTIO.CapaDominio.Nucleo.ServiciosDominio.Filtros;
using RECEPTIO.CapaDominio.Transaction.ServiciosDominio;

namespace RECEPTIO.CapaDominio.Tests.Transaction.ServicioDominio
{
    [TestClass]
    public class TestServicioTransaccionQuiosco
    {
        private ServicioTransaccionQuiosco _servicio;
        private Mock<IRepositorioTransaccionQuiosco> _mockTransaccionQuiosco;
        private Mock<IRepositorioPreGate> _mockPreGate;

        [TestInitialize]
        public void Inicializar()
        {
            _mockTransaccionQuiosco = new Mock<IRepositorioTransaccionQuiosco>();
            _mockPreGate = new Mock<IRepositorioPreGate>();
            _servicio = new ServicioTransaccionQuiosco(_mockTransaccionQuiosco.Object, _mockPreGate.Object);
        }

        [TestMethod]
        public void TestRegistrarProcesoCuandoNoExisteTransaccionQuiosco()
        {
            _mockTransaccionQuiosco.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroTransaccionQuioscoPorId>())).Returns(new List<KIOSK_TRANSACTION>());
            var resultado = _servicio.RegistrarProceso(new KIOSK_TRANSACTION());
            Assert.IsFalse(resultado.FueOk);
            Assert.IsTrue(resultado.Mensaje.Contains("No existe transacción de quiosco #"));
        }

        [TestMethod]
        public void TestRegistrarProcesoCuandoNoExisteProceso()
        {
            _mockTransaccionQuiosco.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroTransaccionQuioscoPorId>())).Returns(new List<KIOSK_TRANSACTION> { new KIOSK_TRANSACTION()});
            var resultado = _servicio.RegistrarProceso(new KIOSK_TRANSACTION());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("No existe proceso para la transacción.", resultado.Mensaje);
        }

        [TestMethod]
        public void TestRegistrarProcesoOk()
        {
            _mockTransaccionQuiosco.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroTransaccionQuioscoPorId>())).Returns(new List<KIOSK_TRANSACTION> { new KIOSK_TRANSACTION { PRE_GATE_ID = 0} });
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "P"} });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            var resultado = _servicio.RegistrarProceso(new KIOSK_TRANSACTION { PRE_GATE_ID = 0, PROCESSES = new List<PROCESS> { new PROCESS() }, KIOSK = new KIOSK() });
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("Registro Ok.", resultado.Mensaje);
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Repositorio;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using RECEPTIO.CapaDominio.Transaction.Dominio.InterfacesRepositorios;
using RECEPTIO.CapaDominio.Transaction.ServiciosDominio;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;

namespace RECEPTIO.CapaDominio.Tests.Transaction.ServicioDominio
{
    [TestClass]
    public class TestServicioPreGate
    {
        private ServicioPreGate _servicio;
        private Mock<IRepositorioPreGate> _mockPreGate;
        private Mock<IRepositorioTransaccionQuiosco> _mockTransaccion;
        private Mock<IRepositorioValidaAduana> _mockRepositorioValidaAduana;
        private Mock<IRepositorioValidacionesN4> _mockRepositorioValidacionesN4;
        private Mock<IRepositorioQuiosco> _mockRepositorioQuiosco;

        [TestInitialize]
        public void Inicializar()
        {
            _mockPreGate = new Mock<IRepositorioPreGate>();
            _mockTransaccion = new Mock<IRepositorioTransaccionQuiosco>();
            _mockRepositorioValidaAduana = new Mock<IRepositorioValidaAduana>();
            _mockRepositorioValidacionesN4 = new Mock<IRepositorioValidacionesN4>();
            _mockRepositorioQuiosco = new Mock<IRepositorioQuiosco>();
            _servicio = new ServicioPreGate(_mockPreGate.Object, _mockTransaccion.Object, _mockRepositorioValidaAduana.Object, _mockRepositorioValidacionesN4.Object, _mockRepositorioQuiosco.Object);
        }

        [TestMethod]
        public void TestValidarPreGateCuandoNoExisteCedulaONoEstaConEstatusN()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorCedula>())).Returns(new List<PRE_GATE>());
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
             var resultado = _servicio.ValidarPreGate("", 0, new Dictionary<short, bool>());
             Assert.IsFalse(resultado.FueOk);
             Assert.AreEqual("1", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarPreGateCuandoTieneMultiplesGates()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorCedula>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ STATUS = "N" },
                new PRE_GATE{ STATUS = "N" }
            });
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarPreGate("", 0, new Dictionary<short, bool>());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("2", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarPreGateCuandoExisteErrorTransaccionesNoContempladas()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorCedula>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 88} } }
            });
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarPreGate("", 0, new Dictionary<short, bool>());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("24", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarPreGateCuandoExisteErrorConcurrenciaEnImpo()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorCedula>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 1} } }
            });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>())).Throws(new DbUpdateConcurrencyException());
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarPreGate("", 0, new Dictionary<short, bool>());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("3", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarPreGateOkImpo()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorCedula>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 1} } }
            });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarPreGate("", 0, new Dictionary<short, bool>());
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("4", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarPreGateCuandoExisteErrorConcurrenciaEnImpoBrBk()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorCedula>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 2} } }
            });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>())).Throws(new DbUpdateConcurrencyException());
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarPreGate("", 0, new Dictionary<short, bool>());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("3", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarPreGateOkImpoBrBk()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorCedula>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 2} } }
            });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarPreGate("", 0, new Dictionary<short, bool>());
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("4", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarPreGateCuandoNoExisteContenedorEntradaExpo()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorCedula>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 3, CONTAINERS = new List<CONTAINER>()} } }
            });
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarPreGate("", 0, new Dictionary<short, bool>());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("21", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarPreGateCuandoExisteErrorConcurrenciaEnExpo()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorCedula>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 3, CONTAINERS = new List<CONTAINER> { new CONTAINER()} } } }
            });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>())).Throws(new DbUpdateConcurrencyException());
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarPreGate("", 0, new Dictionary<short, bool>());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("3", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarPreGateOkExpo()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorCedula>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 3, CONTAINERS = new List<CONTAINER> { new CONTAINER()} } } }
            });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarPreGate("", 0, new Dictionary<short, bool>());
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("4", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarPreGateCuandoNoExisteContenedorEntradaRecepcionVacios()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorCedula>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 7, CONTAINERS = new List<CONTAINER>()} } }
            });
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarPreGate("", 0, new Dictionary<short, bool>());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("21", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarPreGateCuandoExisteErrorConcurrenciaEnRecepcionVacios()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorCedula>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 7, CONTAINERS = new List<CONTAINER> { new CONTAINER()} } } }
            });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>())).Throws(new DbUpdateConcurrencyException());
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarPreGate("", 0, new Dictionary<short, bool>());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("3", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarPreGateOkRecepcionVacios()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorCedula>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 7, CONTAINERS = new List<CONTAINER> { new CONTAINER()} } } }
            });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarPreGate("", 0, new Dictionary<short, bool>());
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("4", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarPreGateCuandoNoHayErrorEnPesaje()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorCedula>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ DRIVER_ID = "<>//>>", PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 4} } }
            });
            _mockRepositorioQuiosco.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroQuioscoPorId>())).Returns(new List<KIOSK> { new KIOSK() });
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarPreGate("", 0, new Dictionary<short, bool>());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("31", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarPreGateCuandoExisteErrorConcurrenciaEnExpoBrBk()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorCedula>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{  DRIVER_ID = "1234657890", PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 4} } }
            });
            _mockRepositorioQuiosco.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroQuioscoPorId>())).Returns(new List<KIOSK> { new KIOSK { NAME = "Kiosco 666", IP = "172.16.2.47" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>())).Throws(new DbUpdateConcurrencyException());
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarPreGate("", 0, new Dictionary<short, bool>());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("3", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarPreGateOkExpoBrBK()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorCedula>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 4 } } }
            });
            _mockRepositorioQuiosco.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroQuioscoPorId>())).Returns(new List<KIOSK> { new KIOSK { NAME = "Kiosco 666", IP = "172.16.2.47" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarPreGate("", 0, new Dictionary<short, bool>());
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("4", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarPreGateCuandoNoHayErrorEnPesajeCfs()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorCedula>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ DRIVER_ID = "<>//>>", PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 5} } }
            });
            _mockRepositorioQuiosco.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroQuioscoPorId>())).Returns(new List<KIOSK> { new KIOSK() });
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarPreGate("", 0, new Dictionary<short, bool>());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("31", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarPreGateCuandoExisteErrorConcurrenciaEnExpoCfs()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorCedula>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{  DRIVER_ID = "1234657890", PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 5} } }
            });
            _mockRepositorioQuiosco.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroQuioscoPorId>())).Returns(new List<KIOSK> { new KIOSK { NAME = "Kiosco 666", IP = "172.16.2.47" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>())).Throws(new DbUpdateConcurrencyException());
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarPreGate("", 0, new Dictionary<short, bool>());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("3", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarPreGateOkExpoCfs()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorCedula>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 5 } } }
            });
            _mockRepositorioQuiosco.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroQuioscoPorId>())).Returns(new List<KIOSK> { new KIOSK { NAME = "Kiosco 666", IP = "172.16.2.47" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarPreGate("", 0, new Dictionary<short, bool>());
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("4", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarPreGateCuandoExisteErrorConcurrenciaEnExpoBanano()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorCedula>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{  DRIVER_ID = "1234657890", PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 8} } }
            });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>())).Throws(new DbUpdateConcurrencyException());
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarPreGate("", 0, new Dictionary<short, bool>());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("3", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarPreGateCuandoExisteErrorConcurrenciaEnExpoBananoCfs()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorCedula>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{  DRIVER_ID = "1234657890", PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 9} } }
            });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>())).Throws(new DbUpdateConcurrencyException());
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarPreGate("", 0, new Dictionary<short, bool>());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("3", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarPreGateOkExpoBanano()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorCedula>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 8 } } }
            });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarPreGate("", 0, new Dictionary<short, bool>());
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("4", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarPreGateOkExpoBananoCfs()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorCedula>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 9 } } }
            });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarPreGate("", 0, new Dictionary<short, bool>());
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("4", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarPreGateCuandoNoExisteContenedorEntradaMixtaVacios()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorCedula>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 10, REFERENCE_ID = "E", CONTAINERS = new List<CONTAINER>()} } }
            });
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarPreGate("", 0, new Dictionary<short, bool>());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("21", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarPreGateCuandoExisteErrorConcurrenciaEnMixtaVacios()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorCedula>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 10, REFERENCE_ID = "E", CONTAINERS = new List<CONTAINER> { new CONTAINER()} } } }
            });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>())).Throws(new DbUpdateConcurrencyException());
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarPreGate("", 0, new Dictionary<short, bool>());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("3", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarPreGateOkMixtaVacios()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorCedula>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 10, REFERENCE_ID = "E", CONTAINERS = new List<CONTAINER> { new CONTAINER()} } } }
            });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarPreGate("", 0, new Dictionary<short, bool>());
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("4", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarPreGateCuandoExisteErrorConcurrenciaEnEntregaVacios()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorCedula>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 11 } } }
            });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>())).Throws(new DbUpdateConcurrencyException());
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarPreGate("", 0, new Dictionary<short, bool>());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("3", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarPreGateOkEntregaVacios()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorCedula>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 11 } } }
            });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarPreGate("", 0, new Dictionary<short, bool>());
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("4", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarPreGateCuandoNoHayErrorEnPesajeProveedores()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorCedula>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ DRIVER_ID = "<>//>>", PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 12} } }
            });
            _mockRepositorioQuiosco.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroQuioscoPorId>())).Returns(new List<KIOSK> { new KIOSK() });
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarPreGate("", 0, new Dictionary<short, bool>());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("31", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarPreGateCuandoExisteErrorConcurrenciaEnProveedores()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorCedula>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{  DRIVER_ID = "1234657890", PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 13} } }
            });
            _mockRepositorioQuiosco.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroQuioscoPorId>())).Returns(new List<KIOSK> { new KIOSK { NAME = "Kiosco 666", IP = "172.16.2.47" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>())).Throws(new DbUpdateConcurrencyException());
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarPreGate("", 0, new Dictionary<short, bool>());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("3", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarPreGateOkProveedores()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorCedula>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 14 } } }
            });
            _mockRepositorioQuiosco.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroQuioscoPorId>())).Returns(new List<KIOSK> { new KIOSK { NAME = "Kiosco 666", IP = "172.16.2.47" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarPreGate("", 0, new Dictionary<short, bool>());
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("4", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarEntradaQuioscoCuandoNoExisteCodigo()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE>());
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarEntradaQuiosco(0, 0, new Dictionary<short, bool>());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("18", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarEntradaQuioscoCuandoYaTieneSalidaTerminal()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ STATUS = "O" }
            });
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarEntradaQuiosco(0, 0, new Dictionary<short, bool>());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("19", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarEntradaQuioscoCuandoNoExisteEntradaTerminal()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ STATUS = "N" }
            });
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarEntradaQuiosco(0, 0, new Dictionary<short, bool>());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("20", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarEntradaQuioscoCuandoNoExisteContenedorEnImpo()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ STATUS = "I", PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 1} } }
            });
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarEntradaQuiosco(0, 0, new Dictionary<short, bool>());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("21", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarEntradaQuioscoCuandoTieneMensajeAduanaImpo()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ STATUS = "I", PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 1, CONTAINERS = new List<CONTAINER> { new CONTAINER()} } } }
            });
            _mockRepositorioValidaAduana.Setup(m => m.ValidaSmdt(It.IsAny<string>())).Returns(false);
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarEntradaQuiosco(0, 0, new Dictionary<short, bool>());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("22", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarEntradaQuioscoCuandoExisteErrorConcurrenciaImpo()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ STATUS = "I", PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 1, CONTAINERS = new List<CONTAINER> { new CONTAINER()} } } }
            });
            _mockRepositorioValidaAduana.Setup(m => m.ValidaSmdt(It.IsAny<string>())).Returns(true);
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>())).Throws(new DbUpdateConcurrencyException());
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarEntradaQuiosco(0, 0, new Dictionary<short, bool>());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("3", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarEntradaQuioscoOkImpo()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ STATUS = "L", KIOSK_TRANSACTIONS = new List<KIOSK_TRANSACTION>{ new KIOSK_TRANSACTION { KIOSK_ID = 1} }, PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 1, CONTAINERS = new List<CONTAINER> { new CONTAINER()} } } }
            });
            _mockRepositorioValidaAduana.Setup(m => m.ValidaSmdt(It.IsAny<string>())).Returns(true);
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarEntradaQuiosco(0, 1, new Dictionary<short, bool>());
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("4", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarEntradaQuioscoCuandoTieneMensajeAduanaImpoBrBk()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ STATUS = "I", PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 2 } } }
            });
            _mockRepositorioValidaAduana.Setup(m => m.ValidaSmdt(It.IsAny<string>())).Returns(false);
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarEntradaQuiosco(0, 0, new Dictionary<short, bool>());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("22", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarEntradaQuioscoCuandoExisteErrorConcurrenciaImpoBrBk()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ STATUS = "I", PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 2 } } }
            });
            _mockRepositorioValidaAduana.Setup(m => m.ValidaSmdt(It.IsAny<string>())).Returns(true);
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>())).Throws(new DbUpdateConcurrencyException());
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarEntradaQuiosco(0, 0, new Dictionary<short, bool>());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("3", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarEntradaQuioscoOkImpoBrBk()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ STATUS = "L", KIOSK_TRANSACTIONS = new List<KIOSK_TRANSACTION>{ new KIOSK_TRANSACTION { KIOSK_ID = 1} }, PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 2 } } }
            });
            _mockRepositorioValidaAduana.Setup(m => m.ValidaSmdt(It.IsAny<string>())).Returns(true);
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarEntradaQuiosco(0, 1, new Dictionary<short, bool>());
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("4", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarEntradaQuioscoCuandoContenedorNoEstaEnPatio()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ STATUS = "I", PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 3, CONTAINERS = new List<CONTAINER> { new CONTAINER { NUMBER = "XXX"} } } } }
            });
            _mockRepositorioValidacionesN4.Setup(m => m.EstaEnPatioContenedor(It.IsAny<string>())).Returns(false);
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarEntradaQuiosco(0, 0, new Dictionary<short, bool>());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("26", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarEntradaQuioscoCuandoExisteErrorConcurrenciaExpo()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ STATUS = "I", PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 3, CONTAINERS = new List<CONTAINER> { new CONTAINER { NUMBER = "XXX"} } } } }
            });
            _mockRepositorioValidacionesN4.Setup(m => m.EstaEnPatioContenedor(It.IsAny<string>())).Returns(true);
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>())).Throws(new DbUpdateConcurrencyException());
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarEntradaQuiosco(0, 0, new Dictionary<short, bool>());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("3", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarEntradaQuioscoOkExpo()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ STATUS = "L", KIOSK_TRANSACTIONS = new List<KIOSK_TRANSACTION>{ new KIOSK_TRANSACTION { KIOSK_ID = 1} }, PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 3, CONTAINERS = new List<CONTAINER> { new CONTAINER { NUMBER = "XXX" } } } } }
            });
            _mockRepositorioValidacionesN4.Setup(m => m.EstaEnPatioContenedor(It.IsAny<string>())).Returns(true);
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarEntradaQuiosco(0, 1, new Dictionary<short, bool>());
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("4", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarEntradaQuioscoCuandoNoExistePesaje()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ DRIVER_ID = "<>//>>",  STATUS = "I", PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 4 } } }
            });
            _mockRepositorioQuiosco.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroQuioscoPorId>())).Returns(new List<KIOSK> { new KIOSK() });
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarEntradaQuiosco(0, 0, new Dictionary<short, bool>());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("31", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarEntradaQuioscoCuandoExisteErrorConcurrenciaExpoBrBk()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ DRIVER_ID = "1234657890", STATUS = "I", PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 4 } } }
            });
            _mockRepositorioQuiosco.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroQuioscoPorId>())).Returns(new List<KIOSK> { new KIOSK() });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>())).Throws(new DbUpdateConcurrencyException());
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarEntradaQuiosco(0, 0, new Dictionary<short, bool>());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("3", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarEntradaQuioscoOkExpoBrBk()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ DRIVER_ID = "1234657890", STATUS = "L", KIOSK_TRANSACTIONS = new List<KIOSK_TRANSACTION>{ new KIOSK_TRANSACTION { KIOSK_ID = 1} }, PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 4 } } }
            });
            _mockRepositorioQuiosco.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroQuioscoPorId>())).Returns(new List<KIOSK> { new KIOSK() });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarEntradaQuiosco(0, 1, new Dictionary<short, bool>());
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("4", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarEntradaQuioscoCuandoExisteErrorConcurrenciaExpoCfs()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ DRIVER_ID = "1234657890", STATUS = "I", PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 5 } } }
            });
            _mockRepositorioQuiosco.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroQuioscoPorId>())).Returns(new List<KIOSK> { new KIOSK() });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>())).Throws(new DbUpdateConcurrencyException());
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarEntradaQuiosco(0, 0, new Dictionary<short, bool>());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("3", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarEntradaQuioscoOkExpoCfs()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ DRIVER_ID = "1234657890", STATUS = "L", KIOSK_TRANSACTIONS = new List<KIOSK_TRANSACTION>{ new KIOSK_TRANSACTION { KIOSK_ID = 1} }, PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 5 } } }
            });
            _mockRepositorioQuiosco.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroQuioscoPorId>())).Returns(new List<KIOSK> { new KIOSK() });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarEntradaQuiosco(0, 1, new Dictionary<short, bool>());
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("4", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarEntradaQuioscoCuandoExisteErrorConcurrenciaExpoBanano()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ DRIVER_ID = "1234657890", STATUS = "I", PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 8 } } }
            });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>())).Throws(new DbUpdateConcurrencyException());
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarEntradaQuiosco(0, 0, new Dictionary<short, bool>());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("3", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarEntradaQuioscoCuandoExisteErrorConcurrenciaExpoBananoCfs()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ DRIVER_ID = "1234657890", STATUS = "I", PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 9 } } }
            });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>())).Throws(new DbUpdateConcurrencyException());
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarEntradaQuiosco(0, 0, new Dictionary<short, bool>());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("3", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarEntradaQuioscoOkExpoBanano()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ DRIVER_ID = "1234657890", STATUS = "L", KIOSK_TRANSACTIONS = new List<KIOSK_TRANSACTION>{ new KIOSK_TRANSACTION { KIOSK_ID = 1} }, PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 8 } } }
            });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarEntradaQuiosco(0, 1, new Dictionary<short, bool>());
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("4", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarEntradaQuioscoOkExpoBananoCfs()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ DRIVER_ID = "1234657890", STATUS = "L", KIOSK_TRANSACTIONS = new List<KIOSK_TRANSACTION>{ new KIOSK_TRANSACTION { KIOSK_ID = 1} }, PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 9 } } }
            });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarEntradaQuiosco(0, 1, new Dictionary<short, bool>());
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("4", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarEntradaQuioscoCuandoTieneMensajeAduanaMixtaVacios()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ STATUS = "I", PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 11, CONTAINERS = new List<CONTAINER> { new CONTAINER()} }, new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 11, CONTAINERS = new List<CONTAINER> { new CONTAINER() } } } }
            });
            _mockRepositorioValidaAduana.Setup(m => m.ValidaSmdt(It.IsAny<string>())).Returns(false);
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarEntradaQuiosco(0, 0, new Dictionary<short, bool>());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("22", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarEntradaQuioscoCuandoContenedorNoEstaEnPatioMixtaVacios()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ STATUS = "I", PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 10, REFERENCE_ID = "E", CONTAINERS = new List<CONTAINER> { new CONTAINER()} }, new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 10, STATUS = "N", REFERENCE_ID = "I", CONTAINERS = new List<CONTAINER> { new CONTAINER() } } } }
            });
            _mockRepositorioValidacionesN4.Setup(m => m.EstaEnPatioContenedor(It.IsAny<string>())).Returns(false);
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarEntradaQuiosco(0, 0, new Dictionary<short, bool>());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("26", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarEntradaQuioscoCuandoExisteErrorConcurrenciaMixtaVaciosReciboVacio()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ STATUS = "I", PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 10, REFERENCE_ID = "E", CONTAINERS = new List<CONTAINER> { new CONTAINER()} }, new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 10, REFERENCE_ID = "I", CONTAINERS = new List<CONTAINER> { new CONTAINER() } } } }
            });
            _mockRepositorioValidacionesN4.Setup(m => m.EstaEnPatioContenedor(It.IsAny<string>())).Returns(true);
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>())).Throws(new DbUpdateConcurrencyException());
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarEntradaQuiosco(0, 0, new Dictionary<short, bool>());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("3", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarEntradaQuioscoOkMixtaVaciosReciboVacio()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ STATUS = "I", PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 10, REFERENCE_ID = "E", CONTAINERS = new List<CONTAINER> { new CONTAINER()} }, new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 10, REFERENCE_ID = "I", CONTAINERS = new List<CONTAINER> { new CONTAINER() } } } }
            });
            _mockRepositorioValidacionesN4.Setup(m => m.EstaEnPatioContenedor(It.IsAny<string>())).Returns(true);
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarEntradaQuiosco(0, 0, new Dictionary<short, bool>());
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("4", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarEntradaQuioscoCuandoExisteErrorConcurrenciaMixtaVaciosEntregaVacio()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ STATUS = "I", PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 11, CONTAINERS = new List<CONTAINER> { new CONTAINER()} }, new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 11, CONTAINERS = new List<CONTAINER> { new CONTAINER() } } } }
            });
            _mockRepositorioValidaAduana.Setup(m => m.ValidaSmdt(It.IsAny<string>())).Returns(true);
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>())).Throws(new DbUpdateConcurrencyException());
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarEntradaQuiosco(0, 0, new Dictionary<short, bool>());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("3", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarEntradaQuioscoOkMixtaVaciosEntregaVacio()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ STATUS = "I", PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 11, CONTAINERS = new List<CONTAINER> { new CONTAINER()} }, new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 11, CONTAINERS = new List<CONTAINER> { new CONTAINER() } } } }
            });
            _mockRepositorioValidaAduana.Setup(m => m.ValidaSmdt(It.IsAny<string>())).Returns(true);
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarEntradaQuiosco(0, 0, new Dictionary<short, bool>());
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("4", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarEntradaQuioscoCuandoNoExistePesajeProveedores()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ DRIVER_ID = "<>//>>",  STATUS = "I", PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 15 } } }
            });
            _mockRepositorioQuiosco.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroQuioscoPorId>())).Returns(new List<KIOSK> { new KIOSK() });
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarEntradaQuiosco(0, 0, new Dictionary<short, bool>());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("31", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarEntradaQuioscoCuandoExisteErrorConcurrenciaProveedores()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ DRIVER_ID = "1234657890", STATUS = "I", PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 12 } } }
            });
            _mockRepositorioQuiosco.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroQuioscoPorId>())).Returns(new List<KIOSK> { new KIOSK() });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>())).Throws(new DbUpdateConcurrencyException());
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarEntradaQuiosco(0, 0, new Dictionary<short, bool>());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("3", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarEntradaQuioscoOkProveedores()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE>
            {
                new PRE_GATE{ DRIVER_ID = "1234657890", STATUS = "L", KIOSK_TRANSACTIONS = new List<KIOSK_TRANSACTION>{ new KIOSK_TRANSACTION { KIOSK_ID = 1} }, PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 13 } } }
            });
            _mockRepositorioQuiosco.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroQuioscoPorId>())).Returns(new List<KIOSK> { new KIOSK() });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            _mockTransaccion.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.ValidarEntradaQuiosco(0, 1, new Dictionary<short, bool>());
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("4", resultado.Mensaje);
        }
    }
}

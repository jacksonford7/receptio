using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RECEPTIO.CapaDominio.Console.Dominio.InterfacesRepositorios;
using RECEPTIO.CapaDominio.Console.ServiciosDominio;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using System.Linq;
using RECEPTIO.CapaAplicacion.Console.Aplicacion.Contratos;

namespace RECEPTIO.CapaDominio.Tests.Console.ServicioDominio
{
    [TestClass]
    public class TestServicioTroubleDesk
    {
        private ServicioTroubleDesk _servicio;
        private Mock<IRepositorioUserSession> _mockUserSession;
        private Mock<IRepositorioKioskTransaction> _mockKioskTransaction;
        private Mock<IRepositorioTroubleTicket> _mockTroubleTicket;
        private Mock<IRepositorioAutoTroubleReason> _mockAutoTroubleReason;
        private Mock<IRepositorioAction> _mockAction;
        private Mock<IRepositorioAduana> _mockAduana;
        private Mock<IRepositorioBreakType> _mockBreakType;
        private Mock<IRepositorioBreak> _mockBreak;

        [TestInitialize]
        public void Inicializar()
        {
            _mockUserSession = new Mock<IRepositorioUserSession>();
            _mockKioskTransaction = new Mock<IRepositorioKioskTransaction>();
            _mockTroubleTicket = new Mock<IRepositorioTroubleTicket>();
            _mockAutoTroubleReason = new Mock<IRepositorioAutoTroubleReason>();
            _mockAction = new Mock<IRepositorioAction>();
            _mockAduana = new Mock<IRepositorioAduana>();
            _mockBreakType = new Mock<IRepositorioBreakType>();
            _mockBreak = new Mock<IRepositorioBreak>();
            _servicio = new ServicioTroubleDesk(_mockUserSession.Object, _mockKioskTransaction.Object, _mockTroubleTicket.Object, _mockAutoTroubleReason.Object, _mockAction.Object, _mockAduana.Object, _mockBreakType.Object, _mockBreak.Object);
        }

        [TestMethod]
        public void TestObtenerTicketsCuandoNoExisteSessionUsuario()
        {
            try
            {
                _mockUserSession.Setup(m => m.ObtenerSesionUsuarioConTickets(It.IsAny<FiltroSesionUsuarioConTickets>())).Returns(new List<USER_SESSION>());
                var resultado = _servicio.ObtenerTickets(0);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("No existe sesión de usuario con id : 0", ex.Message);
                return;
            }
            Assert.Fail("Falló el test.");
        }

        [TestMethod]
        public void TestObtenerTicketsOk()
        {
            _mockUserSession.Setup(m => m.ObtenerSesionUsuarioConTickets(It.IsAny<FiltroSesionUsuarioConTickets>())).Returns(new List<USER_SESSION>{ new USER_SESSION() });
            var resultado = _servicio.ObtenerTickets(0);
            Assert.AreEqual(0, resultado.Count());
        }

        [TestMethod]
        public void TestObtenerDetallesTicketCuandoNoExisteTransaccionQuiosco()
        {
            try
            {
                _mockKioskTransaction.Setup(m => m.ObtenerTransacionQuioscoConProcesosYDatos(It.IsAny<FiltroTransaccionQuioscoPorId>())).Returns(new List<KIOSK_TRANSACTION>());
                var resultado = _servicio.ObtenerDetallesTicket(0);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("No existe transaccion quiosco con id : 0", ex.Message);
                return;
            }
            Assert.Fail("Falló el test.");
        }

        [TestMethod]
        public void TestObtenerDetallesTicketOk()
        {
            _mockKioskTransaction.Setup(m => m.ObtenerTransacionQuioscoConProcesosYDatos(It.IsAny<FiltroTransaccionQuioscoPorId>())).Returns(new List<KIOSK_TRANSACTION> { new KIOSK_TRANSACTION
            {
                PRE_GATE = new PRE_GATE
                {
                    DRIVER_ID = "",
                    TRUCK_LICENCE = "",
                    PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { CONTAINERS = new List<CONTAINER> { new CONTAINER { NUMBER = "XXXJKLjkjkl"} }, TRANSACTION_TYPE = new TRANSACTION_TYPE { DESCRIPTION = "" } } }
                }
            } });
            var resultado = _servicio.ObtenerDetallesTicket(0);
            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public void TestAceptarTicketCuandoNoExisteId()
        {
            _mockTroubleTicket.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroTroubleTicketPoId>())).Returns(new List<TROUBLE_TICKET>());
            try
            {
                _servicio.AceptarTicket(0);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("No existe ticket cuyo id es 0", ex.Message);
                return;
            }
            Assert.Fail("Test Falló.");
        }

        [TestMethod]
        public void TestAceptarTicketOk()
        {
            _mockTroubleTicket.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroTroubleTicketPoId>())).Returns(new List<TROUBLE_TICKET> { new TROUBLE_TICKET()});
            _mockTroubleTicket.Setup(m => m.Actualizar(It.IsAny<TROUBLE_TICKET>()));
            _servicio.AceptarTicket(0);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestCerrarTicketCuandoNoExisteId()
        {
            _mockTroubleTicket.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroTroubleTicketPoId>())).Returns(new List<TROUBLE_TICKET>());
            try
            {
                _servicio.CerrarTicket(0, "",0,0);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("No existe ticket cuyo id es 0", ex.Message);
                return;
            }
            Assert.Fail("Test Falló.");
        }

        [TestMethod]
        public void TestCerrarTicketOk()
        {
            _mockTroubleTicket.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroTroubleTicketPoId>())).Returns(new List<TROUBLE_TICKET> { new TROUBLE_TICKET() });
            _mockTroubleTicket.Setup(m => m.Actualizar(It.IsAny<TROUBLE_TICKET>()));
            _servicio.CerrarTicket(0, "",0,0);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestObtenerMotivosAutoTickets()
        {
            _mockAutoTroubleReason.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroAutoTicketReasonActivos>())).Returns(new List<AUTO_TROUBLE_REASON> { new AUTO_TROUBLE_REASON() });
            var resultado = _servicio.ObtenerMotivosAutoTickets();
            Assert.AreEqual(1, resultado.Count());
        }

        [TestMethod]
        public void TestCrearAutoTicket()
        {
            _mockTroubleTicket.Setup(m => m.Agregar(It.IsAny<TROUBLE_TICKET>()));
            _servicio.CrearAutoTicket(0, 0);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestRegistrarAccion()
        {
            _mockAction.Setup(m => m.Agregar(It.IsAny<ACTION>()));
            _servicio.RegistrarAccion(new ACTION());
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestObtenerMensajesSmdtAduana()
        {
            _mockAduana.Setup(m => m.ObtenerMensajesSmdtAduana(It.IsAny<string>())).Returns(new List<mb_get_ecuapass_message_pass_Result>());
            var resultado = _servicio.ObtenerMensajesSmdtAduana("");
            Assert.IsTrue(resultado.Count() == 0);
        }

        [TestMethod]
        public void TestCambiarEstadoSmdt()
        {
            _mockAduana.Setup(m => m.CambiarEstadoSmdt(It.IsAny<string>(), It.IsAny<string>())).Returns(5);
            var resultado = _servicio.CambiarEstadoSmdt("", "");
            Assert.AreEqual(5, resultado.Value);
        }

        [TestMethod]
        public void TestAgregarTransaccionManual()
        {
            _mockAduana.Setup(m => m.AgregarTransaccionManual(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new mb_add_ecuapass_transaccion_Result());
            var resultado = _servicio.AgregarTransaccionManual(new DatosTransaccionManual());
            Assert.AreEqual(false, resultado.code.HasValue);
        }

        [TestMethod]
        public void TestSuspenderTicketCuandoNoExisteId()
        {
            _mockTroubleTicket.Setup(m => m.ObtenerTicketProcesos(It.IsAny<FiltroTroubleTicketProcesoPoId>())).Returns(new List<PROCESS_TROUBLE_TICKET>());
            try
            {
                _servicio.SuspenderTicket(0);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("No existe ticket cuyo id es 0", ex.Message);
                return;
            }
            Assert.Fail("Test Falló.");
        }

        [TestMethod]
        public void TestSuspenderTicketOk()
        {
            _mockTroubleTicket.Setup(m => m.ObtenerTicketProcesos(It.IsAny<FiltroTroubleTicketProcesoPoId>())).Returns(new List<PROCESS_TROUBLE_TICKET> { new PROCESS_TROUBLE_TICKET() });
            _mockTroubleTicket.Setup(m => m.Actualizar(It.IsAny<TROUBLE_TICKET>()));
            _servicio.SuspenderTicket(0);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestObtenerTiposDescansos()
        {
            _mockBreakType.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroBreakType>())).Returns(new List<BREAK_TYPE> { new BREAK_TYPE() });
            var resultado = _servicio.ObtenerTiposDescansos();
            Assert.AreEqual(1, resultado.Count());
        }

        [TestMethod]
        public void TestRegistrarDescanso()
        {
            _mockBreak.Setup(m => m.Agregar(It.IsAny<BREAK>()));
            var resultado = _servicio.RegistrarDescanso(new BREAK());
            Assert.AreEqual(0, resultado);
        }

        [TestMethod]
        public void TestFinalizarDescanso()
        {
            _mockBreak.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroBreakPorId>())).Returns(new List<BREAK> { new BREAK() });
            _mockBreak.Setup(m => m.Actualizar(It.IsAny<BREAK>()));
            _servicio.FinalizarDescanso(0);
            Assert.IsTrue(true);
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RECEPTIO.CapaDominio.Console.Dominio.InterfacesRepositorios;
using RECEPTIO.CapaDominio.Console.ServiciosDominio;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Repositorio;
using RECEPTIO.CapaDominio.Nucleo.Entidades;

namespace RECEPTIO.CapaDominio.Tests.Console.ServicioDominio
{
    [TestClass]
    public class TestServicioProblema
    {
        private ServicioProblema _servicio;
        private Mock<IRepositorioProcess> _mockProcess;
        private Mock<IRepositorioUserSession> _mockUserSession;
        private Mock<IRepositorioTroubleTicket> _mockTroubleTicket;
        private Mock<IRepositorioError> _mockError;

        [TestInitialize]
        public void Inicializar()
        {
            _mockProcess = new Mock<IRepositorioProcess>();
            _mockUserSession = new Mock<IRepositorioUserSession>();
            _mockTroubleTicket = new Mock<IRepositorioTroubleTicket>();
            _mockError = new Mock<IRepositorioError>();
            _servicio = new ServicioProblema(_mockProcess.Object, _mockUserSession.Object, _mockTroubleTicket.Object, _mockError.Object);
        }

        [TestMethod]
        public void TestRegistrarProblemaCuandoNoExisteProceso()
        {
            try
            {
                _mockProcess.Setup(m => m.ObtenerProcesoConMensaje(It.IsAny<FiltroProcesoPorIdTransaccion>())).Returns(new List<PROCESS>());
                var resultado = _servicio.RegistrarProblema(0);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("No existe proceso cuyo id de transacción es : 0", ex.Message);
                return;
            }
            Assert.Fail("Falló el test.");
        }

        [TestMethod]
        public void TestRegistrarProblemaOk0Sesiones()
        {
            _mockProcess.Setup(m => m.ObtenerProcesoConMensaje(It.IsAny<FiltroProcesoPorIdTransaccion>())).Returns(new List<PROCESS> { new PROCESS { MESSAGE = new MESSAGE { TROUBLE_DESK_MESSAGE = "HELLO" }, KIOSK_TRANSACTION = new KIOSK_TRANSACTION { KIOSK = new KIOSK() } } });
            _mockUserSession.Setup(m => m.ObtenerSesionUsuarioConDispositivo(It.IsAny<FiltroSesionUsuarioAbiertas>())).Returns(new List<USER_SESSION>());
            _mockTroubleTicket.Setup(m => m.Agregar(It.IsAny<PROCESS_TROUBLE_TICKET>()));
            var resultado = _servicio.RegistrarProblema(0);
            Assert.AreEqual(false, resultado.Item1);
            Assert.AreEqual("", resultado.Item2);
            Assert.AreEqual("", resultado.Item3);
        }

        [TestMethod]
        public void TestRegistrarProblemaOk1Sesion()
        {
            _mockProcess.Setup(m => m.ObtenerProcesoConMensaje(It.IsAny<FiltroProcesoPorIdTransaccion>())).Returns(new List<PROCESS> { new PROCESS { MESSAGE = new MESSAGE { TROUBLE_DESK_MESSAGE = "HELLO" }, KIOSK_TRANSACTION = new KIOSK_TRANSACTION { KIOSK = new KIOSK() } } });
            _mockUserSession.Setup(m => m.ObtenerSesionUsuarioConDispositivo(It.IsAny<FiltroSesionUsuarioAbiertas>())).Returns(new List<USER_SESSION> { new USER_SESSION { DEVICE = new DEVICE { IP = "123456" } } });
            _mockTroubleTicket.Setup(m => m.Agregar(It.IsAny<PROCESS_TROUBLE_TICKET>()));
            var resultado = _servicio.RegistrarProblema(0);
            Assert.AreEqual(true, resultado.Item1);
            Assert.AreEqual("HELLO", resultado.Item2);
            Assert.AreEqual("123456", resultado.Item3);
        }

        [TestMethod]
        public void TestRegistrarProblemaOk2Sesiones()
        {
            _mockProcess.Setup(m => m.ObtenerProcesoConMensaje(It.IsAny<FiltroProcesoPorIdTransaccion>())).Returns(new List<PROCESS> { new PROCESS { MESSAGE = new MESSAGE { TROUBLE_DESK_MESSAGE = "HELLO" }, KIOSK_TRANSACTION = new KIOSK_TRANSACTION { KIOSK = new KIOSK() } } });
            _mockUserSession.Setup(m => m.ObtenerSesionUsuarioConDispositivo(It.IsAny<FiltroSesionUsuarioAbiertas>())).Returns(new List<USER_SESSION>
            {
                new USER_SESSION { DEVICE_ID = 1, DEVICE = new DEVICE { IP = "123456" }, PROCESS_TROUBLE_TICKETS = new List<PROCESS_TROUBLE_TICKET>{ new PROCESS_TROUBLE_TICKET()} },
                new USER_SESSION { DEVICE = new DEVICE { IP = "987654" }, PROCESS_TROUBLE_TICKETS = new List<PROCESS_TROUBLE_TICKET>() }
            });
            _mockTroubleTicket.Setup(m => m.Agregar(It.IsAny<PROCESS_TROUBLE_TICKET>()));
            var resultado = _servicio.RegistrarProblema(0);
            Assert.AreEqual(true, resultado.Item1);
            Assert.AreEqual("HELLO", resultado.Item2);
            Assert.AreEqual("987654", resultado.Item3);
        }

        [TestMethod]
        public void TestRegistrarProblemaClienteAppTransactionCuandoNoExisteError()
        {
            try
            {
                _mockError.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroErrorPorId>())).Returns(new List<ERROR>());
                var resultado = _servicio.RegistrarProblemaClienteAppTransaction(0, 0);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("No existe error cuyo id es : 0", ex.Message);
                return;
            }
            Assert.Fail("Falló el test.");
        }

        [TestMethod]
        public void TestRegistrarProblemaClienteAppTransactionOk0Sesiones()
        {
            _mockError.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroErrorPorId>())).Returns(new List<ERROR> { new ERROR { MESSAGE = "HI"} });
            _mockUserSession.Setup(m => m.ObtenerSesionUsuarioConDispositivo(It.IsAny<FiltroSesionUsuarioAbiertas>())).Returns(new List<USER_SESSION>());
            _mockTroubleTicket.Setup(m => m.CrearTicketAppCliente(It.IsAny<CLIENT_APP_TRANSACTION_TROUBLE_TICKET>(), It.IsAny<int>()));
            var resultado = _servicio.RegistrarProblemaClienteAppTransaction(1, 0);
            Assert.AreEqual(false, resultado.Item1);
            Assert.AreEqual("", resultado.Item2);
            Assert.AreEqual("", resultado.Item3);
        }

        [TestMethod]
        public void TestRegistrarProblemaClienteAppTransactionOk1Sesion()
        {
            _mockError.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroErrorPorId>())).Returns(new List<ERROR> { new ERROR { MESSAGE = "HI" } });
            _mockUserSession.Setup(m => m.ObtenerSesionUsuarioConDispositivo(It.IsAny<FiltroSesionUsuarioAbiertas>())).Returns(new List<USER_SESSION> { new USER_SESSION { DEVICE = new DEVICE { IP = "123456" } } });
            _mockTroubleTicket.Setup(m => m.CrearTicketAppCliente(It.IsAny<CLIENT_APP_TRANSACTION_TROUBLE_TICKET>(), It.IsAny<int>()));
            var resultado = _servicio.RegistrarProblemaClienteAppTransaction(1, 0);
            Assert.AreEqual(true, resultado.Item1);
            Assert.AreEqual("HI", resultado.Item2);
            Assert.AreEqual("123456", resultado.Item3);
        }

        [TestMethod]
        public void TestRegistrarProblemaClienteAppTransactionOk2Sesion()
        {
            _mockError.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroErrorPorId>())).Returns(new List<ERROR> { new ERROR { MESSAGE = "HI" } });
            _mockUserSession.Setup(m => m.ObtenerSesionUsuarioConDispositivo(It.IsAny<FiltroSesionUsuarioAbiertas>())).Returns(new List<USER_SESSION>
            {
                new USER_SESSION { DEVICE_ID = 1, DEVICE = new DEVICE { IP = "123456" }, PROCESS_TROUBLE_TICKETS = new List<PROCESS_TROUBLE_TICKET>{ new PROCESS_TROUBLE_TICKET()} },
                new USER_SESSION { DEVICE = new DEVICE { IP = "987654" }, PROCESS_TROUBLE_TICKETS = new List<PROCESS_TROUBLE_TICKET>() }
            });
            _mockTroubleTicket.Setup(m => m.CrearTicketAppCliente(It.IsAny<CLIENT_APP_TRANSACTION_TROUBLE_TICKET>(), It.IsAny<int>()));
            var resultado = _servicio.RegistrarProblemaClienteAppTransaction(1, 0);
            Assert.AreEqual(true, resultado.Item1);
            Assert.AreEqual("HI", resultado.Item2);
            Assert.AreEqual("987654", resultado.Item3);
        }

        [TestMethod]
        public void TestRegistrarProblemaServicioWebTransactionOk0Sesiones()
        {
            _mockUserSession.Setup(m => m.ObtenerSesionUsuarioConDispositivo(It.IsAny<FiltroSesionUsuarioAbiertas>())).Returns(new List<USER_SESSION>());
            _mockTroubleTicket.Setup(m => m.Agregar(It.IsAny<CLIENT_APP_TRANSACTION_TROUBLE_TICKET>()));
            var resultado = _servicio.RegistrarProblemaServicioWebTransaction("dsad", 0, 0);
            Assert.AreEqual(false, resultado.Item1);
            Assert.AreEqual("", resultado.Item2);
            Assert.AreEqual("", resultado.Item3);
        }

        [TestMethod]
        public void TestRegistrarProblemaServicioWebTransactionOk1Sesion()
        {
            _mockUserSession.Setup(m => m.ObtenerSesionUsuarioConDispositivo(It.IsAny<FiltroSesionUsuarioAbiertas>())).Returns(new List<USER_SESSION> { new USER_SESSION { DEVICE = new DEVICE { IP = "123456" } } });
            _mockTroubleTicket.Setup(m => m.Agregar(It.IsAny<CLIENT_APP_TRANSACTION_TROUBLE_TICKET>()));
            var resultado = _servicio.RegistrarProblemaServicioWebTransaction("HI", 0, 0);
            Assert.AreEqual(true, resultado.Item1);
            Assert.AreEqual("HI", resultado.Item2);
            Assert.AreEqual("123456", resultado.Item3);
        }

        [TestMethod]
        public void TestRegistrarProblemaServicioWebTransactionOk2Sesion()
        {
            _mockUserSession.Setup(m => m.ObtenerSesionUsuarioConDispositivo(It.IsAny<FiltroSesionUsuarioAbiertas>())).Returns(new List<USER_SESSION>
            {
                new USER_SESSION { DEVICE_ID = 1, DEVICE = new DEVICE { IP = "123456" }, PROCESS_TROUBLE_TICKETS = new List<PROCESS_TROUBLE_TICKET>{ new PROCESS_TROUBLE_TICKET()} },
                new USER_SESSION { DEVICE = new DEVICE { IP = "987654" }, PROCESS_TROUBLE_TICKETS = new List<PROCESS_TROUBLE_TICKET>() }
            });
            _mockTroubleTicket.Setup(m => m.Agregar(It.IsAny<PROCESS_TROUBLE_TICKET>()));
            var resultado = _servicio.RegistrarProblemaServicioWebTransaction("HI", 0, 0);
            Assert.AreEqual(true, resultado.Item1);
            Assert.AreEqual("HI", resultado.Item2);
            Assert.AreEqual("987654", resultado.Item3);
        }

        [TestMethod]
        public void TestRegistrarProblemaMobileOk0Sesiones()
        {
            _mockUserSession.Setup(m => m.ObtenerSesionUsuarioConDispositivo(It.IsAny<FiltroSesionUsuarioAbiertas>())).Returns(new List<USER_SESSION>());
            _mockTroubleTicket.Setup(m => m.CrearMobileProceso(It.IsAny<MOBILE_TROUBLE_TICKET>(), It.IsAny<long>()));
            var resultado = _servicio.RegistrarProblemaMobile(0, 0);
            Assert.AreEqual(false, resultado.Item1);
            Assert.AreEqual("", resultado.Item2);
            Assert.AreEqual("", resultado.Item3);
        }

        [TestMethod]
        public void TestRegistrarProblemaMobileOk1Sesion()
        {
            _mockUserSession.Setup(m => m.ObtenerSesionUsuarioConDispositivo(It.IsAny<FiltroSesionUsuarioAbiertas>())).Returns(new List<USER_SESSION> { new USER_SESSION { DEVICE = new DEVICE { IP = "123456" } } });
            _mockTroubleTicket.Setup(m => m.CrearMobileProceso(It.IsAny<MOBILE_TROUBLE_TICKET>(), It.IsAny<long>()));
            var resultado = _servicio.RegistrarProblemaMobile(0, 0);
            Assert.AreEqual(true, resultado.Item1);
            Assert.AreEqual("Error en Proceso N4", resultado.Item2);
            Assert.AreEqual("123456", resultado.Item3);
        }

        [TestMethod]
        public void TestRegistrarProblemaMobileOk2Sesiones()
        {
            _mockUserSession.Setup(m => m.ObtenerSesionUsuarioConDispositivo(It.IsAny<FiltroSesionUsuarioAbiertas>())).Returns(new List<USER_SESSION>
            {
                new USER_SESSION { DEVICE_ID = 1, DEVICE = new DEVICE { IP = "123456" }, PROCESS_TROUBLE_TICKETS = new List<PROCESS_TROUBLE_TICKET>{ new PROCESS_TROUBLE_TICKET()} },
                new USER_SESSION { DEVICE = new DEVICE { IP = "987654" }, PROCESS_TROUBLE_TICKETS = new List<PROCESS_TROUBLE_TICKET>() }
            });
            _mockTroubleTicket.Setup(m => m.CrearMobileProceso(It.IsAny<MOBILE_TROUBLE_TICKET>(), It.IsAny<long>()));
            var resultado = _servicio.RegistrarProblemaMobile(0, 0);
            Assert.AreEqual(true, resultado.Item1);
            Assert.AreEqual("Error en Proceso N4", resultado.Item2);
            Assert.AreEqual("987654", resultado.Item3);
        }

        [TestMethod]
        public void TestRegistrarProblemaGenericoMobileOk0Sesiones()
        {
            _mockUserSession.Setup(m => m.ObtenerSesionUsuarioConDispositivo(It.IsAny<FiltroSesionUsuarioAbiertas>())).Returns(new List<USER_SESSION>());
            _mockTroubleTicket.Setup(m => m.Agregar(It.IsAny<MOBILE_TROUBLE_TICKET>()));
            var resultado = _servicio.RegistrarProblemaGenericoMobile("TUPA", 0);
            Assert.AreEqual(false, resultado.Item1);
            Assert.AreEqual("", resultado.Item2);
            Assert.AreEqual("", resultado.Item3);
        }

        [TestMethod]
        public void TestRegistrarProblemaGenericoMobileOk1Sesion()
        {
            _mockUserSession.Setup(m => m.ObtenerSesionUsuarioConDispositivo(It.IsAny<FiltroSesionUsuarioAbiertas>())).Returns(new List<USER_SESSION> { new USER_SESSION { DEVICE = new DEVICE { IP = "123456" } } });
            _mockTroubleTicket.Setup(m => m.Agregar(It.IsAny<MOBILE_TROUBLE_TICKET>()));
            var resultado = _servicio.RegistrarProblemaGenericoMobile("TUPA", 0);
            Assert.AreEqual(true, resultado.Item1);
            Assert.AreEqual("TUPA", resultado.Item2);
            Assert.AreEqual("123456", resultado.Item3);
        }

        [TestMethod]
        public void TestRegistrarProblemaGenericoMobileOk2Sesiones()
        {
            _mockUserSession.Setup(m => m.ObtenerSesionUsuarioConDispositivo(It.IsAny<FiltroSesionUsuarioAbiertas>())).Returns(new List<USER_SESSION>
            {
                new USER_SESSION { DEVICE_ID = 1, DEVICE = new DEVICE { IP = "123456" }, PROCESS_TROUBLE_TICKETS = new List<PROCESS_TROUBLE_TICKET>{ new PROCESS_TROUBLE_TICKET()} },
                new USER_SESSION { DEVICE = new DEVICE { IP = "987654" }, PROCESS_TROUBLE_TICKETS = new List<PROCESS_TROUBLE_TICKET>() }
            });
            _mockTroubleTicket.Setup(m => m.Agregar(It.IsAny<MOBILE_TROUBLE_TICKET>()));
            var resultado = _servicio.RegistrarProblemaGenericoMobile("TUPA", 0); ;
            Assert.AreEqual(true, resultado.Item1);
            Assert.AreEqual("TUPA", resultado.Item2);
            Assert.AreEqual("987654", resultado.Item3);
        }
    }
}

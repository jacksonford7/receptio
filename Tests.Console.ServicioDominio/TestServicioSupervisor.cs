using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RECEPTIO.CapaAplicacion.Console.Aplicacion.Contratos;
using RECEPTIO.CapaAplicacion.Console.Aplicacion.Interfaces;
using RECEPTIO.CapaDominio.Console.Dominio.InterfacesRepositorios;
using RECEPTIO.CapaDominio.Console.ServiciosDominio;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Repositorio;
using RECEPTIO.CapaDominio.Nucleo.Entidades;

namespace RECEPTIO.CapaDominio.Tests.Console.ServicioDominio
{
    [TestClass]
    public class TestServicioSupervisor
    {
        private ServicioSupervisor _servicio;
        private Mock<IRepositorioTroubleTicket> _mockTroubleTicket;
        private Mock<IRepositorioZone> _mockZone;
        private Mock<IRepositorioUserSession> _mockUserSession;
        private Mock<IRepositorioReassignmentMotive> _mockReassignmentMotive;
        private Mock<IRepositorioQuiosco> _mockQuiosco;
        private Mock<IRepositorioLiftBarrier> _mockLiftBarrier;
        private Mock<IRepositorioAction> _mockAction;
        private Mock<IRepositorioPreGate> _mockPreGate;
        private Mock<IRepositorioDevice> _mockDevice;
        private Mock<IRepositorioTransactionType> _mockTransactionType;
        private Mock<IRepositorioReprinter> _mockReprinter;
        private Mock<IRepositorioByPass> _mockByPass;
        private Mock<IRepositorioStockRegister> _mockStockReg;
        private Mock<IRepositorioMotive> _mockMotivo;
        private Mock<IRepositorioSubMotive> _mockSubMotivo;

        [TestInitialize]
        public void Inicializar()
        {
            _mockTroubleTicket = new Mock<IRepositorioTroubleTicket>();
            _mockZone = new Mock<IRepositorioZone>();
            _mockUserSession = new Mock<IRepositorioUserSession>();
            _mockReassignmentMotive = new Mock<IRepositorioReassignmentMotive>();
            _mockQuiosco = new Mock<IRepositorioQuiosco>();
            _mockLiftBarrier = new Mock<IRepositorioLiftBarrier>();
            _mockAction = new Mock<IRepositorioAction>();
            _mockPreGate = new Mock<IRepositorioPreGate>();
            _mockDevice = new Mock<IRepositorioDevice>();
            _mockTransactionType = new Mock<IRepositorioTransactionType>();
            _mockReprinter = new Mock<IRepositorioReprinter>();
            _mockByPass = new Mock<IRepositorioByPass>();
            _mockStockReg = new Mock<IRepositorioStockRegister>();
            _mockMotivo = new Mock<IRepositorioMotive>();
            _mockSubMotivo = new Mock<IRepositorioSubMotive>();
            _servicio = new ServicioSupervisor(_mockTroubleTicket.Object, _mockZone.Object, _mockUserSession.Object, _mockReassignmentMotive.Object, _mockQuiosco.Object, _mockLiftBarrier.Object, _mockAction.Object, _mockPreGate.Object, _mockDevice.Object, _mockTransactionType.Object, _mockReprinter.Object, _mockByPass.Object, _mockStockReg.Object, _mockMotivo.Object, _mockSubMotivo.Object);
        }

        [TestMethod]
        public void TestObtenerTicketsNoAsignados()
        {
            _mockTroubleTicket.Setup(m => m.ObtenerTicketProcesos(It.IsAny<FiltroTicketProcesoNoAsignados>())).Returns(new List<PROCESS_TROUBLE_TICKET> { new PROCESS_TROUBLE_TICKET { PROCESS = new PROCESS { MESSAGE = new MESSAGE(), KIOSK_TRANSACTION = new KIOSK_TRANSACTION { KIOSK = new KIOSK { ZONE = new ZONE()} } } } });
            _mockTroubleTicket.Setup(m => m.ObtenerTicketMobile(It.IsAny<FiltroTicketMobileNoAsignados>())).Returns(new List<MOBILE_TROUBLE_TICKET>());
            _mockTroubleTicket.Setup(m => m.ObtenerTicketTecnico(It.IsAny<FiltroTicketTecnicoNoAsignados>())).Returns(new List<CLIENT_APP_TRANSACTION_TROUBLE_TICKET> { new CLIENT_APP_TRANSACTION_TROUBLE_TICKET { ZONE = new ZONE()} });
            var resultado = _servicio.ObtenerTicketsNoAsignados();
            Assert.AreEqual(2, resultado.Count());
        }

        [TestMethod]
        public void TestCancelarTicketsCuandoNoExisteUnIdTicket()
        {
            try
            {
                _mockTroubleTicket.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroTroubleTicketPoId>())).Returns(new List<TROUBLE_TICKET>());
                _servicio.CancelarTickets(new List<long> { 1, 2 }, "");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("No existe ticket cuyo id es 1", ex.Message);
                return;
            }
            Assert.Fail("Falló el test.");
        }

        [TestMethod]
        public void TestCancelarTicketsOk()
        {
            _mockTroubleTicket.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroTroubleTicketPoId>())).Returns(new List<TROUBLE_TICKET> { new TROUBLE_TICKET()});
            _servicio.CancelarTickets(new List<long> { 0 }, "");
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestReasignarTicketsCuandoResultadoEs0()
        {
            _mockZone.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroZona>())).Returns(new List<ZONE> { new ZONE { ZONE_ID = 1}, new ZONE { ZONE_ID = 2} });
            _mockUserSession.Setup(m => m.ObtenerSesionUsuarioConDispositivo(It.IsAny<FiltroSesionUsuarioAbiertas>())).Returns(new List<USER_SESSION>());
            var resultado = _servicio.ReasignarTickets(new Dictionary<long, Tuple<TipoTicket, short>>());
            Assert.AreEqual(0, resultado);
        }

        [TestMethod]
        public void TestReasignarTicketsCuandoResultadoEs1()
        {
            _mockZone.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroZona>())).Returns(new List<ZONE> { new ZONE { ZONE_ID = 1 }, new ZONE { ZONE_ID = 2 } });
            _mockUserSession.Setup(m => m.ObtenerSesionUsuarioConDispositivo(It.IsAny<FiltroSesionUsuarioAbiertas>())).Returns(new List<USER_SESSION> { new USER_SESSION()});
            var resultado = _servicio.ReasignarTickets(ObtenerData());
            Assert.AreEqual(1, resultado);
        }

        private Dictionary<long, Tuple<TipoTicket, short>> ObtenerData()
        {
            var resultado = new Dictionary<long, Tuple<TipoTicket, short>>
            {
                { 33, new Tuple<TipoTicket, short>(TipoTicket.Proceso, 0) }
            };
            return resultado;
        }

        [TestMethod]
        public void TestReasignarTicketsCuandoResultadoEs2()
        {
            _mockZone.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroZona>())).Returns(new List<ZONE> { new ZONE { ZONE_ID = 0 }, new ZONE { ZONE_ID = 2 } });
            _mockUserSession.Setup(m => m.ObtenerSesionUsuarioConDispositivo(It.IsAny<FiltroSesionUsuarioAbiertas>())).Returns(new List<USER_SESSION> { new USER_SESSION() });
            var resultado = _servicio.ReasignarTickets(ObtenerData());
            Assert.AreEqual(2, resultado);
        }

        [TestMethod]
        public void TestObtenerTicketsSuspendidos()
        {
            _mockTroubleTicket.Setup(m => m.ObtenerTicketProcesos(It.IsAny<FiltroTicketsSuspendidos>())).Returns(new List<PROCESS_TROUBLE_TICKET> { new PROCESS_TROUBLE_TICKET { PROCESS = new PROCESS { MESSAGE = new MESSAGE(), KIOSK_TRANSACTION = new KIOSK_TRANSACTION { KIOSK = new KIOSK { ZONE = new ZONE() } } } } });
            var resultado = _servicio.ObtenerTicketsSuspendidos();
            Assert.AreEqual(1, resultado.Count());
        }

        [TestMethod]
        public void TestReasignarTicketsSuspendidosCuandoResultadoEs0()
        {
            _mockZone.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroZona>())).Returns(new List<ZONE> { new ZONE { ZONE_ID = 1 }, new ZONE { ZONE_ID = 2 } });
            _mockUserSession.Setup(m => m.ObtenerSesionUsuarioConDispositivo(It.IsAny<FiltroSesionUsuarioAbiertas>())).Returns(new List<USER_SESSION>());
            var resultado = _servicio.ReasignarTicketsSuspendidos(new Dictionary<long, Tuple<long, short>>(), 0, "");
            Assert.AreEqual(0, resultado);
        }

        [TestMethod]
        public void TestReasignarTicketsSuspendidosCuandoResultadoEs1()
        {
            _mockZone.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroZona>())).Returns(new List<ZONE> { new ZONE { ZONE_ID = 1 }, new ZONE { ZONE_ID = 2 } });
            _mockUserSession.Setup(m => m.ObtenerSesionUsuarioConDispositivo(It.IsAny<FiltroSesionUsuarioAbiertas>())).Returns(new List<USER_SESSION> { new USER_SESSION() });
            var resultado = _servicio.ReasignarTicketsSuspendidos(ObtenerData2(), 0, "");
            Assert.AreEqual(1, resultado);
        }

        private Dictionary<long, Tuple<long, short>> ObtenerData2()
        {
            var resultado = new Dictionary<long, Tuple<long, short>>
            {
                { 33, new Tuple<long, short>(0, 0) }
            };
            return resultado;
        }

        [TestMethod]
        public void TestReasignarTicketsSuspendidosCuandoResultadoEs2()
        {
            _mockZone.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroZona>())).Returns(new List<ZONE> { new ZONE { ZONE_ID = 0 }, new ZONE { ZONE_ID = 2 } });
            _mockUserSession.Setup(m => m.ObtenerSesionUsuarioConDispositivo(It.IsAny<FiltroSesionUsuarioAbiertas>())).Returns(new List<USER_SESSION> { new USER_SESSION() });
            var resultado = _servicio.ReasignarTicketsSuspendidos(ObtenerData3(), 0, "");
            Assert.AreEqual(2, resultado);
        }

        private Dictionary<long, Tuple<long, short>> ObtenerData3()
        {
            var resultado = new Dictionary<long, Tuple<long, short>>
            {
                { 33, new Tuple<long, short>(5, 0) }
            };
            return resultado;
        }

        [TestMethod]
        public void TestObtenerTicketsParaReporteConFiltroId()
        {
            _mockTroubleTicket.Setup(m => m.ObtenerTicketProcesos(It.IsAny<FiltroTicketProcesoPorIdYFecha>())).Returns(new List<PROCESS_TROUBLE_TICKET> { new PROCESS_TROUBLE_TICKET { ASSIGNMENT_DATE = DateTime.Now,  USER_SESSION = new USER_SESSION { TROUBLE_DESK_USER = new TROUBLE_DESK_USER { USER_NAME = ""} }, PROCESS = new PROCESS { MESSAGE = new MESSAGE(), KIOSK_TRANSACTION = new KIOSK_TRANSACTION {  KIOSK = new KIOSK { ZONE = new ZONE()}, PRE_GATE = new PRE_GATE { PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL> { new PRE_GATE_DETAIL { TRANSACTION_TYPE = new TRANSACTION_TYPE(), CONTAINERS = new List<CONTAINER> { new CONTAINER { NUMBER = "XXX5656" } } } } } } } } });
            _mockTroubleTicket.Setup(m => m.ObtenerTicketMobile(It.IsAny<FiltroTicketMobilePorIdYFecha>())).Returns(new List<MOBILE_TROUBLE_TICKET> { new MOBILE_TROUBLE_TICKET { MESSAGE = "", ASSIGNMENT_DATE = DateTime.Now, USER_SESSION = new USER_SESSION { TROUBLE_DESK_USER = new TROUBLE_DESK_USER(), DEVICE = new DEVICE { ZONE = new ZONE()} } } });
            _mockTroubleTicket.Setup(m => m.ObtenerTicketTecnico(It.IsAny<FiltroTicketTecnicoPorIdYFecha>())).Returns(new List<CLIENT_APP_TRANSACTION_TROUBLE_TICKET> { new CLIENT_APP_TRANSACTION_TROUBLE_TICKET { ZONE = new ZONE(), ASSIGNMENT_DATE = DateTime.Now, USER_SESSION = new USER_SESSION { TROUBLE_DESK_USER = new TROUBLE_DESK_USER(), DEVICE = new DEVICE { ZONE = new ZONE()} } } });
            _mockTroubleTicket.Setup(m => m.ObtenerAutoTicket(It.IsAny<FiltroTicketAutoPorIdYFecha>())).Returns(new List<AUTO_TROUBLE_TICKET> { new AUTO_TROUBLE_TICKET { ASSIGNMENT_DATE = DateTime.Now, AUTO_TROUBLE_REASON = new AUTO_TROUBLE_REASON(), USER_SESSION = new USER_SESSION { TROUBLE_DESK_USER = new TROUBLE_DESK_USER(), DEVICE = new DEVICE { ZONE = new ZONE()} } } });
            var resultado = _servicio.ObtenerTicketsParaReporte(ObtenerDatos());
            Assert.AreEqual(4, resultado.Count());
        }

        private Dictionary<BusquedaTicketReporte, string> ObtenerDatos()
        {
            var resultado = new Dictionary<BusquedaTicketReporte, string>
            {
                { BusquedaTicketReporte.Estado, "T" },
                { BusquedaTicketReporte.TipoTicket, "TO" },
                { BusquedaTicketReporte.Fecha, "2018-01-01:2018-07-01" },
                { BusquedaTicketReporte.Id, "5" }
            };
            return resultado;
        }

        [TestMethod]
        public void TestObtenerTicketsParaReporteConFiltroEstado()
        {
            _mockTroubleTicket.Setup(m => m.ObtenerTicketProcesos(It.IsAny<FiltroTicketProcesoPorFecha>())).Returns(new List<PROCESS_TROUBLE_TICKET> { new PROCESS_TROUBLE_TICKET { ASSIGNMENT_DATE = DateTime.Now, USER_SESSION = new USER_SESSION { TROUBLE_DESK_USER = new TROUBLE_DESK_USER { USER_NAME = "" } }, PROCESS = new PROCESS { MESSAGE = new MESSAGE(), KIOSK_TRANSACTION = new KIOSK_TRANSACTION { KIOSK = new KIOSK { ZONE = new ZONE() }, PRE_GATE = new PRE_GATE { PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL> { new PRE_GATE_DETAIL { TRANSACTION_TYPE = new TRANSACTION_TYPE(), CONTAINERS = new List<CONTAINER> { new CONTAINER { NUMBER = "XXX5656" } } } } } } } } });
            _mockTroubleTicket.Setup(m => m.ObtenerTicketMobile(It.IsAny<FiltroTicketMobilePorFecha>())).Returns(new List<MOBILE_TROUBLE_TICKET> { new MOBILE_TROUBLE_TICKET { MESSAGE = "", ASSIGNMENT_DATE = DateTime.Now, USER_SESSION = new USER_SESSION { TROUBLE_DESK_USER = new TROUBLE_DESK_USER(), DEVICE = new DEVICE { ZONE = new ZONE() } } } });
            _mockTroubleTicket.Setup(m => m.ObtenerTicketTecnico(It.IsAny<FiltroTicketTecnicoPorFecha>())).Returns(new List<CLIENT_APP_TRANSACTION_TROUBLE_TICKET> { new CLIENT_APP_TRANSACTION_TROUBLE_TICKET { ZONE = new ZONE(), ASSIGNMENT_DATE = DateTime.Now, USER_SESSION = new USER_SESSION { TROUBLE_DESK_USER = new TROUBLE_DESK_USER(), DEVICE = new DEVICE { ZONE = new ZONE() } } } });
            _mockTroubleTicket.Setup(m => m.ObtenerAutoTicket(It.IsAny<FiltroTicketAutoPorFecha>())).Returns(new List<AUTO_TROUBLE_TICKET> { new AUTO_TROUBLE_TICKET { FINISH_DATE = DateTime.Now, ASSIGNMENT_DATE = DateTime.Now, AUTO_TROUBLE_REASON = new AUTO_TROUBLE_REASON(), USER_SESSION = new USER_SESSION { TROUBLE_DESK_USER = new TROUBLE_DESK_USER(), DEVICE = new DEVICE { ZONE = new ZONE() } } } });
            var resultado = _servicio.ObtenerTicketsParaReporte(ObtenerDatos2());
            Assert.AreEqual(1, resultado.Count());
        }

        private Dictionary<BusquedaTicketReporte, string> ObtenerDatos2()
        {
            var resultado = new Dictionary<BusquedaTicketReporte, string>
            {
                { BusquedaTicketReporte.Estado, "R" },
                { BusquedaTicketReporte.TipoTicket, "TO" },
                { BusquedaTicketReporte.Fecha, "2018-01-01:2018-07-01" }
            };
            return resultado;
        }

        [TestMethod]
        public void TestObtenerTicketsParaReporteConFiltroEstadoYId()
        {
            _mockTroubleTicket.Setup(m => m.ObtenerTicketProcesos(It.IsAny<FiltroTicketProcesoPorIdYFecha>())).Returns(new List<PROCESS_TROUBLE_TICKET> { new PROCESS_TROUBLE_TICKET { ACCEPTANCE_DATE = new DateTime(), ASSIGNMENT_DATE = DateTime.Now, USER_SESSION = new USER_SESSION { TROUBLE_DESK_USER = new TROUBLE_DESK_USER { USER_NAME = "" } }, PROCESS = new PROCESS { MESSAGE = new MESSAGE(), KIOSK_TRANSACTION = new KIOSK_TRANSACTION { KIOSK = new KIOSK { ZONE = new ZONE() }, PRE_GATE = new PRE_GATE { PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL> { new PRE_GATE_DETAIL { TRANSACTION_TYPE = new TRANSACTION_TYPE(), CONTAINERS = new List<CONTAINER> { new CONTAINER { NUMBER = "XXX5656" } } } } } } } } });
            _mockTroubleTicket.Setup(m => m.ObtenerTicketMobile(It.IsAny<FiltroTicketMobilePorIdYFecha>())).Returns(new List<MOBILE_TROUBLE_TICKET> { new MOBILE_TROUBLE_TICKET { MESSAGE = "", ASSIGNMENT_DATE = DateTime.Now, USER_SESSION = new USER_SESSION { TROUBLE_DESK_USER = new TROUBLE_DESK_USER(), DEVICE = new DEVICE { ZONE = new ZONE() } } } });
            _mockTroubleTicket.Setup(m => m.ObtenerTicketTecnico(It.IsAny<FiltroTicketTecnicoPorIdYFecha>())).Returns(new List<CLIENT_APP_TRANSACTION_TROUBLE_TICKET> { new CLIENT_APP_TRANSACTION_TROUBLE_TICKET { ZONE = new ZONE(), ACCEPTANCE_DATE = new DateTime(), ASSIGNMENT_DATE = DateTime.Now, USER_SESSION = new USER_SESSION { TROUBLE_DESK_USER = new TROUBLE_DESK_USER(), DEVICE = new DEVICE { ZONE = new ZONE() } } } });
            _mockTroubleTicket.Setup(m => m.ObtenerAutoTicket(It.IsAny<FiltroTicketAutoPorIdYFecha>())).Returns(new List<AUTO_TROUBLE_TICKET> { new AUTO_TROUBLE_TICKET { FINISH_DATE = DateTime.Now, ASSIGNMENT_DATE = DateTime.Now, AUTO_TROUBLE_REASON = new AUTO_TROUBLE_REASON(), USER_SESSION = new USER_SESSION { TROUBLE_DESK_USER = new TROUBLE_DESK_USER(), DEVICE = new DEVICE { ZONE = new ZONE() } } } });
            var resultado = _servicio.ObtenerTicketsParaReporte(ObtenerDatos3());
            Assert.AreEqual(2, resultado.Count());
        }

        private Dictionary<BusquedaTicketReporte, string> ObtenerDatos3()
        {
            var resultado = new Dictionary<BusquedaTicketReporte, string>
            {
                { BusquedaTicketReporte.Estado, "ER" },
                { BusquedaTicketReporte.TipoTicket, "TO" },
                { BusquedaTicketReporte.Fecha, "2018-01-01:2018-07-01" },
                { BusquedaTicketReporte.Id, "5" }
            };
            return resultado;
        }

        [TestMethod]
        public void TestObtenerKioscosActivos()
        {
            _mockQuiosco.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroQuioscoActivos>())).Returns(new List<KIOSK> { new KIOSK() });
            var resultado = _servicio.ObtenerKioscosActivos();
            Assert.AreEqual(1, resultado.Count());
        }

        [TestMethod]
        public void TestObtenerMotivosReasignacion()
        {
            _mockReassignmentMotive.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroMotivoReasignacionActivos>())).Returns(new List<REASSIGNMENT_MOTIVE> { new REASSIGNMENT_MOTIVE() });
            var resultado = _servicio.ObtenerMotivosReasignacion();
            Assert.AreEqual(1, resultado.Count());
        }

        [TestMethod]
        public void TestObtenerAccionesTicket()
        {
            _mockAction.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroAccionPorIdTicket>())).Returns(new List<ACTION> { new ACTION() });
            var resultado = _servicio.ObtenerAccionesTicket(0);
            Assert.AreEqual(1, resultado.Count());
        }

        [TestMethod]
        public void TestRegistrarAperturaBarrera()
        {
            try
            {
                _mockLiftBarrier.Setup(m => m.Agregar(It.IsAny<LIFT_UP_BARRIER>()));
                _servicio.RegistrarAperturaBarrera(new LIFT_UP_BARRIER());
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
                return;
            }
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestObtenerSesionesUsuarios()
        {
            _mockUserSession.Setup(m => m.ObtenerSesionUsuarioConDispositivoZonaUsuario(It.IsAny<FiltroSesionUsuarioAbiertasParaCerrarlas>())).Returns(new List<USER_SESSION> { new USER_SESSION() });
            var resultado = _servicio.ObtenerSesionesUsuarios();
            Assert.AreEqual(1, resultado.Count());
        }

        [TestMethod]
        public void TestObtenerTransaccionesKiosco()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorFecha>())).Returns(DatosPrueba());
            var resultado = _servicio.ObtenerTransaccionesKiosco(ObtenerFiltros());
            Assert.AreEqual(1, resultado.Count());
        }

        private IEnumerable<PRE_GATE> DatosPrueba()
        {
            return new List<PRE_GATE>
            {
                new PRE_GATE
                {
                    CREATION_DATE = new DateTime(2018-12-10),
                    DEVICE_ID = 2,
                    DEVICE = new DEVICE{ NAME = "", ZONE = new ZONE{ NAME = ""} },
                    DRIVER_ID = "1234567890",
                    PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 4} },
                    PRE_GATE_ID = 33,
                    STATUS = "K",
                    TRUCK_LICENCE = "GWU45",
                    USER = "kikin",
                    KIOSK_TRANSACTIONS = new List<KIOSK_TRANSACTION>{ new KIOSK_TRANSACTION { KIOSK_ID = 3, KIOSK = new KIOSK { KIOSK_ID = 3} } }
                },
                new PRE_GATE
                {
                    CREATION_DATE = new DateTime(2018-12-10),
                    DEVICE_ID = 2,
                    DEVICE = new DEVICE{ NAME = "", ZONE = new ZONE{ NAME = ""} },
                    DRIVER_ID = "123456789",
                    PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 4} },
                    PRE_GATE_ID = 33,
                    STATUS = "K",
                    TRUCK_LICENCE = "GWU45",
                    USER = "kikin",
                    KIOSK_TRANSACTIONS = new List<KIOSK_TRANSACTION>{ new KIOSK_TRANSACTION { KIOSK_ID = 3} }
                },
                new PRE_GATE
                {
                    CREATION_DATE = new DateTime(2018-12-10),
                    DEVICE_ID = 2,
                    DEVICE = new DEVICE{ NAME = "", ZONE = new ZONE{ NAME = ""} },
                    DRIVER_ID = "1234567890",
                    PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>{ new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 4} },
                    PRE_GATE_ID = 33,
                    STATUS = "A",
                    TRUCK_LICENCE = "GWU45",
                    USER = "kikin",
                    KIOSK_TRANSACTIONS = new List<KIOSK_TRANSACTION>{ new KIOSK_TRANSACTION { KIOSK_ID = 3} }
                }
            };
        }

        private Dictionary<BusquedaReporteTransacciones, string> ObtenerFiltros()
        {
            var resultado = new Dictionary<BusquedaReporteTransacciones, string>
            {
                { BusquedaReporteTransacciones.Fecha, "2018-12-09:2018-12-11" },
                { BusquedaReporteTransacciones.CedulaChofer, "1234567890"},
                { BusquedaReporteTransacciones.Estado, "K"},
                { BusquedaReporteTransacciones.GosTv, "33"},
                { BusquedaReporteTransacciones.IdDispositivo, "2"},
                { BusquedaReporteTransacciones.IdKiosco, "3"},
                { BusquedaReporteTransacciones.IdTipoTransaccion, "4"},
                { BusquedaReporteTransacciones.IdZona, "0"},
                { BusquedaReporteTransacciones.Placa, "GWU45" },
                { BusquedaReporteTransacciones.Usuario, "kikin"}
            };
            return resultado;
        }

        [TestMethod]
        public void TestObtenerTablets()
        {
            _mockDevice.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroDispositivoTablets>())).Returns(new List<DEVICE>());
            var resultado = _servicio.ObtenerTablets();
            Assert.AreEqual(0, resultado.Count());
        }

        [TestMethod]
        public void TestObtenerZonas()
        {
            _mockZone.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroZonasActivas>())).Returns(new List<ZONE>());
            var resultado = _servicio.ObtenerZonas();
            Assert.AreEqual(0, resultado.Count());
        }


        [TestMethod]
        public void TestObtenerTiposTransacciones()
        {
            _mockTransactionType.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroTipoTransaccion>())).Returns(new List<TRANSACTION_TYPE>());
            var resultado = _servicio.ObtenerTiposTransacciones();
            Assert.AreEqual(0, resultado.Count());
        }

        [TestMethod]
        public void TestObtenerInformacionParaReimpresionTicketCuandoNoexisteCodigo()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE>());
            var resultado = _servicio.ObtenerInformacionParaReimpresionTicket(0, false);
            Assert.AreEqual(false, resultado.Item1);
            Assert.AreEqual("No existe el código ingresado.", resultado.Item2);
        }

        [TestMethod]
        public void TestObtenerInformacionParaReimpresionTicketCuandoNoExisteProcesoExitosoN4()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { KIOSK_TRANSACTIONS = new List<KIOSK_TRANSACTION> { new KIOSK_TRANSACTION { KIOSK = new KIOSK(), PROCESSES = new List<PROCESS> { new PROCESS() }  } } } });
            var resultado = _servicio.ObtenerInformacionParaReimpresionTicket(0, false);
            Assert.AreEqual(false, resultado.Item1);
            Assert.AreEqual("No exite un proceso de N4 exitoso", resultado.Item2);
        }

        [TestMethod]
        public void TestObtenerInformacionParaReimpresionTicketOk()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { KIOSK_TRANSACTIONS = new List<KIOSK_TRANSACTION> { new KIOSK_TRANSACTION { KIOSK = new KIOSK(), PROCESSES = new List<PROCESS> { new PROCESS { IS_OK = true, STEP = "PROCESO_N4" } } } } } });
            var resultado = _servicio.ObtenerInformacionParaReimpresionTicket(0, false);
            Assert.AreEqual(true, resultado.Item1);
        }

        [TestMethod]
        public void TestRegistrarReimpresion()
        {
            try
            {
                _mockReprinter.Setup(m => m.Agregar(It.IsAny<REPRINT>()));
                _servicio.RegistrarReimpresion(new REPRINT());
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
                return;
            }
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestValidarIdPreGateCuandoNoExisteId()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE>());
            var resultado = _servicio.ValidarIdPreGate(0);
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("No Existe id", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarIdPreGateCuandoNoExisteTransaccionExitosa()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "O" } });
            var resultado = _servicio.ValidarIdPreGate(0);
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("No se puede utilizar el id porque ya existe una transacción de salida exitosa.", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarIdPreGateCuandoYaExisteByPass()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "I", BY_PASS = new BY_PASS() } });
            var resultado = _servicio.ValidarIdPreGate(0);
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("Ya existe un bypass para este Id.", resultado.Mensaje);
        }

        [TestMethod]
        public void TestValidarIdPreGateOk()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "I" } });
            var resultado = _servicio.ValidarIdPreGate(0);
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("", resultado.Mensaje);
        }

        [TestMethod]
        public void TestCrearByPass()
        {
            try
            {
                _mockByPass.Setup(m => m.InsertarRegistro(It.IsAny<BY_PASS>()));
                _servicio.CrearByPass(new BY_PASS(), 0);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
                return;
            }
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestObtenerByPassCuandoEsNulo()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE>());
            var resultado = _servicio.ObtenerByPass(0);
            Assert.IsNull(resultado);
        }

        [TestMethod]
        public void TestObtenerByPassOk()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { BY_PASS = new BY_PASS()} });
            var resultado = _servicio.ObtenerByPass(0);
            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public void TestActualizarrByPass()
        {
            try
            {
                _mockByPass.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroByPassPorId>())).Returns(new List<BY_PASS> { new BY_PASS() });
                _mockByPass.Setup(m => m.Actualizar(It.IsAny<BY_PASS>()));
                _servicio.ActualizarrByPass(new BY_PASS(), 0);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
                return;
            }
            Assert.IsTrue(true);
        }
    }
}

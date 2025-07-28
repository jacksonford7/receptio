using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Repositorio;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using RECEPTIO.CapaDominio.Nucleo.ServiciosDominio.Filtros;
using RECEPTIO.CapaDominio.Nucleo.ServiciosDominio.N4;
using RECEPTIO.CapaDominio.Nucleo.ServiciosDominio.ServicioN4;
using RECEPTIO.CapaDominio.Transaction.Dominio.InterfacesRepositorios;
using RECEPTIO.CapaDominio.TransactionEmpty.ServiciosDominio;
using System;
using System.Collections.Generic;

namespace RECEPTIO.CapaDominio.Tests.TransactionEmpty.ServiciosDominio
{
    [TestClass]
    public class TestServicioGeneral
    {
        private ServicioGeneral _servicio;
        private Mock<IConector> _mockConector;
        private Mock<IRepositorioPreGate> _mockPreGate;
        private Mock<IRepositorioTransaccionQuiosco> _mockTransaccionQuiosco;
        private Mock<IRepositorioValidaAduana> _mockValidaAduana;
        private Mock<IRepositorioN4> _mockN4;
        private string _xmlProcessTruckSalida;
        private string _xmlTruckVisit;
        private string _xmlGroovy;
        private string _xmlStageDone;
        private string _xmlProcessTruckEntrada;
        private string _xmlStageDone1;
        private string _xmlStageDone2;

        [TestInitialize]
        public void Inicializar()
        {
            _mockConector = new Mock<IConector>();
            _mockTransaccionQuiosco = new Mock<IRepositorioTransaccionQuiosco>();
            _mockPreGate = new Mock<IRepositorioPreGate>();
            _mockValidaAduana = new Mock<IRepositorioValidaAduana>();
            _mockN4 = new Mock<IRepositorioN4>();
            _servicio = new ServicioGeneral(_mockPreGate.Object, _mockTransaccionQuiosco.Object,_mockValidaAduana.Object, _mockConector.Object, _mockN4.Object);
            _xmlProcessTruckSalida = $@"<gate>
    	                <process-truck scan-status=""0"" no-content=""false"">
                            <gate-id>RECEPTIO</gate-id>
		                    <stage-id>outgate</stage-id>
                            <lane-id>PRUEBA</lane-id>
		                    <truck license-nbr=""""/>
                            <driver license-nbr=""""/>
                            <truck-visit gos-tv-key=""0""/>
                            <timestamp>{DateTime.Now.ToString("yyyy-MM-ddTHH")}</timestamp>
                        </process-truck>
                      </gate>";
            _xmlTruckVisit = $@"<gate>
    	                <create-truck-visit>
		                    <gate-id>RECEPTIO_MTY</gate-id>
		                    <stage-id>mty_pre</stage-id>
		                    <truck license-nbr="""" trucking-co-id=""""/>
                            <driver license-nbr=""""/>
                            <truck-visit gos-tv-key=""0""/>
                            <timestamp>{DateTime.Now.ToString("yyyy-MM-ddTHH")}</timestamp>
                        </create-truck-visit>
                      </gate>";
            _xmlGroovy = $@"<groovy class-name=""CGSAMTYGateTransactionInWS"" class-location=""code-extension"">
                          <parameters>
                            <parameter id=""gos-tv-key"" value=""0""/>
                            <parameter id=""fecha"" value=""{DateTime.Now.ToString("yyyy-MM-dd HH")}""/>
                            <parameter id=""order-nbr"" value=""456""/>
                            <parameter id=""operator"" value=""456""/> 
                            <parameter id=""kiosco"" value=""123""/>
                            <parameter id=""stage-id"" value=""mty_pre""/>
                            <parameter id=""nextstage-id"" value=""mty_in""/>
                            <parameter id=""iso-type"" value=""789""/>
                            <parameter id=""item-qty"" value=""1""/>
                            <parameter id=""numdocumento"" value=""123""/> 
                            <parameter id =""notes"" value="""" />
                          </parameters>
                      </groovy>";
            _xmlStageDone = $@"<gate>
    	                <stage-done>
		                    <gate-id>RECEPTIO_MTY</gate-id>
		                    <stage-id>mty_pre</stage-id>
                            <truck-visit gos-tv-key=""0""/>
                        </stage-done>
                      </gate>";
            _xmlProcessTruckEntrada = $@"<gate>
    	                <process-truck scan-status=""0"" no-content=""false"">
                            <gate-id>RECEPTIO_MTY</gate-id>
		                    <stage-id>mty_in</stage-id>
                            <lane-id>PRUEBA</lane-id>
		                    <truck license-nbr=""""/>
                            <driver license-nbr=""""/>
                            <truck-visit gos-tv-key=""0""/>
                            <timestamp>{DateTime.Now.ToString("yyyy-MM-ddTHH")}</timestamp>
                        </process-truck>
                      </gate>";
            _xmlStageDone1 = $@"<gate>
    	                <stage-done>
		                    <gate-id>RECEPTIO_MTY</gate-id>
		                    <stage-id>mty_yard</stage-id>
                            <lane-id>PRUEBA</lane-id>
                            <truck-visit gos-tv-key=""0""/>
                        </stage-done>
                      </gate>";
            _xmlStageDone2 = @"<gate>
    	                <stage-done>
		                    <gate-id>RECEPTIO_MTY</gate-id>
		                    <stage-id>mty_yard2</stage-id>
                            <lane-id>PRUEBA</lane-id>
                            <truck-visit gos-tv-key=""0""/>
                        </stage-done>
                      </gate>";
        }

        [TestMethod]
        public void TestRegistrarProceso()
        {
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE>());
            _mockTransaccionQuiosco.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroTransaccionQuioscoPorId>())).Returns(new List<KIOSK_TRANSACTION>());
            _mockTransaccionQuiosco.Setup(m => m.Actualizar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.RegistrarProceso(new KIOSK_TRANSACTION { PROCESSES = new List<PROCESS> { new PROCESS()} });
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestProcesarCuandoNoExisteId()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE>());
            _mockTransaccionQuiosco.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.Procesar(0, new KIOSK());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("33", resultado.Mensaje);
        }

        [TestMethod]
        public void TestProcesarCuandoNoTieneCodigoTransaccionContemplada()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL> { new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 2, TRANSACTION_TYPE = new TRANSACTION_TYPE { DESCRIPTION = ""} } } } });
            _mockTransaccionQuiosco.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.Procesar(0, new KIOSK());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("34", resultado.Mensaje);
        }

        [TestMethod]
        public void TestProcesarCuandoYaSalioDeLaTerminal1()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "O", PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL> { new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 10 , STATUS = "N" } , new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 10, STATUS = "N" } } } });
            _mockTransaccionQuiosco.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.Procesar(0, new KIOSK());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("19", resultado.Mensaje);
        }

        [TestMethod]
        public void TestProcesarCuandoYaSalioDeLaTerminal2()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "O", PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL> { new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 10, STATUS = "O" }, new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 10, STATUS = "O" } } } });
            _mockTransaccionQuiosco.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.Procesar(0, new KIOSK());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("19", resultado.Mensaje);
        }

        [TestMethod]
        public void TestProcesarCuandoTieneTarnsaccionActivaPlaca()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "O", PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL> { new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 10, STATUS = "P" }, new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 10, STATUS = "P" } } } });
            _mockN4.Setup(m => m.TieneTransaccionActivaPorPlaca(It.IsAny<string>())).Returns(true);
            _mockTransaccionQuiosco.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.Procesar(0, new KIOSK());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("40", resultado.Mensaje);
        }

        [TestMethod]
        public void TestProcesarCuandoNoTieneEntradaKiosco()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "N", PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL> { new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 10}, new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 10} } } });
            _mockTransaccionQuiosco.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.Procesar(0, new KIOSK());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("20", resultado.Mensaje);
        }

        [TestMethod]
        public void TestProcesarCuandoYaTieneDespachoContenedorVacio()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "I", KIOSK_TRANSACTIONS = new List<KIOSK_TRANSACTION> { new KIOSK_TRANSACTION { IS_OK = true, KIOSK = new KIOSK() } }, PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL> { new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 10 }, new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 10 } } } });
            _mockTransaccionQuiosco.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.Procesar(0, new KIOSK());
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("39", resultado.Mensaje);
        }

        [TestMethod]
        public void TestProcesarCuandoFallaImdt()
        {
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "I", KIOSK_TRANSACTIONS = new List<KIOSK_TRANSACTION> { new KIOSK_TRANSACTION { IS_OK = true, KIOSK = new KIOSK { IS_IN = true} } }, PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL> { new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 10, REFERENCE_ID = "I" }, new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 10, REFERENCE_ID = "E", CONTAINERS = new List<CONTAINER> { new CONTAINER { NUMBER = "" } } } } } });
            _mockValidaAduana.Setup(m => m.ObtenerGKeyContenedor(It.IsAny<string>())).Returns(0);
            _mockTransaccionQuiosco.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.Procesar(0, new KIOSK { NAME = "PRUEBA"});
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("36", resultado.Mensaje);
        }

        [TestMethod]
        public void TestProcesarCuandoFallaProcessTruckSalida()
        {
            var pregate = new PRE_GATE { TRUCK_LICENCE = "", DRIVER_ID = "", STATUS = "I", KIOSK_TRANSACTIONS = new List<KIOSK_TRANSACTION> { new KIOSK_TRANSACTION { IS_OK = true, KIOSK = new KIOSK { IS_IN = true } } }, PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL> { new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 10, REFERENCE_ID = "I" }, new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 10, REFERENCE_ID = "E", CONTAINERS = new List<CONTAINER> { new CONTAINER { NUMBER = "" } } } } };
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { pregate });
            _mockValidaAduana.Setup(m => m.ObtenerGKeyContenedor(It.IsAny<string>())).Returns(4290331);
            _mockConector.Setup(m => m.Invocacion(It.IsAny<string>())).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 2, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockTransaccionQuiosco.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.Procesar(0, new KIOSK { NAME = "PRUEBA" });
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("14", resultado.Mensaje);
        }

        [TestMethod]
        public void TestProcesarCuandoTieneVisitaActiva()
        {
            var pregate = new PRE_GATE { TRUCK_LICENCE = "", DRIVER_ID = "", STATUS = "I", KIOSK_TRANSACTIONS = new List<KIOSK_TRANSACTION> { new KIOSK_TRANSACTION { IS_OK = true, KIOSK = new KIOSK { IS_IN = true } } }, PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL> { new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 10, REFERENCE_ID = "I" }, new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 10, REFERENCE_ID = "E", CONTAINERS = new List<CONTAINER> { new CONTAINER { NUMBER = "" } } } } };
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { pregate });
            _mockValidaAduana.Setup(m => m.ObtenerGKeyContenedor(It.IsAny<string>())).Returns(4290331);
            _mockConector.Setup(m => m.Invocacion(It.IsAny<string>())).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockN4.Setup(m => m.TieneTransaccionActiva(It.IsAny<string>())).Returns(true);
            _mockTransaccionQuiosco.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            var resultado = _servicio.Procesar(0, new KIOSK { NAME = "PRUEBA" });
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("40", resultado.Mensaje);
        }

        [TestMethod]
        public void TestProcesarCuandoFallaTruckVisit()
        {
            var pregate = new PRE_GATE { TRUCK_LICENCE = "", DRIVER_ID = "", STATUS = "I", KIOSK_TRANSACTIONS = new List<KIOSK_TRANSACTION> { new KIOSK_TRANSACTION { IS_OK = true, KIOSK = new KIOSK { IS_IN = true } } }, PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL> { new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 10, REFERENCE_ID = "I", DOCUMENT_ID = "", CONTAINERS = new List<CONTAINER> { new CONTAINER { NUMBER = "" } } }, new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 10, REFERENCE_ID = "E", CONTAINERS = new List<CONTAINER> { new CONTAINER { NUMBER = "" } } } } };
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { pregate });
            _mockValidaAduana.Setup(m => m.ObtenerGKeyContenedor(It.IsAny<string>())).Returns(4290331);
            _mockConector.Setup(m => m.Invocacion(_xmlProcessTruckSalida)).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockTransaccionQuiosco.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            _mockPreGate.Setup(m => m.Agregar(It.IsAny<PRE_GATE>()));
            _mockN4.Setup(m => m.TieneTransaccionActiva(It.IsAny<string>())).Returns(false);
            _mockConector.Setup(m => m.Invocacion(_xmlTruckVisit)).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 3, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            var resultado = _servicio.Procesar(0, new KIOSK { NAME = "PRUEBA" });
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("14", resultado.Mensaje);
        }

        [TestMethod]
        public void TestProcesarCuandoFallaGroovy()
        {
            var pregate = new PRE_GATE { TRUCK_LICENCE = "", DRIVER_ID = "", STATUS = "I", KIOSK_TRANSACTIONS = new List<KIOSK_TRANSACTION> { new KIOSK_TRANSACTION { IS_OK = true, KIOSK = new KIOSK { IS_IN = true } } }, PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL> { new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 10, TRANSACTION_NUMBER = "123-456-789", REFERENCE_ID = "I", DOCUMENT_ID = "", CONTAINERS = new List<CONTAINER> { new CONTAINER { NUMBER = "" } } }, new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 10, REFERENCE_ID = "E", CONTAINERS = new List<CONTAINER> { new CONTAINER { NUMBER = "" } } } } };
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { pregate });
            _mockValidaAduana.Setup(m => m.ObtenerGKeyContenedor(It.IsAny<string>())).Returns(4290331);
            _mockConector.Setup(m => m.Invocacion(_xmlProcessTruckSalida)).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockTransaccionQuiosco.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            _mockPreGate.Setup(m => m.Agregar(It.IsAny<PRE_GATE>()));
            _mockN4.Setup(m => m.TieneTransaccionActiva(It.IsAny<string>())).Returns(false);
            _mockConector.Setup(m => m.Invocacion(_xmlTruckVisit)).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockConector.Setup(m => m.Invocacion(_xmlGroovy)).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 3, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            var resultado = _servicio.Procesar(0, new KIOSK { NAME = "PRUEBA", IP = "123" });
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("14", resultado.Mensaje);
        }

        [TestMethod]
        public void TestProcesarCuandoFallaStageDone()
        {
            var pregate = new PRE_GATE { TRUCK_LICENCE = "", DRIVER_ID = "", STATUS = "I", KIOSK_TRANSACTIONS = new List<KIOSK_TRANSACTION> { new KIOSK_TRANSACTION { IS_OK = true, KIOSK = new KIOSK { IS_IN = true } } }, PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL> { new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 10, TRANSACTION_NUMBER = "123-456-789", REFERENCE_ID = "I", DOCUMENT_ID = "", CONTAINERS = new List<CONTAINER> { new CONTAINER { NUMBER = "" } } }, new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 10, REFERENCE_ID = "E", CONTAINERS = new List<CONTAINER> { new CONTAINER { NUMBER = "" } } } } };
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { pregate });
            _mockValidaAduana.Setup(m => m.ObtenerGKeyContenedor(It.IsAny<string>())).Returns(4290331);
            _mockConector.Setup(m => m.Invocacion(_xmlProcessTruckSalida)).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockTransaccionQuiosco.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            _mockPreGate.Setup(m => m.Agregar(It.IsAny<PRE_GATE>()));
            _mockN4.Setup(m => m.TieneTransaccionActiva(It.IsAny<string>())).Returns(false);
            _mockConector.Setup(m => m.Invocacion(_xmlTruckVisit)).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockConector.Setup(m => m.Invocacion(_xmlGroovy)).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockConector.Setup(m => m.Invocacion(_xmlStageDone)).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 2, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            var resultado = _servicio.Procesar(0, new KIOSK { NAME = "PRUEBA", IP = "123" });
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("14", resultado.Mensaje);
        }

        [TestMethod]
        public void TestProcesarCuandoFallaProcessTruckEntrada()
        {
            var pregate = new PRE_GATE { TRUCK_LICENCE = "", DRIVER_ID = "", STATUS = "I", KIOSK_TRANSACTIONS = new List<KIOSK_TRANSACTION> { new KIOSK_TRANSACTION { IS_OK = true, KIOSK = new KIOSK { IS_IN = true } } }, PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL> { new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 10, TRANSACTION_NUMBER = "123-456-789", REFERENCE_ID = "I", DOCUMENT_ID = "", CONTAINERS = new List<CONTAINER> { new CONTAINER { NUMBER = "" } } }, new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 10, REFERENCE_ID = "E", CONTAINERS = new List<CONTAINER> { new CONTAINER { NUMBER = "" } } } } };
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { pregate });
            _mockValidaAduana.Setup(m => m.ObtenerGKeyContenedor(It.IsAny<string>())).Returns(4290331);
            _mockConector.Setup(m => m.Invocacion(_xmlProcessTruckSalida)).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockTransaccionQuiosco.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            _mockPreGate.Setup(m => m.Agregar(It.IsAny<PRE_GATE>()));
            _mockN4.Setup(m => m.TieneTransaccionActiva(It.IsAny<string>())).Returns(false);
            _mockConector.Setup(m => m.Invocacion(_xmlTruckVisit)).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockConector.Setup(m => m.Invocacion(_xmlGroovy)).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockConector.Setup(m => m.Invocacion(_xmlStageDone)).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockConector.Setup(m => m.Invocacion(_xmlProcessTruckEntrada)).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 2, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            var resultado = _servicio.Procesar(0, new KIOSK { NAME = "PRUEBA", IP = "123" });
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("14", resultado.Mensaje);
        }

        [TestMethod]
        public void TestProcesarCuandoFallaStageDone2()
        {
            var pregate = new PRE_GATE { TRUCK_LICENCE = "", DRIVER_ID = "", STATUS = "I", KIOSK_TRANSACTIONS = new List<KIOSK_TRANSACTION> { new KIOSK_TRANSACTION { IS_OK = true, KIOSK = new KIOSK { IS_IN = true } } }, PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL> { new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 10, TRANSACTION_NUMBER = "123-456-789", REFERENCE_ID = "I", DOCUMENT_ID = "", CONTAINERS = new List<CONTAINER> { new CONTAINER { NUMBER = "" } } }, new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 10, REFERENCE_ID = "E", CONTAINERS = new List<CONTAINER> { new CONTAINER { NUMBER = "" } } } } };
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { pregate });
            _mockValidaAduana.Setup(m => m.ObtenerGKeyContenedor(It.IsAny<string>())).Returns(4290331);
            _mockConector.Setup(m => m.Invocacion(_xmlProcessTruckSalida)).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockTransaccionQuiosco.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            _mockPreGate.Setup(m => m.Agregar(It.IsAny<PRE_GATE>()));
            _mockN4.Setup(m => m.TieneTransaccionActiva(It.IsAny<string>())).Returns(false);
            _mockConector.Setup(m => m.Invocacion(_xmlTruckVisit)).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockConector.Setup(m => m.Invocacion(_xmlGroovy)).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockConector.Setup(m => m.Invocacion(_xmlStageDone)).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockConector.Setup(m => m.Invocacion(_xmlProcessTruckEntrada)).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockConector.Setup(m => m.Invocacion(_xmlStageDone1)).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 2, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            var resultado = _servicio.Procesar(0, new KIOSK { NAME = "PRUEBA", IP = "123" });
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("14", resultado.Mensaje);
        }

        [TestMethod]
        public void TestProcesarCuandoFallaStageDone3()
        {
            var pregate = new PRE_GATE { TRUCK_LICENCE = "", DRIVER_ID = "", STATUS = "I", KIOSK_TRANSACTIONS = new List<KIOSK_TRANSACTION> { new KIOSK_TRANSACTION { IS_OK = true, KIOSK = new KIOSK { IS_IN = true } } }, PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL> { new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 10, TRANSACTION_NUMBER = "123-456-789", REFERENCE_ID = "I", DOCUMENT_ID = "", CONTAINERS = new List<CONTAINER> { new CONTAINER { NUMBER = "" } } }, new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 10, REFERENCE_ID = "E", CONTAINERS = new List<CONTAINER> { new CONTAINER { NUMBER = "" } } } } };
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { pregate });
            _mockValidaAduana.Setup(m => m.ObtenerGKeyContenedor(It.IsAny<string>())).Returns(4290331);
            _mockConector.Setup(m => m.Invocacion(_xmlProcessTruckSalida)).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockTransaccionQuiosco.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            _mockPreGate.Setup(m => m.Agregar(It.IsAny<PRE_GATE>()));
            _mockN4.Setup(m => m.TieneTransaccionActiva(It.IsAny<string>())).Returns(false);
            _mockConector.Setup(m => m.Invocacion(_xmlTruckVisit)).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockConector.Setup(m => m.Invocacion(_xmlGroovy)).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockConector.Setup(m => m.Invocacion(_xmlStageDone)).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockConector.Setup(m => m.Invocacion(_xmlProcessTruckEntrada)).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockConector.Setup(m => m.Invocacion(_xmlStageDone1)).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockConector.Setup(m => m.Invocacion(_xmlStageDone2)).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 2, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            var resultado = _servicio.Procesar(0, new KIOSK { NAME = "PRUEBA", IP = "123" });
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("14", resultado.Mensaje);
        }

        [TestMethod]
        public void TestProcesarOk()
        {
            var pregate = new PRE_GATE { TRUCK_LICENCE = "", DRIVER_ID = "", STATUS = "I", KIOSK_TRANSACTIONS = new List<KIOSK_TRANSACTION> { new KIOSK_TRANSACTION { IS_OK = true, KIOSK = new KIOSK { IS_IN = true } } }, PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL> { new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 10, TRANSACTION_NUMBER = "123-456-789", REFERENCE_ID = "I", DOCUMENT_ID = "", CONTAINERS = new List<CONTAINER> { new CONTAINER { NUMBER = "" } } }, new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 10, REFERENCE_ID = "E", CONTAINERS = new List<CONTAINER> { new CONTAINER { NUMBER = "" } } } } };
            _mockPreGate.Setup(m => m.ObtenerPreGateConDetalle(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { pregate });
            _mockValidaAduana.Setup(m => m.ObtenerGKeyContenedor(It.IsAny<string>())).Returns(4290331);
            _mockConector.Setup(m => m.Invocacion(_xmlProcessTruckSalida)).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "123" } } });
            _mockTransaccionQuiosco.Setup(m => m.Agregar(It.IsAny<KIOSK_TRANSACTION>()));
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            _mockPreGate.Setup(m => m.Agregar(It.IsAny<PRE_GATE>()));
            _mockN4.Setup(m => m.TieneTransaccionActiva(It.IsAny<string>())).Returns(false);
            _mockConector.Setup(m => m.Invocacion(_xmlTruckVisit)).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockConector.Setup(m => m.Invocacion(_xmlGroovy)).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockConector.Setup(m => m.Invocacion(_xmlStageDone)).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockConector.Setup(m => m.Invocacion(_xmlProcessTruckEntrada)).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockConector.Setup(m => m.Invocacion(_xmlStageDone1)).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockConector.Setup(m => m.Invocacion(_xmlStageDone2)).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = @"<argo:gate-response xmlns:argo=""http://www.navis.com/argo"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://www.navis.com/argo GateWebserviceResponse.xsd""><stage-done-response><truck-visit tv-key=""3205814"" gos-tv-key=""-50088"" is-internal=""false"" next-stage-id=""mty_out"" status=""OK"" gate-id=""RECEPTIO_MTY"" entered=""2018-12-19T11:48:09""><trucking-co id=""0992724617001"" /><truck id=""9590"" tag-id=""17601484"" license-nbr=""KBA0730"" /><chassis-profile id=""40"" /><driver card-id=""1600185555"" driver-name=""GIOVANNY OLDEMAR VILLACIS NAVEDA"" /></truck-visit><truck-transactions><truck-transaction tran-key=""9732773"" tran-nbr=""3393262"" tv-key=""3205814"" tran-type=""PUM"" category=""IMPRT"" freight-kind=""MTY"" mission=""1A16"" block-id=""1A"" next-stage-id=""mty_out"" gate-id=""RECEPTIO_MTY"" order-nbr=""MSC"" status=""OK"" notes=""Transaction done by Kiosco #172.16.9.25"" is-hazard=""false""><container eqid=""CXRU1197672"" type=""45R1"" chs-is-owners=""false"" seal-1="""" seal-2="""" seal-3="""" seal-4="""" is-sealed=""false"" gross-weight=""4620.0"" safe-weight=""34000.0"" tare-weight=""4620.0"" line-id=""MSC"" departure-order-nbr=""MSC"" has-documents=""false"" slot=""1A16E.4"" block-id=""1A"" unit-flex-string-1=""029083902018CI000276P"" unit-flex-string-4=""WITHOUT"" unit-flex-string-6=""4620"" unit-flex-string-7=""4000.0"" unit-flex-string-10=""MEDUGU117982"" ufv-flex-string-9=""PABLB"" owner-id=""MSC"" operator-id=""MSC""><accessory /><routing destination="""" pol=""PAROD"" pod=""ECGYE"" /></container><eq-order order-nbr=""MSC"" order-type=""EDO"" line-id=""MSC"" freight-kind=""MTY""><eq-order-items><eq-order-item eq-length=""40.0"" eq-length-name=""40'"" eq-iso-group=""RE"" eq-iso-group-name=""Refrigerated container"" eq-height=""9.501312335958005"" eq-height-name=""9'6&quot;"" /></eq-order-items></eq-order></truck-transaction></truck-transactions></stage-done-response></argo:gate-response>" } } });
            var resultado = _servicio.Procesar(0, new KIOSK { NAME = "PRUEBA", IP = "123" });
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("13", resultado.Mensaje);
        }
    }
}

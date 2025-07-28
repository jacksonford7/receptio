using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RECEPTIO.CapaAplicacion.Transaction.Aplicacion.Contratos;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Repositorio;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using RECEPTIO.CapaDominio.Nucleo.ServiciosDominio.N4;
using RECEPTIO.CapaDominio.Nucleo.ServiciosDominio.ServicioN4;
using RECEPTIO.CapaDominio.Transaction.ServiciosDominio;
using System.Collections.Generic;

namespace RECEPTIO.CapaDominio.Tests.Transaction.ServicioDominio
{
    [TestClass]
    public class TestServicioN4
    {
        private ServicioN4 _servicio;
        private Mock<IConector> _mock;
        private Mock<IRepositorioTransaccionQuiosco> _mockTransaccionQuiosco;
        private Mock<IRepositorioPreGate> _mockPreGate;
        private Mock<IRepositorioValidaAduana> _mockValidaAduana;
        private DatosEntradaN4 _datos;
        private DatosEntradaN4 _datosT4;
        private DatosEntradaN4 _datosReceiveExportBanano;
        private DatosEntradaN4 _datosReceiveExportBananoCfs;
        private DatosEntradaN4 _datosReceiveEmptyMix;
        private DatosEntradaN4 _datosEntradaProveedores;
        private DatosPreGateSalida _datosSalidaDeliveryImport;
        private DatosPreGateSalida _datosSalidaReceiveExport;
        private DatosPreGateSalida _datosSalidaReceiveExportBrBk;
        private DatosPreGateSalida _datosSalidaReceiveExportCfs;
        private DatosPreGateSalida _datosSalidaReceiveEmpty;
        private DatosPreGateSalida _datosSalidaReceiveBanano;
        private DatosPreGateSalida _datosSalidaReceiveBananoCfs;
        private DatosPreGateSalida _datosSalidaReceiveEmptyMix;
        private DatosPreGateSalida _datosSalidaDeliveryEmpty;
        private DatosPreGateSalida _datosSalidaProveedores;
        private DatosPreGateSalida _datosSalidaNoContempladas;

        [TestInitialize]
        public void Inicializar()
        {
            _mock = new Mock<IConector>();
            _mockTransaccionQuiosco = new Mock<IRepositorioTransaccionQuiosco>();
            _mockPreGate = new Mock<IRepositorioPreGate>();
            _mockValidaAduana = new Mock<IRepositorioValidaAduana>();
            _servicio = new ServicioN4(_mock.Object, _mockTransaccionQuiosco.Object, _mockPreGate.Object, _mockValidaAduana.Object);
            _datos = new DatosEntradaN4
            {
                CedulaChofer = "132456789",
                NombreQuiosco = "KIOSKO XXX",
                IdTransaccion = "65415",
                PlacaVehiculo = "GQU0899",
                TipoTransaccion = 2
            };
            _datosT4 = new DatosEntradaN4
            {
                CedulaChofer = "132456789",
                NombreQuiosco = "KIOSKO XXX",
                IdTransaccion = "65415",
                PlacaVehiculo = "GQU0899",
                Peso = 4546,
                TipoTransaccion = 4
            };
            _datosReceiveExportBanano = new DatosEntradaN4
            {
                CedulaChofer = "132456789",
                NombreQuiosco = "KIOSKO XXX",
                IdTransaccion = "65415",
                PlacaVehiculo = "GQU0899",
                Peso = 4546,
                TipoTransaccion = 8
            };
            _datosReceiveExportBananoCfs = new DatosEntradaN4
            {
                CedulaChofer = "132456789",
                NombreQuiosco = "KIOSKO XXX",
                IdTransaccion = "65415",
                PlacaVehiculo = "GQU0899",
                Peso = 4546,
                TipoTransaccion = 9
            };
            _datosReceiveEmptyMix = new DatosEntradaN4
            {
                CedulaChofer = "132456789",
                NombreQuiosco = "KIOSKO XXX",
                IdTransaccion = "65415",
                PlacaVehiculo = "GQU0899",
                Peso = 0,
                TipoTransaccion = 10
            };
            _datosEntradaProveedores = new DatosEntradaN4
            {
                CedulaChofer = "132456789",
                NombreQuiosco = "KIOSKO XXX",
                IdTransaccion = "65415",
                PlacaVehiculo = "GQU0899",
                Peso = 0,
                TipoTransaccion = 12
            };
            _datosSalidaDeliveryImport = new DatosPreGateSalida
            {
                PreGate = new PRE_GATE
                {
                    DRIVER_ID = "1234567890", TRUCK_LICENCE = "XXX666",
                    PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL> { new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 2} }
                },
                NombreQuiosco = "KIOSKO XXX",
                IdTransaccion = "65415",
                TipoTransaccion = 2
            };
            _datosSalidaReceiveExport = new DatosPreGateSalida
            {
                PreGate = new PRE_GATE
                {
                    DRIVER_ID = "1234567890",
                    TRUCK_LICENCE = "XXX666",
                    PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL> { new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 3, CONTAINERS = new List<CONTAINER> { new CONTAINER { NUMBER = "XXX"} } } }
                },
                NombreQuiosco = "KIOSKO XXX",
                IdTransaccion = "65415",
                TipoTransaccion = 3
            };
            _datosSalidaReceiveEmpty = new DatosPreGateSalida
            {
                PreGate = new PRE_GATE
                {
                    DRIVER_ID = "1234567890",
                    TRUCK_LICENCE = "XXX666",
                    PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL> { new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 7, CONTAINERS = new List<CONTAINER> { new CONTAINER { NUMBER = "XXX" } } } }
                },
                NombreQuiosco = "KIOSKO XXX",
                IdTransaccion = "65415",
                TipoTransaccion = 7
            };
            _datosSalidaReceiveExportBrBk = new DatosPreGateSalida
            {
                PreGate = new PRE_GATE
                {
                    DRIVER_ID = "1234567890",
                    TRUCK_LICENCE = "XXX666",
                    PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL> { new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 4, TRANSACTION_NUMBER = "" } }
                },
                NombreQuiosco = "KIOSKO XXX",
                IdTransaccion = "65415",
                TipoTransaccion = 4
            };
            _datosSalidaReceiveExportCfs = new DatosPreGateSalida
            {
                PreGate = new PRE_GATE
                {
                    DRIVER_ID = "1234567890",
                    TRUCK_LICENCE = "XXX666",
                    PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL> { new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 5, TRANSACTION_NUMBER = "" } }
                },
                NombreQuiosco = "KIOSKO XXX",
                IdTransaccion = "65415",
                TipoTransaccion = 5
            };
            _datosSalidaReceiveBanano = new DatosPreGateSalida
            {
                PreGate = new PRE_GATE
                {
                    DRIVER_ID = "1234567890",
                    TRUCK_LICENCE = "XXX666",
                    PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL> { new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 8, TRANSACTION_NUMBER = "" } }
                },
                NombreQuiosco = "KIOSKO XXX",
                IdTransaccion = "65415",
                TipoTransaccion = 8
            };
            _datosSalidaReceiveBananoCfs = new DatosPreGateSalida
            {
                PreGate = new PRE_GATE
                {
                    DRIVER_ID = "1234567890",
                    TRUCK_LICENCE = "XXX666",
                    PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL> { new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 9, TRANSACTION_NUMBER = "" } }
                },
                NombreQuiosco = "KIOSKO XXX",
                IdTransaccion = "65415",
                TipoTransaccion = 9
            };
            _datosSalidaReceiveEmptyMix = new DatosPreGateSalida
            {
                PreGate = new PRE_GATE
                {
                    DRIVER_ID = "1234567890",
                    TRUCK_LICENCE = "XXX666",
                    PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL> { new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 10, TRANSACTION_NUMBER = "", CONTAINERS = new List<CONTAINER> { new CONTAINER { NUMBER = "XXX" } } } }
                },
                NombreQuiosco = "KIOSKO XXX",
                IdTransaccion = "65415",
                TipoTransaccion = 10
            };
            _datosSalidaDeliveryEmpty = new DatosPreGateSalida
            {
                PreGate = new PRE_GATE
                {
                    DRIVER_ID = "1234567890",
                    TRUCK_LICENCE = "XXX666",
                    PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL> { new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 11, TRANSACTION_NUMBER = "" } }
                },
                NombreQuiosco = "KIOSKO XXX",
                IdTransaccion = "65415",
                TipoTransaccion = 11
            };
            _datosSalidaProveedores = new DatosPreGateSalida
            {
                PreGate = new PRE_GATE
                {
                    DRIVER_ID = "1234567890",
                    TRUCK_LICENCE = "XXX666",
                    PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL> { new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 13, TRANSACTION_NUMBER = "" } }
                },
                NombreQuiosco = "KIOSKO XXX",
                IdTransaccion = "65415",
                TipoTransaccion = 13
            };
            _datosSalidaNoContempladas = new DatosPreGateSalida
            {
                PreGate = new PRE_GATE
                {
                    DRIVER_ID = "1234567890",
                    TRUCK_LICENCE = "XXX666",
                    PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL> { new PRE_GATE_DETAIL { TRANSACTION_TYPE_ID = 2, CONTAINERS = new List<CONTAINER> { new CONTAINER { NUMBER = "XXX" } } } }
                },
                NombreQuiosco = "KIOSKO XXX",
                IdTransaccion = "65415",
                TipoTransaccion = 55
            };
        }

        [TestMethod]
        public void TestEjecutarProcesosEntradaT12367CuandoFalla()
        {
            _mock.Setup(m => m.Invocacion(EstadoProcessTruckFull.ArmarXml(_datos))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 2 });
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "P" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            var resultado = _servicio.EjecutarProcesosEntrada(_datos);
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("14", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosT12367Ok()
        {
            _mock.Setup(m => m.Invocacion(EstadoProcessTruckFull.ArmarXml(_datos))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = ""} } });
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "P" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            var resultado = _servicio.EjecutarProcesosEntrada(_datos);
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("13", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosEntradaT45CuandoFallaGroovy()
        {
            _mock.Setup(m => m.Invocacion(EstadoGroovyPesajeEntrada.ArmarXml(_datosT4))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 2 });
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "P" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            var resultado = _servicio.EjecutarProcesosEntrada(_datosT4);
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("14", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosEntradaT45CuandoFallaProcessTruck()
        {
            _mock.Setup(m => m.Invocacion(EstadoGroovyPesajeEntrada.ArmarXml(_datosT4))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mock.Setup(m => m.Invocacion(EstadoProcessTruckFull.ArmarXml(_datosT4))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 2 });
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "P" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            var resultado = _servicio.EjecutarProcesosEntrada(_datosT4);
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("14", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosEntradaT45Ok()
        {
            _mock.Setup(m => m.Invocacion(EstadoGroovyPesajeEntrada.ArmarXml(_datosT4))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mock.Setup(m => m.Invocacion(EstadoProcessTruckFull.ArmarXml(_datosT4))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "P" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            var resultado = _servicio.EjecutarProcesosEntrada(_datosT4);
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("13", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosEntradaT8CuandoFallaGroovy()
        {
            _mock.Setup(m => m.Invocacion(EstadoGroovyPesajeEntradaBanano.ArmarXml(_datosReceiveExportBanano))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 2 });
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "P" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            var resultado = _servicio.EjecutarProcesosEntrada(_datosReceiveExportBanano);
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("14", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosEntradaT8CuandoFallaProcessTruck()
        {
            _mock.Setup(m => m.Invocacion(EstadoGroovyPesajeEntradaBanano.ArmarXml(_datosReceiveExportBanano))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mock.Setup(m => m.Invocacion(EstadoProcessTruckFullBanano.ArmarXml(_datosReceiveExportBanano))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 2 });
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "P" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            var resultado = _servicio.EjecutarProcesosEntrada(_datosReceiveExportBanano);
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("14", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosEntradaT8Ok()
        {
            _mock.Setup(m => m.Invocacion(EstadoGroovyPesajeEntradaBanano.ArmarXml(_datosReceiveExportBanano))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mock.Setup(m => m.Invocacion(EstadoProcessTruckFullBanano.ArmarXml(_datosReceiveExportBanano))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "P" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            var resultado = _servicio.EjecutarProcesosEntrada(_datosReceiveExportBanano);
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("13", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosEntradaT9CuandoFallaProcessTruck()
        {
            _mock.Setup(m => m.Invocacion(EstadoProcessTruckFullBanano.ArmarXml(_datosReceiveExportBananoCfs))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 2 });
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "P" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            var resultado = _servicio.EjecutarProcesosEntrada(_datosReceiveExportBananoCfs);
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("14", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosEntradaT9Ok()
        {
            _mock.Setup(m => m.Invocacion(EstadoProcessTruckFullBanano.ArmarXml(_datosReceiveExportBananoCfs))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "P" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            var resultado = _servicio.EjecutarProcesosEntrada(_datosReceiveExportBananoCfs);
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("13", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosEntradaT10CuandoFalla()
        {
            _mock.Setup(m => m.Invocacion(EstadoProcessTruckFull.ArmarXml(_datosReceiveEmptyMix))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 2 });
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "P" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            var resultado = _servicio.EjecutarProcesosEntrada(_datosReceiveEmptyMix);
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("14", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosT10Ok()
        {
            _mock.Setup(m => m.Invocacion(EstadoProcessTruckFull.ArmarXml(_datosReceiveEmptyMix))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "P" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            var resultado = _servicio.EjecutarProcesosEntrada(_datosReceiveEmptyMix);
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("13", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosProveedoresOk()
        {
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "P" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            var resultado = _servicio.EjecutarProcesosEntrada(_datosEntradaProveedores);
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("13", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosSalidaDeliveryImportT126CuandoFalla()
        {
            _mock.Setup(m => m.Invocacion(EstadoSalidaProcessTruckFull.ArmarXml(_datosSalidaDeliveryImport))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 3 });
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "L" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            var resultado = _servicio.EjecutarProcesosSalida(_datosSalidaDeliveryImport);
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("14", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosSalidaDeliveryImportT126Ok()
        {
            _mock.Setup(m => m.Invocacion(EstadoSalidaProcessTruckFull.ArmarXml(_datosSalidaDeliveryImport))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 1, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "P" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            var resultado = _servicio.EjecutarProcesosSalida(_datosSalidaDeliveryImport);
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("13", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosSalidaReceiveExportCuandoFallaIie()
        {
            _mockValidaAduana.Setup(m => m.ObtenerGKeyContenedor(It.IsAny<string>())).Returns(81);
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "P" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            var resultado = _servicio.EjecutarProcesosSalida(_datosSalidaReceiveExport);
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("14", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosSalidaReceiveExportCuandoFallaProcessTruck()
        {
            _mockValidaAduana.Setup(m => m.ObtenerGKeyContenedor(It.IsAny<string>())).Returns(4290331);
            _mock.Setup(m => m.Invocacion(EstadoSalidaProcessTruckFull.ArmarXml(_datosSalidaDeliveryImport))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 3 });
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "P" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            var resultado = _servicio.EjecutarProcesosSalida(_datosSalidaReceiveExport);
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("14", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosSalidaReceiveExportOk()
        {
            _mockValidaAduana.Setup(m => m.ObtenerGKeyContenedor(It.IsAny<string>())).Returns(4290331);
            _mock.Setup(m => m.Invocacion(EstadoSalidaProcessTruckFull.ArmarXml(_datosSalidaReceiveExport))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 1, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "I" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            var resultado = _servicio.EjecutarProcesosSalida(_datosSalidaReceiveExport);
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("13", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosSalidaCuandoTieneTransaccionesNoContempladas()
        {
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "P" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            var resultado = _servicio.EjecutarProcesosSalida(_datosSalidaNoContempladas);
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("24", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosSalidaReceiveExportBrBkCuandoFallaPesaje()
        {
            _mock.Setup(m => m.Invocacion(EstadoGroovyPesajeSalida.ArmarXml(_datosSalidaReceiveExportBrBk))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 2, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "P" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            var resultado = _servicio.EjecutarProcesosSalida(_datosSalidaReceiveExportBrBk);
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("14", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosSalidaReceiveExportBrBkCuandoFallaIie()
        {
            _mock.Setup(m => m.Invocacion(EstadoGroovyPesajeSalida.ArmarXml(_datosSalidaReceiveExportBrBk))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "P" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            var resultado = _servicio.EjecutarProcesosSalida(_datosSalidaReceiveExportBrBk);
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("14", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosSalidaReceiveExportBrBkCuandoFallaProcessTruck()
        {
            _mock.Setup(m => m.Invocacion(EstadoGroovyPesajeSalida.ArmarXml(_datosSalidaReceiveExportBrBk))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mock.Setup(m => m.Invocacion(EstadoSalidaProcessTruckFull.ArmarXml(_datosSalidaReceiveExportBrBk))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 3 });
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "P" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            var resultado = _servicio.EjecutarProcesosSalida(_datosSalidaReceiveExportBrBk);
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("14", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosSalidaReceiveExportBrBkOk()
        {
            _mock.Setup(m => m.Invocacion(EstadoGroovyPesajeSalida.ArmarXml(_datosSalidaReceiveExportBrBk))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mock.Setup(m => m.Invocacion(EstadoSalidaProcessTruckFull.ArmarXml(_datosSalidaReceiveExportBrBk))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 1, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "P" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            var resultado = _servicio.EjecutarProcesosSalida(_datosSalidaReceiveExportBrBk);
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("13", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosSalidaReceiveExportCfsCuandoFallaPesaje()
        {
            _mock.Setup(m => m.Invocacion(EstadoGroovyPesajeSalida.ArmarXml(_datosSalidaReceiveExportCfs))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 2, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "P" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            var resultado = _servicio.EjecutarProcesosSalida(_datosSalidaReceiveExportCfs);
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("14", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosSalidaReceiveExportCfsCuandoFallaProcessTruck()
        {
            _mock.Setup(m => m.Invocacion(EstadoGroovyPesajeSalida.ArmarXml(_datosSalidaReceiveExportCfs))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mock.Setup(m => m.Invocacion(EstadoSalidaProcessTruckFull.ArmarXml(_datosSalidaReceiveExportCfs))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 3 });
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "P" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            var resultado = _servicio.EjecutarProcesosSalida(_datosSalidaReceiveExportCfs);
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("14", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosSalidaReceiveExportCfsOk()
        {
            _mock.Setup(m => m.Invocacion(EstadoGroovyPesajeSalida.ArmarXml(_datosSalidaReceiveExportCfs))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mock.Setup(m => m.Invocacion(EstadoSalidaProcessTruckFull.ArmarXml(_datosSalidaReceiveExportCfs))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 1, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "P" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            var resultado = _servicio.EjecutarProcesosSalida(_datosSalidaReceiveExportCfs);
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("13", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosSalidaReceiveEmptyCuandoFallaImdt()
        {
            _mockValidaAduana.Setup(m => m.ObtenerGKeyContenedor(It.IsAny<string>())).Returns(81);
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "P" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            var resultado = _servicio.EjecutarProcesosSalida(_datosSalidaReceiveEmpty);
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("14", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosSalidaReceiveEmptyCuandoFallaProcessTruck()
        {
            _mockValidaAduana.Setup(m => m.ObtenerGKeyContenedor(It.IsAny<string>())).Returns(4290331);
            _mock.Setup(m => m.Invocacion(EstadoSalidaProcessTruckFull.ArmarXml(_datosSalidaReceiveEmpty))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 3 });
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "P" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            var resultado = _servicio.EjecutarProcesosSalida(_datosSalidaReceiveExport);
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("14", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosSalidaReceiveEmptyOk()
        {
            _mockValidaAduana.Setup(m => m.ObtenerGKeyContenedor(It.IsAny<string>())).Returns(4290331);
            _mock.Setup(m => m.Invocacion(EstadoSalidaProcessTruckFull.ArmarXml(_datosSalidaReceiveExport))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 1, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "I" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            var resultado = _servicio.EjecutarProcesosSalida(_datosSalidaReceiveEmpty);
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("13", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosSalidaReceiveExportBananoCuandoFallaIie()
        {
            _datosSalidaReceiveExportBrBk.NombreQuiosco = "/>>>>";
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "P" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            var resultado = _servicio.EjecutarProcesosSalida(_datosSalidaReceiveBanano);
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("14", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosSalidaReceiveBananoCuandoFallaProcessTruck()
        {
            _mock.Setup(m => m.Invocacion(EstadoSalidaProcessTruckFullBanano.ArmarXml(_datosSalidaReceiveBanano))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 3 });
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "P" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            var resultado = _servicio.EjecutarProcesosSalida(_datosSalidaReceiveBanano);
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("14", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosSalidaReceiveBananoOk()
        {
            _mock.Setup(m => m.Invocacion(EstadoSalidaProcessTruckFullBanano.ArmarXml(_datosSalidaReceiveBanano))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 1, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "I" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            var resultado = _servicio.EjecutarProcesosSalida(_datosSalidaReceiveBanano);
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("13", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosSalidaReceiveBananoCfsCuandoFallaProcessTruck()
        {
            _mock.Setup(m => m.Invocacion(EstadoSalidaProcessTruckFullBanano.ArmarXml(_datosSalidaReceiveBananoCfs))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 3 });
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "P" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            var resultado = _servicio.EjecutarProcesosSalida(_datosSalidaReceiveBananoCfs);
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("14", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosSalidaReceiveBananoCfsOk()
        {
            _mock.Setup(m => m.Invocacion(EstadoSalidaProcessTruckFullBanano.ArmarXml(_datosSalidaReceiveBananoCfs))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 1, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "I" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            var resultado = _servicio.EjecutarProcesosSalida(_datosSalidaReceiveBananoCfs);
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("13", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosSalidaReceiveEmptyMixCuandoFallaProcessTruck()
        {
            _mock.Setup(m => m.Invocacion(EstadoSalidaProcessTruckFull.ArmarXml(_datosSalidaReceiveEmptyMix))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 3 });
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "P" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            var resultado = _servicio.EjecutarProcesosSalida(_datosSalidaReceiveEmptyMix);
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("14", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosSalidaReceiveEmptyMixOk()
        {
            _mock.Setup(m => m.Invocacion(EstadoSalidaProcessTruckFull.ArmarXml(_datosSalidaReceiveEmptyMix))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 1, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "I" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            var resultado = _servicio.EjecutarProcesosSalida(_datosSalidaReceiveEmptyMix);
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("13", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosSalidaDeliveryEmptyCuandoFallaProcessTruck()
        {
            _mock.Setup(m => m.Invocacion(EstadoSalidaProcessTruckDeliveryEmpty.ArmarXml(_datosSalidaDeliveryEmpty))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 3 });
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "P" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            var resultado = _servicio.EjecutarProcesosSalida(_datosSalidaDeliveryEmpty);
            Assert.IsFalse(resultado.FueOk);
            Assert.AreEqual("14", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosSalidaDeliveryEmptOk()
        {
            _mock.Setup(m => m.Invocacion(EstadoSalidaProcessTruckDeliveryEmpty.ArmarXml(_datosSalidaDeliveryEmpty))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 1, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "I" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            var resultado = _servicio.EjecutarProcesosSalida(_datosSalidaDeliveryEmpty);
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("13", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosSalidaProveedoresOk()
        {
            _mockPreGate.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroPreGatePorId>())).Returns(new List<PRE_GATE> { new PRE_GATE { STATUS = "P" } });
            _mockPreGate.Setup(m => m.Actualizar(It.IsAny<PRE_GATE>()));
            var resultado = _servicio.EjecutarProcesosSalida(_datosSalidaProveedores);
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("13", resultado.Mensaje);
        }
    }
}

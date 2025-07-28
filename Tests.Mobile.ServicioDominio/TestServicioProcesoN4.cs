using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RECEPTIO.CapaAplicacion.Mobile.Aplicacion.Contratos;
using RECEPTIO.CapaDominio.Mobile.Dominio.InterfacesRepositorios;
using RECEPTIO.CapaDominio.Mobile.ServiciosDominio;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using RECEPTIO.CapaDominio.Nucleo.ServiciosDominio.N4;
using RECEPTIO.CapaDominio.Nucleo.ServiciosDominio.ServicioN4;
using System.Collections.Generic;

namespace RECEPTIO.CapaDominio.Tests.Mobile.ServicioDominio
{
    [TestClass]
    public class TestServicioProcesoN4
    {
        private ServicioProcesoN4 _servicio;
        private Mock<IRepositorioTosProccess> _mockTosProccess;
        private Mock<IConector> _mock;
        private DatosN4 _datos;
        private DatosDeliveryImportBrbkCfs _datosBrBkCfs;
        private DatosReceiveExport _datosReceiveExport;
        private DatosReceiveExportBrBk _datosReceiveExportBrBk;
        private DatosReceiveExportBanano _datosReceiveExportBanano;
        private DatosDeliveryImportP2D _datosP2D;

        [TestInitialize]
        public void Inicializar()
        {
            _mockTosProccess = new Mock<IRepositorioTosProccess>();
            _mock = new Mock<IConector>();
            _servicio = new ServicioProcesoN4(_mockTosProccess.Object, _mock.Object);
            _datos = new DatosN4
            {
                CedulaChofer = "132456789",
                IdCompania = "09011045600111",
                IdPreGate = "65415",
                NumerosTransacciones = new List<string> { "423432", "656565665"},
                PlacaVehiculo = "GQU0899"
            };
            _datosBrBkCfs = new DatosDeliveryImportBrbkCfs
            {
                CedulaChofer = "132456789",
                IdCompania = "09011045600111",
                IdPreGate = "65415",
                NumerosTransacciones = new List<string> { "423432", "656565665" },
                PlacaVehiculo = "GQU0899",
                Bl = "CEC2018SMLU0042-0004-0080"
            };
            _datosP2D = new DatosDeliveryImportP2D
            {
                CedulaChofer = "0915469985",
                IdCompania = "0991456848001",
                IdPreGate = "2022",
                NumerosTransacciones = new List<string> { "219837", "219846" },
                PlacaVehiculo = "GBO1002",
                DatosTransaccionP2D = new List<DatosBLP2D> { new DatosBLP2D() { NumeroTransaccion = "219837", Bl = "CEC2018KOSU0033-0029-0007", Qty ="1" },
                                                             new DatosBLP2D() { NumeroTransaccion = "219846", Bl = "CEC2019CMAU0030-0001-0000", Qty = "1" } }
                
            };
            _datosReceiveExport = new DatosReceiveExport
            {
                CedulaChofer = "132456789",
                IdCompania = "09011045600111",
                IdPreGate = "65415",
                PlacaVehiculo = "GQU0899",
                DataContenedores = new List<DatosContenedor>
                {
                    new DatosContenedor{ Contenedor = new CONTAINER{ NUMBER = "XXX" }, Linea = "HSD", Aisv ="SE45615", Iso = "9000", Tamano = "40", TipoCarga = "FCL" },
                    new DatosContenedor{ Contenedor = new CONTAINER{ NUMBER = "YYY", SEALS = new List<SEAL>{ new SEAL { CAPTION = "SEAL-1", VALUE = "4545"}, new SEAL { CAPTION = "SEAL-3", VALUE = "48898" } } }, Linea = "MSK", Aisv ="GGHHH", Iso = "9001", Tamano = "20", TipoCarga = "FCL" },
                    new DatosContenedor{ Contenedor = new CONTAINER{ NUMBER = "ZZZ", SEALS = new List<SEAL>{ new SEAL { CAPTION = "SEAL-2", VALUE = "666"}, new SEAL { CAPTION = "SEAL-4", VALUE = "123" } }, DAMAGES = new List<DAMAGE> { new DAMAGE { COMPONENT = "6666", DAMAGE_TYPE = "fg", SEVERITY = "MINOR", NOTES = "erwdvc" }, new DAMAGE { COMPONENT = "4562", DAMAGE_TYPE = "F3", SEVERITY = "MAYOR", NOTES = "MANGA" } } }, Linea = "MSC", Aisv ="SERFF", Iso = "90012", Tamano = "40", TipoCarga = "LCL" }
                }
            };
            _datosReceiveExportBrBk = new DatosReceiveExportBrBk
            {
                CedulaChofer = "132456789",
                IdCompania = "09011045600111",
                IdPreGate = "65415",
                PlacaVehiculo = "GQU0899",
                Bl = "CEC2018SMLU0042-0004-0080",
                Ip = "172.16.2.55",
                Cantidad = 363,
                Notas = "BLA BLA BLA",
                VeeselVisit = "CGSAKL7889798798",
                Dae = "dasdasddasd"
            };
            _datosReceiveExportBanano = new DatosReceiveExportBanano
            {
                CedulaChofer = "132456789",
                IdCompania = "09011045600111",
                IdPreGate = "65415",
                PlacaVehiculo = "GQU0899",
                Bl = "CEC2018SMLU0042-0004-0080",
                Ip = "172.16.2.55",
                Cantidad = 363,
                Notas = "BLA BLA BLA",
                VeeselVisit = "CGSAKL7889798798",
                Dae = "dasdasddasd"
            };
            _mockTosProccess.Setup(m => m.Agregar(It.IsAny<TOS_PROCCESS>()));
        }

        [TestMethod]
        public void TestEjecutarProcesosDeliveryImportFullCuandoFallaTruckVisit()
        {
            _mock.Setup(m => m.Invocacion(EstadoTruckVisitImportFull.ArmarXml(_datos))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 2, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            var resultado = _servicio.EjecutarProcesosDeliveryImportFull(_datos);
            Assert.IsFalse(resultado.FueOk);
            Assert.IsTrue(resultado.Mensaje.Contains("N4 RETORNO ERROR EN CREATE TRUCK VISIT"));
        }

        [TestMethod]
        public void TestEjecutarProcesosDeliveryImportFullCuandoFallaSubmit()
        {
            _mock.Setup(m => m.Invocacion(EstadoTruckVisitImportFull.ArmarXml(_datos))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 1, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mock.Setup(m => m.Invocacion(EstadoSubmitImportFull.ArmarXml(_datos.GosTvKey, "423432"))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 3, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            var resultado = _servicio.EjecutarProcesosDeliveryImportFull(_datos);
            Assert.IsFalse(resultado.FueOk);
            Assert.IsTrue(resultado.Mensaje.Contains("N4 RETORNO ERROR EN SUBMIT"));
        }

        [TestMethod]
        public void TestEjecutarProcesosDeliveryImportFullCuandoFallaStageDone()
        {
            _mock.Setup(m => m.Invocacion(EstadoTruckVisitImportFull.ArmarXml(_datos))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mock.Setup(m => m.Invocacion(EstadoSubmitImportFull.ArmarXml(_datos.GosTvKey, "423432"))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mock.Setup(m => m.Invocacion(EstadoSubmitImportFull.ArmarXml(_datos.GosTvKey, "656565665"))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mock.Setup(m => m.Invocacion(EstadoStageDoneImportFull.ArmarXml(_datos))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = false, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            var resultado = _servicio.EjecutarProcesosDeliveryImportFull(_datos);
            Assert.IsFalse(resultado.FueOk);
            Assert.IsTrue(resultado.Mensaje.Contains("N4 RETORNO ERROR EN STAGE DONE"));
        }

        [TestMethod]
        public void TestEjecutarProcesosDeliveryImportFullOk()
        {
            _mock.Setup(m => m.Invocacion(EstadoTruckVisitImportFull.ArmarXml(_datos))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mock.Setup(m => m.Invocacion(EstadoSubmitImportFull.ArmarXml(_datos.GosTvKey, "423432"))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mock.Setup(m => m.Invocacion(EstadoSubmitImportFull.ArmarXml(_datos.GosTvKey, "656565665"))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mock.Setup(m => m.Invocacion(EstadoStageDoneImportFull.ArmarXml(_datos))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            var resultado = _servicio.EjecutarProcesosDeliveryImportFull(_datos);
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("Proceso Ok.", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosDeliveryBrBkCfsCuandoFallaTruckVisit()
        {
            _mock.Setup(m => m.Invocacion(EstadoTruckVisitImportBrBkCfs.ArmarXml(_datosBrBkCfs))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 2, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            var resultado = _servicio.EjecutarProcesosDeliveryImportBrBkCfs(_datosBrBkCfs);
            Assert.IsFalse(resultado.FueOk);
            Assert.IsTrue(resultado.Mensaje.Contains("N4 RETORNO ERROR EN CREATE TRUCK VISIT"));
        }

        [TestMethod]
        public void TestEjecutarProcesosDeliveryImportBrBkCfsCuandoFallaSubmit()
        {
            _mock.Setup(m => m.Invocacion(EstadoTruckVisitImportBrBkCfs.ArmarXml(_datosBrBkCfs))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 1, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mock.Setup(m => m.Invocacion(EstadoGroovyImportBrBkCfs.ArmarXml(_datosBrBkCfs, "423432"))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 3, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            var resultado = _servicio.EjecutarProcesosDeliveryImportBrBkCfs(_datosBrBkCfs);
            Assert.IsFalse(resultado.FueOk);
            Assert.IsTrue(resultado.Mensaje.Contains("N4 RETORNO ERROR EN GROOVY"));
        }

        [TestMethod]
        public void TestEjecutarProcesosDeliveryImportBrBkCfsCuandoFallaStageDone()
        {
            _mock.Setup(m => m.Invocacion(EstadoTruckVisitImportBrBkCfs.ArmarXml(_datosBrBkCfs))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mock.Setup(m => m.Invocacion(EstadoGroovyImportBrBkCfs.ArmarXml(_datosBrBkCfs, "423432"))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mock.Setup(m => m.Invocacion(EstadoGroovyImportBrBkCfs.ArmarXml(_datosBrBkCfs, "656565665"))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mock.Setup(m => m.Invocacion(EstadoStageDoneImportBrBkCfs.ArmarXml(_datosBrBkCfs))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = false, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            var resultado = _servicio.EjecutarProcesosDeliveryImportBrBkCfs(_datosBrBkCfs);
            Assert.IsFalse(resultado.FueOk);
            Assert.IsTrue(resultado.Mensaje.Contains("N4 RETORNO ERROR EN STAGE DONE"));
        }

        [TestMethod]
        public void TestEjecutarProcesosDeliveryImportBrBkCfsOk()
        {
            _mock.Setup(m => m.Invocacion(EstadoTruckVisitImportBrBkCfs.ArmarXml(_datosBrBkCfs))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mock.Setup(m => m.Invocacion(EstadoGroovyImportBrBkCfs.ArmarXml(_datosBrBkCfs, "423432"))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mock.Setup(m => m.Invocacion(EstadoGroovyImportBrBkCfs.ArmarXml(_datosBrBkCfs, "656565665"))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mock.Setup(m => m.Invocacion(EstadoStageDoneImportBrBkCfs.ArmarXml(_datosBrBkCfs))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            var resultado = _servicio.EjecutarProcesosDeliveryImportBrBkCfs(_datosBrBkCfs);
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("Proceso Ok.", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosReceiveExportCuandoFallaTruckVisit()
        {
            _mock.Setup(m => m.Invocacion(EstadoTruckVisitReceiveExport.ArmarXml(_datosReceiveExport))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 2, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            var resultado = _servicio.EjecutarProcesosReceiveExport(_datosReceiveExport);
            Assert.IsFalse(resultado.FueOk);
            Assert.IsTrue(resultado.Mensaje.Contains("N4 RETORNO ERROR EN CREATE TRUCK VISIT"));
        }

        [TestMethod]
        public void TestEjecutarProcesosReceiveExportCuandoFallaSubmit()
        {
            _mock.Setup(m => m.Invocacion(EstadoTruckVisitReceiveExport.ArmarXml(_datosReceiveExport))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 1, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mock.Setup(m => m.Invocacion(EstadoSubmitReceiveExport.ArmarXml(_datosReceiveExport))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 3, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            var resultado = _servicio.EjecutarProcesosReceiveExport(_datosReceiveExport);
            Assert.IsFalse(resultado.FueOk);
            Assert.IsTrue(resultado.Mensaje.Contains("N4 RETORNO ERROR EN SUBMIT"));
        }

        [TestMethod]
        public void TestEjecutarProcesosReceiveExportCuandoFallaStageDone()
        {
            _mock.Setup(m => m.Invocacion(EstadoTruckVisitReceiveExport.ArmarXml(_datosReceiveExport))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mock.Setup(m => m.Invocacion(EstadoSubmitReceiveExport.ArmarXml(_datosReceiveExport))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mock.Setup(m => m.Invocacion(EstadoStageDoneReceiveExport.ArmarXml(_datosReceiveExport))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = false, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            var resultado = _servicio.EjecutarProcesosReceiveExport(_datosReceiveExport);
            Assert.IsFalse(resultado.FueOk);
            Assert.IsTrue(resultado.Mensaje.Contains("N4 RETORNO ERROR EN STAGE DONE"));
        }

        [TestMethod]
        public void TestEjecutarProcesosReceiveExportOk()
        {
            _mock.Setup(m => m.Invocacion(EstadoTruckVisitReceiveExport.ArmarXml(_datosReceiveExport))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mock.Setup(m => m.Invocacion(EstadoSubmitReceiveExport.ArmarXml(_datosReceiveExport))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mock.Setup(m => m.Invocacion(EstadoStageDoneReceiveExport.ArmarXml(_datosReceiveExport))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            var resultado = _servicio.EjecutarProcesosReceiveExport(_datosReceiveExport);
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("Proceso Ok.", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosReceiveExportBrBkCuandoFallaTruckVisit()
        {
            _mock.Setup(m => m.Invocacion(EstadoTruckVisitReceiveExportBrBkCfs.ArmarXml(_datosReceiveExportBrBk))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 2, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            var resultado = _servicio.EjecutarProcesosReceiveExportBrBk(_datosReceiveExportBrBk);
            Assert.IsFalse(resultado.FueOk);
            Assert.IsTrue(resultado.Mensaje.Contains("N4 RETORNO ERROR EN CREATE TRUCK VISIT"));
        }

        [TestMethod]
        public void TestEjecutarProcesosReceiveExportBrBkCuandoFallaSubmit()
        {
            _mock.Setup(m => m.Invocacion(EstadoTruckVisitReceiveExportBrBkCfs.ArmarXml(_datosReceiveExportBrBk))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 1, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mock.Setup(m => m.Invocacion(EstadoGroovyReceiveExportBrBkCfs.ArmarXml(_datosReceiveExportBrBk))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 3, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            var resultado = _servicio.EjecutarProcesosReceiveExportBrBk(_datosReceiveExportBrBk);
            Assert.IsFalse(resultado.FueOk);
            Assert.IsTrue(resultado.Mensaje.Contains("N4 RETORNO ERROR EN GROOVY"));
        }

        [TestMethod]
        public void TestEjecutarProcesosReceiveExportBrBkCuandoFallaStageDone()
        {
            _mock.Setup(m => m.Invocacion(EstadoTruckVisitReceiveExportBrBkCfs.ArmarXml(_datosReceiveExportBrBk))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mock.Setup(m => m.Invocacion(EstadoGroovyReceiveExportBrBkCfs.ArmarXml(_datosReceiveExportBrBk))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mock.Setup(m => m.Invocacion(EstadoStageDoneRecieveExportBrBkCfs.ArmarXml(_datosReceiveExportBrBk))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = false, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            var resultado = _servicio.EjecutarProcesosReceiveExportBrBk(_datosReceiveExportBrBk);
            Assert.IsFalse(resultado.FueOk);
            Assert.IsTrue(resultado.Mensaje.Contains("N4 RETORNO ERROR EN STAGE DONE"));
        }

        [TestMethod]
        public void TestEjecutarProcesosReceiveExportBrBkOk()
        {
            _mock.Setup(m => m.Invocacion(EstadoTruckVisitReceiveExportBrBkCfs.ArmarXml(_datosReceiveExportBrBk))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mock.Setup(m => m.Invocacion(EstadoGroovyReceiveExportBrBkCfs.ArmarXml(_datosReceiveExportBrBk))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mock.Setup(m => m.Invocacion(EstadoStageDoneRecieveExportBrBkCfs.ArmarXml(_datosReceiveExportBrBk))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            var resultado = _servicio.EjecutarProcesosReceiveExportBrBk(_datosReceiveExportBrBk);
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("Proceso Ok.", resultado.Mensaje);
        }

        [TestMethod]
        public void TestEjecutarProcesosReceiveExportBananoCuandoFallaTruckVisit()
        {
            _mock.Setup(m => m.Invocacion(EstadoTruckVisitReceiveExportBanano.ArmarXml(_datosReceiveExportBanano))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 2, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            var resultado = _servicio.EjecutarProcesosReceiveExportBanano(_datosReceiveExportBanano);
            Assert.IsFalse(resultado.FueOk);
            Assert.IsTrue(resultado.Mensaje.Contains("N4 RETORNO ERROR EN CREATE TRUCK VISIT"));
        }

        [TestMethod]
        public void TestEjecutarProcesosReceiveExportBananoCuandoFallaSubmit()
        {
            _mock.Setup(m => m.Invocacion(EstadoTruckVisitReceiveExportBanano.ArmarXml(_datosReceiveExportBanano))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 1, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mock.Setup(m => m.Invocacion(EstadoGroovyReceiveExportBanano.ArmarXml(_datosReceiveExportBanano))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 3, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            var resultado = _servicio.EjecutarProcesosReceiveExportBanano(_datosReceiveExportBanano);
            Assert.IsFalse(resultado.FueOk);
            Assert.IsTrue(resultado.Mensaje.Contains("N4 RETORNO ERROR EN GROOVY"));
        }

        [TestMethod]
        public void TestEjecutarProcesosReceiveExportBananoCuandoFallaStageDone()
        {
            _mock.Setup(m => m.Invocacion(EstadoTruckVisitReceiveExportBanano.ArmarXml(_datosReceiveExportBanano))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mock.Setup(m => m.Invocacion(EstadoGroovyReceiveExportBanano.ArmarXml(_datosReceiveExportBanano))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mock.Setup(m => m.Invocacion(EstadoStageDoneRecieveExportBanano.ArmarXml(_datosReceiveExportBanano))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = false, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            var resultado = _servicio.EjecutarProcesosReceiveExportBanano(_datosReceiveExportBanano);
            Assert.IsFalse(resultado.FueOk);
            Assert.IsTrue(resultado.Mensaje.Contains("N4 RETORNO ERROR EN STAGE DONE"));
        }

        [TestMethod]
        public void TestEjecutarProcesosReceiveExportBananoOk()
        {
            _mock.Setup(m => m.Invocacion(EstadoTruckVisitReceiveExportBanano.ArmarXml(_datosReceiveExportBanano))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mock.Setup(m => m.Invocacion(EstadoGroovyReceiveExportBanano.ArmarXml(_datosReceiveExportBanano))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            _mock.Setup(m => m.Invocacion(EstadoStageDoneRecieveExportBanano.ArmarXml(_datosReceiveExportBanano))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            var resultado = _servicio.EjecutarProcesosReceiveExportBanano(_datosReceiveExportBanano);
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("Proceso Ok.", resultado.Mensaje);
        }


        [TestMethod]
        public void TestEjecutarProcesosDeliveryImportP2DOk()
        {
            _mock.Setup(m => m.Invocacion(EstadoTruckVisitImportP2D.ArmarXml(_datosP2D))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            foreach (var datosTransaccionP2D in _datosP2D.DatosTransaccionP2D)
            {
                _mock.Setup(m => m.Invocacion(EstadoGroovyImportP2D.ArmarXml(_datosP2D, datosTransaccionP2D))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            }
            
            
            _mock.Setup(m => m.Invocacion(EstadoStageDoneImportP2D.ArmarXml(_datosP2D))).Returns(new RespuestaServicioN4 { RecepcionXmlOk = true, IdEstado = 0, ResultadosConsultas = new List<QueryResultType> { new QueryResultType { Result = "" } } });
            var resultado = _servicio.EjecutarProcesosDeliveryImportP2D(_datosP2D);
            Assert.IsTrue(resultado.FueOk);
            Assert.AreEqual("Proceso Ok.", resultado.Mensaje);
        }
    }
}

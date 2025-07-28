using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RECEPTIO.CapaDominio.Console.Dominio.InterfacesRepositorios;
using RECEPTIO.CapaDominio.Console.ServiciosDominio;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Repositorio;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using System.Collections.Generic;

namespace RECEPTIO.CapaDominio.Tests.Console.ServicioDominio
{
    [TestClass]
    public class TestServicioLogin
    {
        private ServicioLogin _servicio;
        private Mock<IRepositorioTroubleDeskUser> _mockTroubleDeskUser;
        private Mock<IRepositorioDevice> _mockDevice;
        private Mock<IRepositorioUserSession> _mockUserSession;
        private Mock<IRepositorioTroubleTicket> _mockTroubleTicket;

        [TestInitialize]
        public void Inicializar()
        {
            _mockTroubleDeskUser = new Mock<IRepositorioTroubleDeskUser>();
            _mockDevice = new Mock<IRepositorioDevice>();
            _mockUserSession = new Mock<IRepositorioUserSession>();
            _mockTroubleTicket = new Mock<IRepositorioTroubleTicket>();
            _servicio = new ServicioLogin(_mockTroubleDeskUser.Object, _mockDevice.Object, _mockUserSession.Object, _mockTroubleTicket.Object);
        }

        [TestMethod]
        public void TestAutenticarCuandoUsuarioNoEstaRegistrado()
        {
            _mockTroubleDeskUser.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroUsuario>())).Returns(new List<TROUBLE_DESK_USER>());
            var resultado = _servicio.Autenticar("", "", "");
            Assert.IsFalse(resultado.EstaAutenticado);
            Assert.AreEqual("Usuario no registrado para usar la aplicación.", resultado.Mensaje);
        }

        [TestMethod]
        public void TestAutenticarCuandoUsuarioNoEstaInactivo()
        {
            _mockTroubleDeskUser.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroUsuario>())).Returns(new List<TROUBLE_DESK_USER> { new TROUBLE_DESK_USER()});
            var resultado = _servicio.Autenticar("", "", "");
            Assert.IsFalse(resultado.EstaAutenticado);
            Assert.AreEqual("Usuario esta inactivo.", resultado.Mensaje);
        }

        [TestMethod]
        public void TestAutenticarCuandoDispositivoNoEstaRegistrado()
        {
            _mockTroubleDeskUser.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroUsuario>())).Returns(new List<TROUBLE_DESK_USER> { new TROUBLE_DESK_USER { IS_ACTIVE = true} });
            _mockDevice.Setup(m => m.ObtenerDispositivoConZona(It.IsAny<FiltroDispositivoPorIp>())).Returns(new List<DEVICE>());
            var resultado = _servicio.Autenticar("", "", "");
            Assert.IsFalse(resultado.EstaAutenticado);
            Assert.AreEqual("El dispositivo no está registrado para que Console opere sobre él.", resultado.Mensaje);
        }

        [TestMethod]
        public void TestAutenticarCuandoDispositivoNoEstaInactivo()
        {
            _mockTroubleDeskUser.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroUsuario>())).Returns(new List<TROUBLE_DESK_USER> { new TROUBLE_DESK_USER { IS_ACTIVE = true } });
            _mockDevice.Setup(m => m.ObtenerDispositivoConZona(It.IsAny<FiltroDispositivoPorIp>())).Returns(new List<DEVICE> { new DEVICE()});
            var resultado = _servicio.Autenticar("", "", "");
            Assert.IsFalse(resultado.EstaAutenticado);
            Assert.AreEqual("El dispositivo esta inactivo para que Console opere sobre él.", resultado.Mensaje);
        }

        [TestMethod]
        public void TestAutenticarCuandoContrasenaEsInvalida()
        {
            _mockTroubleDeskUser.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroUsuario>())).Returns(new List<TROUBLE_DESK_USER> { new TROUBLE_DESK_USER { IS_ACTIVE = true } });
            _mockDevice.Setup(m => m.ObtenerDispositivoConZona(It.IsAny<FiltroDispositivoPorIp>())).Returns(new List<DEVICE> { new DEVICE { IS_ACTIVE = true} });
            var resultado = _servicio.Autenticar("calvarado", "", "");
            Assert.IsFalse(resultado.EstaAutenticado);
            Assert.AreEqual("Usuario y/o contraseña inválidas.", resultado.Mensaje);
        }

        [TestMethod]
        public void TestAutenticarCuandoNotienePermisos()
        {
            _mockTroubleDeskUser.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroUsuario>())).Returns(new List<TROUBLE_DESK_USER> { new TROUBLE_DESK_USER { IS_ACTIVE = true } });
            _mockDevice.Setup(m => m.ObtenerDispositivoConZona(It.IsAny<FiltroDispositivoPorIp>())).Returns(new List<DEVICE> { new DEVICE { IS_ACTIVE = true } });
            var resultado = _servicio.Autenticar("calvarado", "Pa$$w0rd25", "");
            Assert.IsFalse(resultado.EstaAutenticado);
            Assert.AreEqual("No tiene permisos para ingresar a la aplicación.", resultado.Mensaje);
        }

        [TestMethod]
        public void TestAutenticarOk()
        {
            _mockTroubleDeskUser.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroUsuario>())).Returns(new List<TROUBLE_DESK_USER> { new TROUBLE_DESK_USER { IS_ACTIVE = true } });
            _mockDevice.Setup(m => m.ObtenerDispositivoConZona(It.IsAny<FiltroDispositivoPorIp>())).Returns(new List<DEVICE> { new DEVICE { IS_ACTIVE = true , ZONE = new ZONE { NAME = ""} } });
            _mockUserSession.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroSesionUsuarioAbiertas>())).Returns(new List<USER_SESSION>());
            var resultado = _servicio.Autenticar("calvarado", "Pa$$w0rd25", "");
            Assert.IsTrue(resultado.EstaAutenticado);
            Assert.AreEqual("Usuario Auténticado y Autorizado.", resultado.Mensaje);
        }

        [TestMethod]
        public void TestCerrarSesionCuandoNoExisteId()
        {
            _mockUserSession.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroSesionUsuarioPorId>())).Returns(new List<USER_SESSION>());
            try
            {
                _servicio.CerrarSesion(0);
            }
            catch (System.Exception ex)
            {
                Assert.AreEqual("No existe sesion de usuario 0", ex.Message);
                return;
            }
            Assert.Fail("Test Falló.");
        }

        [TestMethod]
        public void TestCerrarSesionOk()
        {
            _mockUserSession.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroSesionUsuarioPorId>())).Returns(new List<USER_SESSION> { new USER_SESSION()});
            _mockUserSession.Setup(m => m.Actualizar(It.IsAny<USER_SESSION>()));
            _servicio.CerrarSesion(0);
            Assert.IsTrue(true);
        }
    }
}

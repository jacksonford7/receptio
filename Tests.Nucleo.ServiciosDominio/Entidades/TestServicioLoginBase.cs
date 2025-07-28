using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Repositorio;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using RECEPTIO.CapaDominio.Nucleo.ServiciosDominio.Otros;

namespace RECEPTIO.CapaDominio.Tests.Nucleo.ServiciosDominio.Entidades
{
    [TestClass]
    public class TestServicioLoginBase
    {
        private ServicioLoginBase _servicio;
        private Mock<IRepositorioTroubleDeskUser> _mockTroubleDeskUser;

        [TestInitialize]
        public void Inicializar()
        {
            _mockTroubleDeskUser = new Mock<IRepositorioTroubleDeskUser>();
            _servicio = new ServicioLoginBase(_mockTroubleDeskUser.Object);
        }

        [TestMethod]
        public void TestAutenticarCuandoUsuarioNoEstaRegistrado()
        {
            _mockTroubleDeskUser.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroUsuario>())).Returns(new List<TROUBLE_DESK_USER>());
            var resultado = _servicio.AutenticarAccion("", "");
            Assert.IsFalse(resultado.Item1);
            Assert.AreEqual("Usuario no registrado en RECEPTIO.", resultado.Item2);
        }

        [TestMethod]
        public void TestAutenticarCuandoUsuarioNoEstaInactivo()
        {
            _mockTroubleDeskUser.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroUsuario>())).Returns(new List<TROUBLE_DESK_USER> { new TROUBLE_DESK_USER() });
            var resultado = _servicio.AutenticarAccion("", "");
            Assert.IsFalse(resultado.Item1);
            Assert.AreEqual("Usuario esta inactivo.", resultado.Item2);
        }

        [TestMethod]
        public void TestAutenticarCuandoUsuarioNoEsSupervisor()
        {
            _mockTroubleDeskUser.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroUsuario>())).Returns(new List<TROUBLE_DESK_USER> { new TROUBLE_DESK_USER { IS_ACTIVE = true } });
            var resultado = _servicio.AutenticarAccion("", "");
            Assert.IsFalse(resultado.Item1);
            Assert.AreEqual("Usuario no es supervisor.", resultado.Item2);
        }

        [TestMethod]
        public void TestAutenticarCuandoContrasenaEsInvalida()
        {
            _mockTroubleDeskUser.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroUsuario>())).Returns(new List<TROUBLE_DESK_USER> { new TROUBLE_DESK_USER { IS_ACTIVE = true, IS_SUPERVISOR = true } });
            var resultado = _servicio.AutenticarAccion("calvarado", "");
            Assert.IsFalse(resultado.Item1);
            Assert.AreEqual("Usuario y/o contraseña inválidas.", resultado.Item2);
        }

        [TestMethod]
        public void TestAutenticarCuandoNotienePermisos()
        {
            _mockTroubleDeskUser.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroUsuario>())).Returns(new List<TROUBLE_DESK_USER> { new TROUBLE_DESK_USER { IS_ACTIVE = true, IS_SUPERVISOR = true } });
            var resultado = _servicio.AutenticarAccion("calvarado", "Pa$$w0rd25");
            Assert.IsFalse(resultado.Item1);
            Assert.AreEqual("No tiene permisos para usar la opción.", resultado.Item2);
        }

        [TestMethod]
        public void TestAutenticarOk()
        {
            _mockTroubleDeskUser.Setup(m => m.ObtenerObjetos(It.IsAny<FiltroUsuario>())).Returns(new List<TROUBLE_DESK_USER> { new TROUBLE_DESK_USER { IS_ACTIVE = true, IS_SUPERVISOR = true } });
            var resultado = _servicio.AutenticarAccion("calvarado", "Pa$$w0rd25");
            Assert.IsTrue(resultado.Item1);
            Assert.AreEqual("Usuario Auténticado y Autorizado.", resultado.Item2);
        }
    }
}

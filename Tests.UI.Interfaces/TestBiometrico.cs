using Microsoft.VisualStudio.TestTools.UnitTesting;
using RECEPTIO.CapaPresentacion.UI.Biometrico;

namespace RECEPTIO.CapaPresentacion.Tests.UI.Interfaces
{
    [TestClass]
    public class TestBiometrico
    {
        private Biometrico _servicio;

        [TestInitialize]
        public void Inicializar()
        {
            _servicio = new Biometrico();
        }

        [TestMethod]
        public void TestProcesoHuellaCuandoNoSeColocaLaHuella()
        {
            const string cedula = "0916298235";
            var resultado = _servicio.ProcesoHuella(cedula);
            Assert.IsTrue(resultado.Contains("ERROR CON EL DISPOSITIVO"));
        }

        [TestMethod]
        public void TestProcesoHuellaCuandoNoConcuerda()
        {
            const string cedula = "0916298235";
            var resultado = _servicio.ProcesoHuella(cedula);
            Assert.IsTrue(resultado.Contains("HUELLA NO CONCUERDA"));
        }

        [TestMethod]
        public void TestProcesoHuellaOk()
        {
            const string cedula = "0916298235";
            var resultado = _servicio.ProcesoHuella(cedula);
            Assert.IsTrue(resultado.Contains("ALVARADO SANCHEZ:CARLOS STALIN"));
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RECEPTIO.CapaInfraestructura.Transaction.SqlEntityFramework.Repositorios;

namespace RECEPTIO.CapaInfraestructura.Tests.Transaction.SqlEntityFramework
{
    [TestClass]
    public class TestRepositorioValidacionesN4
    {
        private RepositorioValidacionesN4 _repositorio;

        [TestInitialize]
        public void Inicializar()
        {
            _repositorio = new RepositorioValidacionesN4();
        }

        [TestMethod]
        public void TestEstaEnPatioContenedorNoOk()
        {
            const string numeroContenedor = "KOSU220166";
            var resultado = _repositorio.EstaEnPatioContenedor(numeroContenedor);
            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void TestEstaEnPatioContenedorOk()
        {
            const string numeroContenedor = "KOSU2201661";
            var resultado = _repositorio.EstaEnPatioContenedor(numeroContenedor);
            Assert.IsTrue(resultado);
        }
    }
}

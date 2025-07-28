using Microsoft.VisualStudio.TestTools.UnitTesting;
using RECEPTIO.CapaInfraestructura.Transaction.SqlEntityFramework.Repositorios;

namespace RECEPTIO.CapaInfraestructura.Tests.Transaction.SqlEntityFramework
{
    [TestClass]
    public class TestRepositorioTag
    {
        private RepositorioTag _repositorio;

        [TestInitialize]
        public void Inicializar()
        {
            _repositorio = new RepositorioTag();
        }

        [TestMethod]
        public void TestObtenerTag()
        {
            const string placa = "sss";
            var resultado = _repositorio.ObtenerTag(placa);
            Assert.AreEqual("13327413", resultado);
        }
    }
}

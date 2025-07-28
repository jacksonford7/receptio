using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Filtros;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using RECEPTIO.CapaInfraestructura.Nucleo.Infraestructura;

namespace RECEPTIO.CapaInfraestructura.Tests.Nucleo.Infraestructura
{
    [TestClass]
    public class TestRepositorioPreGate : IDisposable
    {
        private RepositorioPreGate _repositorio;
        private bool _disposed;

        [TestInitialize]
        public void Inicializar()
        {
            _repositorio = new RepositorioPreGate();
        }

        [TestMethod]
        public void TestObtenerPreGates()
        {
            var filtro = new FiltroPruebaPreGate();
            var items = _repositorio.ObtenerPreGateConDetalle(filtro);
            Assert.IsTrue(items.Count() > 0);
        }

        [TestMethod]
        public void TestObtenerPreGatesSupervisor()
        {
            var filtro = new FiltroPruebaPreGate();
            var items = _repositorio.ObtenerPreGateConDetalleParaSupervisor(filtro);
            Assert.IsTrue(items.Count() > 0);
        }

        [TestMethod]
        public void TestActualizarPreGate()
        {
            const int id = 1;
            const string estado = "K";
            var filtro = new FiltroPruebaPreGateActualizar(id);
            var item = _repositorio.ObtenerObjetos(filtro).FirstOrDefault();
            if (item == null)
                Assert.Fail("Test falló porque no existe ningún item a actualizar.");
            item.STATUS = estado;
            _repositorio.Actualizar(item);
            item = _repositorio.ObtenerObjetos(filtro).FirstOrDefault();
            if (item == null)
                Assert.Fail("Test falló porque no existe ningún item a actualizar.");
            Assert.AreEqual(estado, item.STATUS);
        }

        [TestMethod]
        public void ObtenerSecuenciaIdPreGate()
        {
            var id = _repositorio.ObtenerSecuenciaIdPreGate();
            Assert.IsTrue(id > 260);
        }

        [TestMethod]
        public void ObtenerValidacionesGenerales()
        {
            var id = _repositorio.ObtenerValidacionesGenerales("",string.Empty ,0, 319970);
            Assert.IsTrue(id =="");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
                _repositorio.Dispose();
            _disposed = true;
        }
    }

    internal class FiltroPruebaPreGate : Filtros<PRE_GATE>
    {
        public override Expression<Func<PRE_GATE, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<PRE_GATE>(p => p.PRE_GATE_ID > 0);
            return filtro.SastifechoPor();
        }
    }

    internal class FiltroPruebaPreGateActualizar : Filtros<PRE_GATE>
    {
        private readonly long _id;

        internal FiltroPruebaPreGateActualizar(long id)
        {
            _id = id;
        }

        public override Expression<Func<PRE_GATE, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<PRE_GATE>(d => d.PRE_GATE_ID == _id);
            return filtro.SastifechoPor();
        }
    }
}

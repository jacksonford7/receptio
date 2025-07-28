using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Filtros;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using RECEPTIO.CapaInfraestructura.Transaction.SqlEntityFramework.Repositorios;

namespace RECEPTIO.CapaInfraestructura.Tests.Transaction.SqlEntityFramework
{
    [TestClass]
    public class TestRepositorioDetallePreGate : IDisposable
    {
        private RepositorioDetallePreGate _repositorio;
        private bool _disposed;

        [TestInitialize]
        public void Inicializar()
        {
            _repositorio = new RepositorioDetallePreGate();
        }

        [TestMethod]
        public void TestObtenerDetallePreGate()
        {
            var filtro = new FiltroPruebaDetallePreGate();
            var item = _repositorio.ObtenerObjetos(filtro);
            Assert.IsNotNull(item);
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

    internal class FiltroPruebaDetallePreGate : Filtros<PRE_GATE_DETAIL>
    {
        public override Expression<Func<PRE_GATE_DETAIL, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<PRE_GATE_DETAIL>(d => d.ID > 0);
            return filtro.SastifechoPor();
        }
    }
}

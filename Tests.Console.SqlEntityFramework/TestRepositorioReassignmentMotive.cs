using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Filtros;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using RECEPTIO.CapaInfraestructura.Console.SqlEntityFramework.Repositorios;

namespace RECEPTIO.CapaInfraestructura.Tests.Console.SqlEntityFramework
{
    [TestClass]
    public class TestRepositorioReassignmentMotive : IDisposable
    {
        private RepositorioReassignmentMotive _repositorio;
        private bool _disposed;

        [TestInitialize]
        public void Inicializar()
        {
            _repositorio = new RepositorioReassignmentMotive();
        }

        [TestMethod]
        public void TestObtenerMotivosReasignacion()
        {
            const int numeroMinimoItems = 1;
            var filtro = new FiltroPruebaReassignmentMotive();
            var items = _repositorio.ObtenerObjetos(filtro);
            Assert.IsTrue(items.Count() >= numeroMinimoItems);
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

    internal class FiltroPruebaReassignmentMotive : Filtros<REASSIGNMENT_MOTIVE>
    {
        public override Expression<Func<REASSIGNMENT_MOTIVE, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<REASSIGNMENT_MOTIVE>(rm => rm.ID > 0);
            return filtro.SastifechoPor();
        }
    }
}

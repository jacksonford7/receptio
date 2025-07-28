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
    public class TestRepositorioTroubleDeskUser : IDisposable
    {
        private RepositorioTroubleDeskUser _repositorio;
        private bool _disposed;

        [TestInitialize]
        public void Inicializar()
        {
            _repositorio = new RepositorioTroubleDeskUser();
        }

        [TestMethod]
        public void TestObtenerUsuarios()
        {
            const int numeroMinimoItems = 1;
            var filtro = new FiltroPruebaUsuario();
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

    internal class FiltroPruebaUsuario : Filtros<TROUBLE_DESK_USER>
    {
        public override Expression<Func<TROUBLE_DESK_USER, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<TROUBLE_DESK_USER>(tdu => tdu.TTU_ID > 0);
            return filtro.SastifechoPor();
        }
    }
}

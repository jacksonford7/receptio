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
    public class TestRepositorioUserSession : IDisposable
    {
        private RepositorioUserSession _repositorio;
        private bool _disposed;

        [TestInitialize]
        public void Inicializar()
        {
            _repositorio = new RepositorioUserSession();
        }

        [TestMethod]
        public void TestObtenerSesionUsuarioConTickets()
        {
            const int numeroMinimoItems = 1;
            var filtro = new FiltroPruebaSesionUsuario();
            var items = _repositorio.ObtenerSesionUsuarioConTickets(filtro);
            Assert.IsTrue(items.Count() >= numeroMinimoItems);
        }

        [TestMethod]
        public void TestObtenerSesionUsuarioConDispositivo()
        {
            const int numeroMinimoItems = 1;
            var filtro = new FiltroPruebaSesionUsuario();
            var items = _repositorio.ObtenerSesionUsuarioConDispositivo(filtro);
            Assert.IsTrue(items.Count() >= numeroMinimoItems);
        }

        [TestMethod]
        public void TestObtenerSesionUsuarioConDispositivoZonaUsuario()
        {
            const int numeroMinimoItems = 1;
            var filtro = new FiltroPruebaSesionUsuario();
            var items = _repositorio.ObtenerSesionUsuarioConDispositivoZonaUsuario(filtro);
            Assert.IsTrue(items.Count() >= numeroMinimoItems);
        }

        [TestMethod]
        public void TestCrearSesion()
        {
            var item = new USER_SESSION
            {
                START_SESSION__DATE = DateTime.Now,
                DEVICE_ID = 2,
                TTU_ID = 1
            };
            try
            {
                _repositorio.Agregar(item);
                Assert.IsTrue(true);
            }
            catch (Exception ex)
            {
                Assert.Fail($"{ex.Message} {ex.InnerException} {ex.StackTrace}");
            }
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

    internal class FiltroPruebaSesionUsuario : Filtros<USER_SESSION>
    {
        public override Expression<Func<USER_SESSION, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<USER_SESSION>(us => us.ID > 0);
            return filtro.SastifechoPor();
        }
    }
}

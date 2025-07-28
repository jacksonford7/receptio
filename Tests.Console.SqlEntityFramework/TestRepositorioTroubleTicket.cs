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
    public class TestRepositorioTroubleTicket : IDisposable
    {
        private RepositorioTroubleTicket _repositorio;
        private bool _disposed;

        [TestInitialize]
        public void Inicializar()
        {
            _repositorio = new RepositorioTroubleTicket();
        }

        [TestMethod]
        public void TestObtenerTicketProcesos()
        {
            const int numeroMinimoItems = 1;
            var filtro = new FiltroPruebaTicketProcesos();
            var items = _repositorio.ObtenerTicketProcesos(filtro);
            Assert.IsTrue(items.Count() >= numeroMinimoItems);
        }

        [TestMethod]
        public void TestObtenerAutoTicket()
        {
            const int numeroMinimoItems = 0;
            var filtro = new FiltroPruebaAutoTicket();
            var items = _repositorio.ObtenerAutoTicket(filtro);
            Assert.IsTrue(items.Count() >= numeroMinimoItems);
        }

        [TestMethod]
        public void TestObtenerTicketMobile()
        {
            const int numeroMinimoItems = 0;
            var filtro = new FiltroPruebaTicketMobile();
            var items = _repositorio.ObtenerTicketMobile(filtro);
            Assert.IsTrue(items.Count() >= numeroMinimoItems);
        }

        [TestMethod]
        public void TestObtenerTicketTecnico()
        {
            const int numeroMinimoItems = 1;
            var filtro = new FiltroPruebaTicketTecnico();
            var items = _repositorio.ObtenerTicketTecnico(filtro);
            Assert.IsTrue(items.Count() >= numeroMinimoItems);
        }

        [TestMethod]
        public void TestCrearTicketProceso()
        {
            try
            {
                var item = new PROCESS_TROUBLE_TICKET
                {
                    CREATION_DATE = DateTime.Now,
                    IS_CANCEL = true
                };
                _repositorio.CrearTicketProceso(item, 1);
                Assert.IsTrue(true);
            }
            catch (Exception ex)
            {
                Assert.Fail($"{ex.Message} {ex.InnerException} {ex.StackTrace}");
            }
        }

        [TestMethod]
        public void TestCrearTicketAppCliente()
        {
            try
            {
                var item = new CLIENT_APP_TRANSACTION_TROUBLE_TICKET
                {
                    CREATION_DATE = DateTime.Now,
                    IS_CANCEL = true,
                    ZONE_ID = 1
                };
                _repositorio.CrearTicketAppCliente(item, 4);
                Assert.IsTrue(true);
            }
            catch (Exception ex)
            {
                Assert.Fail($"{ex.Message} {ex.InnerException} {ex.StackTrace}");
            }
        }

        [TestMethod]
        public void TestAgregarSesionUsuarioATicketProceso()
        {
            try
            {
                _repositorio.AgregarSesionUsuarioATicketProceso(6, 1);
                Assert.IsTrue(true);
            }
            catch (Exception ex)
            {
                Assert.Fail($"{ex.Message} {ex.InnerException} {ex.StackTrace}");
            }
        }

        [TestMethod]
        public void TestAgregarSesionUsuarioATicketTecnico()
        {
            try
            {
                _repositorio.AgregarSesionUsuarioATicketTecnico(7, 1);
                Assert.IsTrue(true);
            }
            catch (Exception ex)
            {
                Assert.Fail($"{ex.Message} {ex.InnerException} {ex.StackTrace}");
            }
        }

        [TestMethod]
        public void TestReasignarTicketProceso()
        {
            try
            {
                _repositorio.ReasignarTicketProceso(20056, 20191, new REASSIGNMENT { DATE = DateTime.Now, MOTIVE_ID = 1, USER = "manga"});
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

    internal class FiltroPruebaTicketProcesos : Filtros<PROCESS_TROUBLE_TICKET>
    {
        public override Expression<Func<PROCESS_TROUBLE_TICKET, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<PROCESS_TROUBLE_TICKET>(ptt => ptt.TT_ID > 0);
            return filtro.SastifechoPor();
        }
    }

    internal class FiltroPruebaAutoTicket : Filtros<AUTO_TROUBLE_TICKET>
    {
        public override Expression<Func<AUTO_TROUBLE_TICKET, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<AUTO_TROUBLE_TICKET>(att => att.TT_ID > 0);
            return filtro.SastifechoPor();
        }
    }

    internal class FiltroPruebaTicketMobile : Filtros<MOBILE_TROUBLE_TICKET>
    {
        public override Expression<Func<MOBILE_TROUBLE_TICKET, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<MOBILE_TROUBLE_TICKET>(mtt => mtt.TT_ID > 0);
            return filtro.SastifechoPor();
        }
    }

    internal class FiltroPruebaTicketTecnico : Filtros<CLIENT_APP_TRANSACTION_TROUBLE_TICKET>
    {
        public override Expression<Func<CLIENT_APP_TRANSACTION_TROUBLE_TICKET, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<CLIENT_APP_TRANSACTION_TROUBLE_TICKET>(ttt => ttt.TT_ID > 0);
            return filtro.SastifechoPor();
        }
    }
}

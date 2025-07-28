using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using RECEPTIO.CapaAplicacion.Console.Aplicacion.Contratos;
using RECEPTIO.CapaAplicacion.Console.Aplicacion.Interfaces;
using RECEPTIO.CapaDominio.Console.Dominio.InterfacesRepositorios;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Filtros;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Repositorio;
using RECEPTIO.CapaDominio.Nucleo.Entidades;

namespace RECEPTIO.CapaDominio.Console.ServiciosDominio
{
    public class ServicioLogin : ILogin
    {
        internal readonly IRepositorioTroubleDeskUser RepositorioTroubleDeskUser;
        internal readonly IRepositorioDevice RepositorioDevice;
        internal readonly IRepositorioUserSession RepositorioUserSesion;
        internal readonly IRepositorioTroubleTicket RepositorioTroubleTicket;
        private EstadoLogin _estado;
        internal string Usuario;
        internal string Contrasena;
        internal string Ip;
        internal int IdUsuario;
        internal bool EsLider;
        internal DEVICE Dispositivo;
        internal List<long> IdTicketsTecnicos;
        internal List<long> IdTicketsMobile;
        internal List<long> IdTicketsProcesos;
        internal List<long> IdAutoTickets;

        public ServicioLogin(IRepositorioTroubleDeskUser repositorioTroubleDeskUser, IRepositorioDevice repositorioDevice, IRepositorioUserSession repositorioUserSesion, IRepositorioTroubleTicket repositorioTroubleTicket)
        {
            RepositorioTroubleDeskUser = repositorioTroubleDeskUser;
            RepositorioDevice = repositorioDevice;
            RepositorioUserSesion = repositorioUserSesion;
            RepositorioTroubleTicket = repositorioTroubleTicket;
        }

        public DatosLogin Autenticar(string usuario, string contrasena, string ip)
        {
            Usuario = usuario;
            Contrasena = contrasena;
            Ip = ip;
            using (var transaction = new TransactionScope())
            {
                _estado = new EstadoUsuario();
                var resultado = _estado.Ejecutar(this);
                transaction.Complete();
                return resultado;
            }
        }

        internal DatosLogin SetearEstado(EstadoLogin estado)
        {
            _estado = estado;
            return _estado.Ejecutar(this);
        }

        public void CerrarSesion(long idSesion)
        {
            var sesion = RepositorioUserSesion.ObtenerObjetos(new FiltroSesionUsuarioPorId(idSesion)).FirstOrDefault();
            if (sesion == null)
                throw new ApplicationException($"No existe sesion de usuario {idSesion}");
            sesion.FINISH_SESSION_DATE = DateTime.Now;
            RepositorioUserSesion.Actualizar(sesion);
        }

        public void LiberarRecursos()
        {
            RepositorioTroubleDeskUser.LiberarRecursos();
            RepositorioDevice.LiberarRecursos();
            RepositorioUserSesion.LiberarRecursos();
            RepositorioTroubleTicket.LiberarRecursos();
        }
    }

    public class FiltroSesionUsuarioPorId : IFiltros<USER_SESSION>
    {
        private readonly long _idSesion;

        public FiltroSesionUsuarioPorId(long idSesion)
        {
            _idSesion = idSesion;
        }

        public Expression<Func<USER_SESSION, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<USER_SESSION>(us => us.ID == _idSesion);
            return filtro.SastifechoPor();
        }
    }

    internal abstract class EstadoLogin
    {
        internal abstract DatosLogin Ejecutar(ServicioLogin servicio);
    }

    internal class EstadoUsuario : EstadoLogin
    {
        internal override DatosLogin Ejecutar(ServicioLogin servicio)
        {
            var usuario = servicio.RepositorioTroubleDeskUser.ObtenerObjetos(new FiltroUsuario(servicio.Usuario)).FirstOrDefault();
            if (usuario == null)
                return new DatosLogin { Mensaje = "Usuario no registrado para usar la aplicación." };
            else if (!usuario.IS_ACTIVE)
                return new DatosLogin { Mensaje = "Usuario esta inactivo." };
            else
            {
                servicio.IdUsuario = usuario.TTU_ID;
                servicio.EsLider = usuario.IS_SUPERVISOR;
                return servicio.SetearEstado(new EstadoDispositivo());
            }
        }
    }

    public class FiltroUsuario : IFiltros<TROUBLE_DESK_USER>
    {
        private readonly string _usuario;

        public FiltroUsuario(string usuario)
        {
            _usuario = usuario;
        }

        public Expression<Func<TROUBLE_DESK_USER, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<TROUBLE_DESK_USER>(tdu => tdu.USER_NAME == _usuario);
            return filtro.SastifechoPor();
        }
    }

    internal class EstadoDispositivo : EstadoLogin
    {
        internal override DatosLogin Ejecutar(ServicioLogin servicio)
        {
            var dispositivo = servicio.RepositorioDevice.ObtenerDispositivoConZona(new FiltroDispositivoPorIp(servicio.Ip)).FirstOrDefault();
            if (dispositivo == null)
                return new DatosLogin { Mensaje = "El dispositivo no está registrado para que Console opere sobre él." };
            else if (!dispositivo.IS_ACTIVE)
                return new DatosLogin { Mensaje = "El dispositivo esta inactivo para que Console opere sobre él." };
            else
            {
                servicio.Dispositivo = dispositivo;
                return servicio.SetearEstado(new EstadoActiveDirectory());
            }
        }
    }

    public class FiltroDispositivoPorIp : IFiltros<DEVICE>
    {
        private readonly string _ip;

        public FiltroDispositivoPorIp(string ip)
        {
            _ip = ip;
        }

        public Expression<Func<DEVICE, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<DEVICE>(d => d.IP == _ip);
            return filtro.SastifechoPor();
        }
    }

    internal class EstadoActiveDirectory : EstadoLogin
    {
        internal override DatosLogin Ejecutar(ServicioLogin servicio)
        {
            var resultadoBusquedaActiveDirectory = BusquedaActiveDirectory(servicio.Usuario, servicio.Contrasena);

            if (resultadoBusquedaActiveDirectory == "1")
                return new DatosLogin
                {
                    Mensaje = "Usuario y/o contraseña inválidas."
                };
            else if (resultadoBusquedaActiveDirectory == "2")
                return new DatosLogin
                {
                    Mensaje = "No tiene permisos para ingresar a la aplicación."
                };
            else
                return servicio.SetearEstado(new EstadoCerrarSesionesAbiertas());
        }

        private string BusquedaActiveDirectory(string usuario, string contrasena)
        {
            var servicioActiveDirectory = new Barreras.Librerias.AutenticacionActiveDirectory();
            return servicioActiveDirectory.Login(usuario, contrasena);
        }
    }

    internal class EstadoCerrarSesionesAbiertas : EstadoLogin
    {
        internal override DatosLogin Ejecutar(ServicioLogin servicio)
        {
            var sesiones = servicio.RepositorioUserSesion.ObtenerSesionUsuarioConTickets(new FiltroSesionesAbiertasPorIdUsuario(servicio.IdUsuario));
            foreach (var sesion in sesiones)
            {
                GuardarTicketsPendientes(servicio, sesion);
                sesion.FINISH_SESSION_DATE = DateTime.Now;
                servicio.RepositorioUserSesion.Actualizar(sesion);
            }
            return servicio.SetearEstado(new EstadoGrabarSesionUsuario());
        }

        private void GuardarTicketsPendientes(ServicioLogin servicio, USER_SESSION sesion)
        {
            servicio.IdAutoTickets = sesion.AUTO_TROUBLE_TICKETS.Count(att => !att.IS_CANCEL && !att.FINISH_DATE.HasValue) == 0 ? new List<long>() : sesion.AUTO_TROUBLE_TICKETS.Where(att => !att.IS_CANCEL && !att.FINISH_DATE.HasValue).Select(att => att.TT_ID).ToList();
            servicio.IdTicketsMobile = sesion.MOBILE_TROUBLE_TICKETS.Count(att => !att.IS_CANCEL && !att.FINISH_DATE.HasValue) == 0 ? new List<long>() : sesion.MOBILE_TROUBLE_TICKETS.Where(att => !att.IS_CANCEL && !att.FINISH_DATE.HasValue).Select(att => att.TT_ID).ToList();
            servicio.IdTicketsProcesos = sesion.PROCESS_TROUBLE_TICKETS.Count(att => !att.IS_CANCEL && !att.FINISH_DATE.HasValue) == 0 ? new List<long>() : sesion.PROCESS_TROUBLE_TICKETS.Where(att => !att.IS_CANCEL && !att.FINISH_DATE.HasValue).Select(att => att.TT_ID).ToList();
            servicio.IdTicketsTecnicos = sesion.CLIENT_APP_TRANSACTION_TROUBLE_TICKETS.Count(att => !att.IS_CANCEL && !att.FINISH_DATE.HasValue) == 0 ? new List<long>() : sesion.CLIENT_APP_TRANSACTION_TROUBLE_TICKETS.Where(att => !att.IS_CANCEL && !att.FINISH_DATE.HasValue).Select(att => att.TT_ID).ToList();
        }
    }

    internal class FiltroSesionesAbiertasPorIdUsuario : IFiltros<USER_SESSION>
    {
        private readonly int _idUsuario;

        public FiltroSesionesAbiertasPorIdUsuario(int idUsuario)
        {
            _idUsuario = idUsuario;
        }

        public Expression<Func<USER_SESSION, bool>> SastifechoPor()
        {
            var filtro = new FiltroDirecto<USER_SESSION>(us => us.TTU_ID == _idUsuario && !us.FINISH_SESSION_DATE.HasValue);
            return filtro.SastifechoPor();
        }
    }

    internal class EstadoGrabarSesionUsuario : EstadoLogin
    {
        internal override DatosLogin Ejecutar(ServicioLogin servicio)
        {
            var sesion = new USER_SESSION
            {
                START_SESSION__DATE = DateTime.Now,
                DEVICE_ID = servicio.Dispositivo.DEVICE_ID,
                TTU_ID = servicio.IdUsuario
            };
            servicio.RepositorioUserSesion.Agregar(sesion);
            if(servicio.IdAutoTickets != null || servicio.IdTicketsMobile != null || servicio.IdTicketsProcesos != null || servicio.IdTicketsTecnicos != null)
                AgregarTicketsPendientes(servicio, sesion.ID);
            return new DatosLogin
            {
                Zona = servicio.Dispositivo.ZONE.NAME,
                EsLider = servicio.EsLider,
                EstaAutenticado = true,
                IdSesion = sesion.ID,
                IdUsuario = servicio.IdUsuario,
                Mensaje = "Usuario Auténticado y Autorizado."
            };
        }

        private void AgregarTicketsPendientes(ServicioLogin servicio, long id)
        {
            AutoTickets(servicio, id);
            TicketsTecnicos(servicio, id);
            TicketsProceso(servicio, id);
            TicketsMobile(servicio, id);
        }

        private void AutoTickets(ServicioLogin servicio, long id)
        {
            foreach (var idTicket in servicio.IdAutoTickets)
                servicio.RepositorioTroubleTicket.AgregarSesionUsuarioAAutoTicket(idTicket, id);
        }

        private void TicketsTecnicos(ServicioLogin servicio, long id)
        {
            foreach (var idTicket in servicio.IdTicketsTecnicos)
                servicio.RepositorioTroubleTicket.AgregarSesionUsuarioATicketTecnico(idTicket, id);
        }

        private void TicketsProceso(ServicioLogin servicio, long id)
        {
            foreach (var idTicket in servicio.IdTicketsProcesos)
                servicio.RepositorioTroubleTicket.AgregarSesionUsuarioATicketProceso(idTicket, id);
        }

        private void TicketsMobile(ServicioLogin servicio, long id)
        {
            foreach (var idTicket in servicio.IdTicketsMobile)
                servicio.RepositorioTroubleTicket.AgregarSesionUsuarioATicketMobile(idTicket, id);
        }
    }
}

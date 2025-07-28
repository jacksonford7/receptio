using RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Interfaces;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Filtros;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Repositorio;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace RECEPTIO.CapaDominio.Nucleo.ServiciosDominio.Otros
{
    public class ServicioLoginBase : ILoginBase
    {
        internal readonly IRepositorioTroubleDeskUser RepositorioTroubleDeskUser;
        private EstadoAutenticarSupervisor _estado;
        internal string Usuario;
        internal string Contrasena;

        public ServicioLoginBase(IRepositorioTroubleDeskUser repositorioTroubleDeskUser)
        {
            RepositorioTroubleDeskUser = repositorioTroubleDeskUser;
        }

        public Tuple<bool, string> AutenticarAccion(string usuario, string contrasena)
        {
            Usuario = usuario;
            Contrasena = contrasena;
            _estado = new EstadoUsuario();
            return _estado.Ejecutar(this);
        }

        internal Tuple<bool, string> SetearEstado(EstadoAutenticarSupervisor estado)
        {
            _estado = estado;
            return _estado.Ejecutar(this);
        }

        public void LiberarRecursos()
        {
            RepositorioTroubleDeskUser.LiberarRecursos();
        }
    }

    internal abstract class EstadoAutenticarSupervisor
    {
        internal abstract Tuple<bool, string> Ejecutar(ServicioLoginBase servicio);
    }

    internal class EstadoUsuario : EstadoAutenticarSupervisor
    {
        internal override Tuple<bool, string> Ejecutar(ServicioLoginBase servicio)
        {
            var usuario = servicio.RepositorioTroubleDeskUser.ObtenerObjetos(new FiltroUsuario(servicio.Usuario)).FirstOrDefault();
            if (usuario == null)
                return new Tuple<bool, string>(false, "Usuario no registrado en RECEPTIO.");
            else if (!usuario.IS_ACTIVE)
                return new Tuple<bool, string>(false, "Usuario esta inactivo.");
            else if (!usuario.IS_SUPERVISOR)
                return new Tuple<bool, string>(false, "Usuario no es supervisor.");
            else
                return servicio.SetearEstado(new EstadoActiveDirectory());
        }
    }

    internal class EstadoActiveDirectory : EstadoAutenticarSupervisor
    {
        internal override Tuple<bool, string> Ejecutar(ServicioLoginBase servicio)
        {
            var resultadoBusquedaActiveDirectory = BusquedaActiveDirectory(servicio.Usuario, servicio.Contrasena);

            if (resultadoBusquedaActiveDirectory == "1")
                return new Tuple<bool, string>(false, "Usuario y/o contraseña inválidas.");
            else if (resultadoBusquedaActiveDirectory == "2")
                return new Tuple<bool, string>(false, "No tiene permisos para usar la opción.");
            else
                return new Tuple<bool, string>(true, "Usuario Auténticado y Autorizado.");
        }

        private string BusquedaActiveDirectory(string usuario, string contrasena)
        {
            var servicioActiveDirectory = new Barreras.Librerias.AutenticacionActiveDirectory();
            return servicioActiveDirectory.Login(usuario, contrasena);
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
}

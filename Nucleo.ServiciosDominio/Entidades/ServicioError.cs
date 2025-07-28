using RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Interfaces;
using RECEPTIO.CapaDominio.Nucleo.Dominio.Repositorio;
using System;

namespace RECEPTIO.CapaDominio.Nucleo.ServiciosDominio.Entidades
{
    public class ServicioError : IError
    {
        private readonly IRepositorioError _repositorio;

        public ServicioError(IRepositorioError repositorio)
        {
            _repositorio = repositorio;
        }

        public int CrearError(Nucleo.Entidades.ERROR error)
        {
            error.THROW_ON = DateTime.Now;
            error.TYPE_ERROR_ID = 2;
            _repositorio.Agregar(error);
            return error.ERROR_ID;
        }

        public int GrabarErrorTecnico(Nucleo.Entidades.ERROR error)
        {
            error.THROW_ON = DateTime.Now;
            error.TYPE_ERROR_ID = 1;
            _repositorio.Agregar(error);
            return error.ERROR_ID;
        }

        public void LiberarRecursos()
        {
            _repositorio.LiberarRecursos();
        }
    }
}

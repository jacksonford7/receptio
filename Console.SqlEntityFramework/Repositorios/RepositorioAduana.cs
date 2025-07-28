using RECEPTIO.CapaDominio.Console.Dominio.InterfacesRepositorios;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using RECEPTIO.CapaInfraestructura.Nucleo.Infraestructura;
using System.Collections.Generic;
using System.Linq;

namespace RECEPTIO.CapaInfraestructura.Console.SqlEntityFramework.Repositorios
{
    public class RepositorioAduana : Repositorio<RepositorioAduana>, IRepositorioAduana
    {
        public IEnumerable<mb_get_ecuapass_message_pass_Result> ObtenerMensajesSmdtAduana(string numeroTransaccion)
        {
            return Contexto.mb_get_ecuapass_message_pass(numeroTransaccion);
        }

        public mb_add_ecuapass_transaccion_Result AgregarTransaccionManual(long gKeyUnidad, string tipoCarga, string objetoSolicita, string usuarioSolicita, string contenedor, string mrn, string msn, string hsn, string numeroEntrega, string comentarios)
        {
            return Contexto.mb_add_ecuapass_transaccion(gKeyUnidad, tipoCarga, objetoSolicita, usuarioSolicita, contenedor, mrn, msn, hsn, numeroEntrega, comentarios).FirstOrDefault();
        }

        public byte? CambiarEstadoSmdt(string numeroTransaccion, string userName)
        {
            return Contexto.mb_set_estate_smdt_transaccion(numeroTransaccion, userName, "").FirstOrDefault();
        }
    }
}

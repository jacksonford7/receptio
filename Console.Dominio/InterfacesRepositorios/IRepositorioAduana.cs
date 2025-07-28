using RECEPTIO.CapaDominio.Nucleo.Entidades;
using System.Collections.Generic;

namespace RECEPTIO.CapaDominio.Console.Dominio.InterfacesRepositorios
{
    public interface IRepositorioAduana
    {
        IEnumerable<mb_get_ecuapass_message_pass_Result> ObtenerMensajesSmdtAduana(string numeroTransaccion);
        byte? CambiarEstadoSmdt(string numeroTransaccion, string userName);
        mb_add_ecuapass_transaccion_Result AgregarTransaccionManual(long gKeyUnidad, string tipoCarga, string objetoSolicita, string usuarioSolicita, string contenedor, string mrn, string msn, string hsn, string numeroEntrega, string comentarios);
        void LiberarRecursos();
    }
}

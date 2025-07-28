using RECEPTIO.CapaAplicacion.Console.Aplicacion.Contratos;
using RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Interfaces;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using System.Collections.Generic;
using System.ServiceModel;

namespace RECEPTIO.CapaAplicacion.Console.Aplicacion.Interfaces
{
    [ServiceContract]
    public interface ITroubleDesk : IBase
    {
        [OperationContract]
        IEnumerable<Ticket> ObtenerTickets(long idSesionUsuario);

        [OperationContract]
        DetalleTicket ObtenerDetallesTicket(int idTransaccionQuiosco);

        [OperationContract]
        void AceptarTicket(long idTicket);

        [OperationContract]
        void CerrarTicket(long idTicket, string notas, int motivo, int submotivo);

        [OperationContract]
        IEnumerable<AUTO_TROUBLE_REASON> ObtenerMotivosAutoTickets();

        [OperationContract]
        void CrearAutoTicket(int idMotivo, long idSesionUsuario);

        [OperationContract]
        void RegistrarAccion(ACTION accion);

        [OperationContract]
        IEnumerable<mb_get_ecuapass_message_pass_Result> ObtenerMensajesSmdtAduana(string numeroTransaccion);

        [OperationContract]
        byte? CambiarEstadoSmdt(string numeroTransaccion, string userName);

        [OperationContract]
        mb_add_ecuapass_transaccion_Result AgregarTransaccionManual(DatosTransaccionManual datosTransaccionManual);

        [OperationContract]
        void SuspenderTicket(long idTicket);

        [OperationContract]
        IEnumerable<BREAK_TYPE> ObtenerTiposDescansos();

        [OperationContract]
        int RegistrarDescanso(BREAK descanso);

        [OperationContract]
        void FinalizarDescanso(int idDescanso);
    }
}

using RECEPTIO.CapaAplicacion.Console.Aplicacion.Contratos;
using RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Contratos;
using RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Interfaces;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace RECEPTIO.CapaAplicacion.Console.Aplicacion.Interfaces
{
    [ServiceContract]
    public interface ISupervisor : IBase
    {
        [OperationContract]
        IEnumerable<Ticket> ObtenerTicketsNoAsignados();

        [OperationContract]
        short ReasignarTickets(Dictionary<long, Tuple<TipoTicket, short>> tickets);

        [OperationContract]
        void CancelarTickets(IEnumerable<long> idTickets, string userName);

        [OperationContract]
        IEnumerable<Ticket> ObtenerTicketsSuspendidos();

        [OperationContract]
        short ReasignarTicketsSuspendidos(Dictionary<long, Tuple<long, short>> tickets, int idMotivo, string usuario);

        [OperationContract]
        short ReasignarTicketsSuspendidosEspecifico(Dictionary<long, Tuple<long, short>> tickets, int idMotivo, string usuario, TROUBLE_DESK_USER usuarioSeleccionado);

        [OperationContract]
        IEnumerable<TicketReporte> ObtenerTicketsParaReporte(Dictionary<BusquedaTicketReporte, string> filtros);

        [OperationContract]
        IEnumerable<ACTION> ObtenerAccionesTicket(long idTicket);

        [OperationContract]
        IEnumerable<REASSIGNMENT_MOTIVE> ObtenerMotivosReasignacion();

        [OperationContract]
        IEnumerable<KIOSK> ObtenerKioscosActivos();

        [OperationContract]
        void RegistrarAperturaBarrera(LIFT_UP_BARRIER objecto);

        [OperationContract]
        IEnumerable<USER_SESSION> ObtenerSesionesUsuarios();

        [OperationContract]
        IEnumerable<PRE_GATE> ObtenerTransaccionesKiosco(Dictionary<BusquedaReporteTransacciones, string> filtros);

        [OperationContract]
        IEnumerable<DEVICE> ObtenerTablets();

        [OperationContract]
        IEnumerable<ZONE> ObtenerZonas();

        [OperationContract]
        IEnumerable<TRANSACTION_TYPE> ObtenerTiposTransacciones();

        [OperationContract]
        Tuple<bool, string, PRE_GATE> ObtenerInformacionParaReimpresionTicket(long idPreGate, bool esEntrada);

        [OperationContract]
        void RegistrarReimpresion(REPRINT objecto);

        [OperationContract]
        Respuesta ValidarIdPreGate(long idPreGate);

        [OperationContract]
        void CrearByPass(BY_PASS byPass, int idUsuario);

        [OperationContract]
        BY_PASS ObtenerByPass(long idPreGate);

        [OperationContract]
        void ActualizarrByPass(BY_PASS byPass, int idUsuario);

        [OperationContract]
        Respuesta ValidarIdPreGateParaCancelar(long idPreGate);

        [OperationContract]
        void CrearByPassCancelPregate(BY_PASS byPass, int idUsuario);

        [OperationContract]
        void ActualizarStatusPregate(long idPreGate, string Status);

        [OperationContract]
        void ActualizarStatusStockRegister(long idPreGate, bool Status);

        [OperationContract]
        IEnumerable<MOTIVE> ObtenerMotivos(int Type );

        [OperationContract]
        IEnumerable<SUB_MOTIVE> ObtenerSubMotivos(int idMotivo);

        [OperationContract]
        string ObtenerValidacionesGenerales(string _opcion, string _StrValor, int _IntValor, long _BigintValor);
    }

    public enum BusquedaTicketReporte
    {
        Id,
        Estado,
        Fecha,
        Usuario,
        Contenedor,
        Placa,
        CedulaChofer,
        TipoTicket,
        IdKiosco
    }

    public enum BusquedaReporteTransacciones
    {
        Fecha,
        GosTv,
        Usuario,
        CedulaChofer,
        Placa,
        IdDispositivo,
        Estado,
        IdTipoTransaccion,
        IdKiosco,
        IdZona
    }
}

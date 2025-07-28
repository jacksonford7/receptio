using RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Interfaces;
using System;

namespace RECEPTIO.CapaAplicacion.Console.Aplicacion.Interfaces
{
    public interface IProblema : IBase
    {
        Tuple<bool, string, string> RegistrarProblema(int idTransaccionQuiosco);
        Tuple<bool, string, string> RegistrarProblemaMobile(long idTosProcess, short idZona);
        Tuple<bool, string, string> RegistrarProblemaGenericoMobile(string mensajeError, short idZona);
        Tuple<bool, string, string> AnunciarProblemaTransaccionPendiente(long idPreGate, short idZona);
        Tuple<bool, string, string> RegistrarProblemaClienteAppTransaction(int idError, short idZona);
        Tuple<bool, string, string> RegistrarProblemaServicioWebTransaction(string error, short idZona, int idAplicacion);
    }
}

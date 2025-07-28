using RECEPTIO.CapaDominio.Nucleo.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RECEPTIO.CapaDominio.Nucleo.ServiciosDominio.Otros
{
    public abstract class AsignacionProblema
    {
        protected USER_SESSION ObtenerSesionUsuarioAlgoritmo(List<USER_SESSION> sesionesUsuarios)
        {
            var minimo = sesionesUsuarios.Min(us => us.PROCESS_TROUBLE_TICKETS.Count() + us.AUTO_TROUBLE_TICKETS.Count() + us.MOBILE_TROUBLE_TICKETS.Count() + us.CLIENT_APP_TRANSACTION_TROUBLE_TICKETS.Count());
            var sesionesUsuariosPosibles = sesionesUsuarios.Where(us => (us.PROCESS_TROUBLE_TICKETS.Count() + us.AUTO_TROUBLE_TICKETS.Count() + us.MOBILE_TROUBLE_TICKETS.Count() + us.CLIENT_APP_TRANSACTION_TROUBLE_TICKETS.Count()) == minimo);
            var random = new Random();
            var indice = random.Next(sesionesUsuariosPosibles.Count());
            return sesionesUsuariosPosibles.ToArray()[indice];
        }
    }
}

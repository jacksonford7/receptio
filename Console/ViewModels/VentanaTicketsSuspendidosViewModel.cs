using Console.Vistas;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;

namespace Console.ViewModels
{
    internal class VentanaTicketsSuspendidosViewModel : VentanaTicketsNoAsignadosViewModel
    {
        #region Variables
        #endregion

        #region Constructor
        internal VentanaTicketsSuspendidosViewModel(Windows.UI.Xaml.Controls.Page ventanaTicketsSuspendidos) : base(ventanaTicketsSuspendidos)
        {
        }
        #endregion

        #region Propiedades
        public override string Titulo
        {
            get
            {
                //return "TICKETS SUSPENDIDOS";
                return "REASIGNACIÓN TICKETS";
            }
        }
        #endregion

        #region Metodos
        protected override async void ObtenerTickets(object obj)
        {
            TodosTickets = await Servicio.ObtenerTicketsSuspendidosAsync();
            PresentarTickets();
        }

        protected override async void ReasignarTickets(object obj)
        {
            var tickets = new Dictionary<long, Tuple<long, short>>();
            foreach (var ticket in Tickets.Where(t => t.EstaChequeado))
                tickets.Add(ticket.IdTicket, new Tuple<long, short>(ticket.IdSesionUsuario.Value, ticket.IdZona));
            var ventana = new VentanaMotivosReasignacion { ViewModel = new VentanaMotivosReasignacionViewModel(Servicio, tickets) };
            await ventana.ShowAsync();
            if(ventana.ViewModel.Resultado != -1)
                MostrarMensajeAsync(ventana.ViewModel.Resultado);
        }

       
        #endregion
    }
}

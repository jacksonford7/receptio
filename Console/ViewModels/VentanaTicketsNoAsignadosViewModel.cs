using Console.ServicioConsole;
using Console.Vistas;
using RECEPTIO.CapaPresentacion.UWP.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Popups;

namespace Console.ViewModels
{
    internal class VentanaTicketsNoAsignadosViewModel : Base
    {
        #region Variables
        private readonly Windows.UI.Xaml.Controls.Page _ventanaTicketsNoAsignados;
        private RelayCommand _comandoRegresar;
        private RelayCommand _comandoActualizar;
        private RelayCommand _comandoReasignar;
        private RelayCommand _comandoCancelar;
        private ObservableCollection<Ticket> _tickets;
        private Ticket _ticketSeleccionado;
        private bool _chequearTodos;
        protected ServicioConsoleClient Servicio;
        protected ObservableCollection<Ticket> TodosTickets;
        #endregion

        #region Constructor
        internal VentanaTicketsNoAsignadosViewModel(Windows.UI.Xaml.Controls.Page ventanaTicketsNoAsignados)
        {
            _ventanaTicketsNoAsignados = ventanaTicketsNoAsignados;
            PropertyChanged += (s, e) => DelegarEventosCambiosAEventoComando();
            InstanciarComandos();
            InicializarServicioConsole();
            ObtenerTickets(null);
        }

        private void DelegarEventosCambiosAEventoComando()
        {
            _comandoReasignar.RaiseCanExecuteChanged();
            _comandoCancelar.RaiseCanExecuteChanged();
        }

        private void InstanciarComandos()
        {
            _comandoRegresar = new RelayCommand(Regresar);
            _comandoActualizar = new RelayCommand(ObtenerTickets);
            _comandoReasignar = new RelayCommand(ReasignarTickets, PuedoAccionarSobreTickets);
            _comandoCancelar = new RelayCommand(CancelarTickets, PuedoAccionarSobreTickets);
        }
        #endregion

        #region Propiedades
        public RelayCommand ComandoRegresar
        {
            get
            {
                return _comandoRegresar;
            }
            set
            {
                SetProperty(ref _comandoRegresar, value);
            }
        }

        public RelayCommand ComandoActualizar
        {
            get
            {
                return _comandoActualizar;
            }
            set
            {
                SetProperty(ref _comandoActualizar, value);
            }
        }

        public RelayCommand ComandoReasignar
        {
            get
            {
                return _comandoReasignar;
            }
            set
            {
                SetProperty(ref _comandoReasignar, value);
            }
        }

        public RelayCommand ComandoCancelar
        {
            get
            {
                return _comandoCancelar;
            }
            set
            {
                SetProperty(ref _comandoCancelar, value);
            }
        }

        public ObservableCollection<Ticket> Tickets
        {
            get
            {
                return _tickets;
            }
            set
            {
                SetProperty(ref _tickets, value);
            }
        }

        public Ticket TicketSeleccionado
        {
            get
            {
                return _ticketSeleccionado;
            }
            set
            {
                SetProperty(ref _ticketSeleccionado, value);
            }
        }

        public bool ChequearTodos
        {
            get
            {
                return _chequearTodos;
            }
            set
            {
                if (_chequearTodos == value)
                    return;
                _chequearTodos = value;
                RaisePropertyChanged("ChequearTodos");
                ChequearTodas();
            }
        }

        public virtual string Titulo
        {
            get
            {
                return "TICKETS NO ASIGNADOS";
            }
        }
        #endregion

        #region Metodos
        private void InicializarServicioConsole()
        {
            Servicio = (ServicioConsoleClient)App.Current.Resources["ServicioConsole"];
        }

        protected virtual async void ObtenerTickets(object obj)
        {
            TodosTickets = await Servicio.ObtenerTicketsNoAsignadosAsync();
            PresentarTickets();
        }

        protected void PresentarTickets()
        {
            Tickets = new ObservableCollection<Ticket>();
            foreach (var ticket in TodosTickets)
                Tickets.Add(ticket);
            DelegarEventosCambiosAEventoComando();
        }

        private void Regresar(object obj)
        {
            _ventanaTicketsNoAsignados.Frame.Navigate(typeof(VentanaSupervisor));
        }

        private bool PuedoAccionarSobreTickets(object obj)
        {
            return Tickets != null && Tickets.Any(t => t.EstaChequeado);
        }

        protected virtual async void ReasignarTickets(object obj)
        {
            var tickets = new Dictionary<long, Tuple<TipoTicket, short>>();
            foreach (var ticket in Tickets.Where(t => t.EstaChequeado))
                tickets.Add(ticket.IdTicket, new Tuple<TipoTicket, short>(ticket.Tipo, ticket.IdZona));
            var resultado = await Servicio.ReasignarTicketsAsync(tickets);
            MostrarMensajeAsync(resultado);
        }

        protected async void MostrarMensajeAsync(short resultado)
        {
            var mensajeDialogo = new MessageDialog("", "Reasignación de Tickets.");
            switch (resultado)
            {
                case 0:
                    mensajeDialogo.Content = "No se pudo reasignar ningún ticket porque no existe ninguna sesión de usuario disponible.";
                    break;
                case 1:
                    mensajeDialogo.Content = "No todos los tickets se pudieron reasignar, ya que no existen sesiones de usuarios disponibles en una zona determinada.";
                    ObtenerTickets(null);
                    ChequearTodos = false;
                    break;
                case 2:
                    mensajeDialogo.Content = "Reasignación Ok.";
                    ObtenerTickets(null);
                    ChequearTodos = false;
                    break;
                default:
                    break;
            }
            await mensajeDialogo.ShowAsync();
        }

        private async void CancelarTickets(object obj)
        {
            var mensajeDialogo = new MessageDialog("¿Está seguro de cancelar los tickets?", "Cancelación de Tickets.");
            mensajeDialogo.Commands.Add(new UICommand("Sí", new UICommandInvokedHandler(RealizarAccionCancelar)));
            mensajeDialogo.Commands.Add(new UICommand("No"));
            mensajeDialogo.DefaultCommandIndex = 1;
            mensajeDialogo.CancelCommandIndex = 1;
            await mensajeDialogo.ShowAsync();
        }

        private async void RealizarAccionCancelar(IUICommand command)
        {
            await Servicio.CancelarTicketsAsync(new ObservableCollection<long>(Tickets.Where(t => t.EstaChequeado).Select(t => t.IdTicket)), ((DatosLogin)App.Current.Resources["DatosLogin"]).Usuario);
            var mensajeDialogo = new MessageDialog("Proceso Ok.", "Cancelación de Tickets.");
            await mensajeDialogo.ShowAsync();
            ObtenerTickets(null);
            ChequearTodos = false;
        }

        private void ChequearTodas()
        {
            if (ChequearTodos)
            {
                if (Tickets.All(t => t.EstaChequeado))
                    return;
                CambiarChecks(true);
            }
            else
            {
                if (Tickets.All(t => t.EstaChequeado))
                    CambiarChecks(false);
            }
        }

        private void CambiarChecks(bool estaChequeado)
        {
            foreach (var ticket in TodosTickets)
                ticket.EstaChequeado = estaChequeado;
            PresentarTickets();
        }
        #endregion
    }
}
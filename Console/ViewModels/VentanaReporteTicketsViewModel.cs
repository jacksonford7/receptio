using Console.Modelos;
using Console.ServicioConsole;
using Console.Vistas;
using RECEPTIO.CapaPresentacion.UWP.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace Console.ViewModels
{
    internal class VentanaReporteTicketsViewModel : Base
    {
        #region Variables
        private readonly Page _pagina;
        private ServicioConsoleClient _servicio;
        private RelayCommand _comandoRegresar;
        private RelayCommand _comandoBuscar;
        private RelayCommand _comandoVerAcciones;
        private ObservableCollection<TicketReporte> _tickets;
        private TicketReporte _ticketSeleccionado;
        private long _idTicket;
        private DateTimeOffset _fechaDesde;
        private DateTimeOffset _fechaHasta;
        private ObservableCollection<EstadoTicket> _estados;
        private EstadoTicket _estadoSeleccionado;
        private string _userName;
        private ObservableCollection<TipoTicketParaReporte> _tiposTickets;
        private TipoTicketParaReporte _tipoTicketSeleccionado;
        private ObservableCollection<KIOSK> _quioscos;
        private KIOSK _quioscoSeleccionado;
        private string _contenedor;
        private string _placa;
        private string _cedulaChofer;
        private bool _estaHabilitadoCampo;
        #endregion

        #region Constructor
        internal VentanaReporteTicketsViewModel(Page pagina)
        {
            _pagina = pagina;
            PropertyChanged += (s, e) => DelegarEventosCambiosAEventoComando();
            InstanciarComandos();
            InicializarServicioConsole();
            EstablecerFechas();
            CargarEstados();
            CargarTiposTickets();
            CargarQuioscos();
        }

        private void DelegarEventosCambiosAEventoComando()
        {
            _comandoBuscar.RaiseCanExecuteChanged();
            _comandoVerAcciones.RaiseCanExecuteChanged();
        }

        private void InstanciarComandos()
        {
            _comandoRegresar = new RelayCommand(Regresar);
            _comandoBuscar = new RelayCommand(ObtenerTickets);
            _comandoVerAcciones = new RelayCommand(VerAcciones, PuedoVerAcciones);
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

        public RelayCommand ComandoBuscar
        {
            get
            {
                return _comandoBuscar;
            }
            set
            {
                SetProperty(ref _comandoBuscar, value);
            }
        }

        public RelayCommand ComandoVerAcciones
        {
            get
            {
                return _comandoVerAcciones;
            }
            set
            {
                SetProperty(ref _comandoVerAcciones, value);
            }
        }

        public ObservableCollection<TicketReporte> Tickets
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

        public TicketReporte TicketSeleccionado
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

        public ObservableCollection<EstadoTicket> Estados
        {
            get
            {
                return _estados;
            }
            set
            {
                SetProperty(ref _estados, value);
            }
        }

        public EstadoTicket EstadoSeleccionado
        {
            get
            {
                return _estadoSeleccionado;
            }
            set
            {
                SetProperty(ref _estadoSeleccionado, value);
            }
        }

        public long IdTicket
        {
            get
            {
                return _idTicket;
            }
            set
            {
                if (_idTicket == value)
                    return;
                _idTicket = value;
                RaisePropertyChanged("IdTicket");
            }
        }

        public DateTimeOffset FechaDesde
        {
            get
            {
                return _fechaDesde;
            }
            set
            {
                if (_fechaDesde == value)
                    return;
                _fechaDesde = value;
                RaisePropertyChanged("FechaDesde");
            }
        }

        public DateTimeOffset FechaHasta
        {
            get
            {
                return _fechaHasta;
            }
            set
            {
                if (_fechaHasta == value)
                    return;
                _fechaHasta = value;
                RaisePropertyChanged("FechaHasta");
            }
        }

        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                if (_userName == value)
                    return;
                _userName = value;
                RaisePropertyChanged("UserName");
            }
        }

        public ObservableCollection<TipoTicketParaReporte> TiposTickets
        {
            get
            {
                return _tiposTickets;
            }
            set
            {
                SetProperty(ref _tiposTickets, value);
            }
        }

        public TipoTicketParaReporte TipoTicketParaReporteSeleccionado
        {
            get
            {
                return _tipoTicketSeleccionado;
            }
            set
            {
                SetProperty(ref _tipoTicketSeleccionado, value);
                if (_tipoTicketSeleccionado != null && _tipoTicketSeleccionado.Codigo == "PR")
                    EstaHabilitadoCampo = true;
                else
                {
                    LimpiarCampos();
                    EstaHabilitadoCampo = false;
                }
            }
        }

        private void LimpiarCampos()
        {
            Contenedor = "";
            Placa = "";
            CedulaChofer = "";
        }

        public ObservableCollection<KIOSK> Quioscos
        {
            get
            {
                return _quioscos;
            }
            set
            {
                SetProperty(ref _quioscos, value);
            }
        }

        public KIOSK QuioscoSeleccionado
        {
            get
            {
                return _quioscoSeleccionado;
            }
            set
            {
                SetProperty(ref _quioscoSeleccionado, value);
            }
        }

        public string Contenedor
        {
            get
            {
                return _contenedor;
            }
            set
            {
                if (_contenedor == value)
                    return;
                _contenedor = value;
                RaisePropertyChanged("Contenedor");
            }
        }

        public string Placa
        {
            get
            {
                return _placa;
            }
            set
            {
                if (_placa == value)
                    return;
                _placa = value;
                RaisePropertyChanged("Placa");
            }
        }

        public string CedulaChofer
        {
            get
            {
                return _cedulaChofer;
            }
            set
            {
                if (_cedulaChofer == value)
                    return;
                _cedulaChofer = value;
                RaisePropertyChanged("CedulaChofer");
            }
        }

        public bool EstaHabilitadoCampo
        {
            get
            {
                return _estaHabilitadoCampo;
            }
            set
            {
                if (_estaHabilitadoCampo == value)
                    return;
                _estaHabilitadoCampo = value;
                RaisePropertyChanged("EstaHabilitadoCampo");
            }
        }
        #endregion

        #region Metodos
        private void InicializarServicioConsole()
        {
            _servicio = (ServicioConsoleClient)App.Current.Resources["ServicioConsole"];
        }

        private async void ObtenerTickets(object obj)
        {
            if (BotonPresionado)
                return;
            BotonPresionado = true;
            EstaOcupado = true;
            if (FechaHasta.Subtract(FechaDesde).TotalDays > 7)
            {
                var mensajeDialogo = new MessageDialog("El rango de fechas no debe exceder los 7 días.", "Consulta Tickets.");
                await mensajeDialogo.ShowAsync();
            }
            else
                Tickets = await _servicio.ObtenerTicketsParaReporteAsync(ObtenerFiltros());
            EstaOcupado = false;
            BotonPresionado = false;
        }

        private Dictionary<BusquedaTicketReporte, string> ObtenerFiltros()
        {
            var resultado = new Dictionary<BusquedaTicketReporte, string>();
            var manejadorBusqueda = new ManejadorBusquedaPorFecha();
            manejadorBusqueda.AplicarFiltro(ref resultado, this);
            return resultado;
        }

        private void Regresar(object obj)
        {
            _pagina.Frame.Navigate(typeof(VentanaSupervisor));
        }

        private void EstablecerFechas()
        {
            //FechaDesde = new DateTimeOffset(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day - 1, 0, 0, 0, TimeSpan.Zero);
            FechaDesde = DateTime.Today.AddDays(-1);
            FechaHasta = new DateTimeOffset(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, TimeSpan.Zero); ;
        }

        private void CargarEstados()
        {
            Estados = new ObservableCollection<EstadoTicket>
            {
                new EstadoTicket
                {
                    Codigo = "T",
                    Nombre = "Todos"
                },
                new EstadoTicket
                {
                    Codigo = "NT",
                    Nombre = "No Tomado"
                },
                new EstadoTicket
                {
                    Codigo = "ER",
                    Nombre = "En Resolución"
                },
                new EstadoTicket
                {
                    Codigo = "R",
                    Nombre = "Resuelto"
                }
            };
            EstadoSeleccionado = Estados.FirstOrDefault();
        }

        private void CargarTiposTickets()
        {
            TiposTickets = new ObservableCollection<TipoTicketParaReporte>
            {
                new TipoTicketParaReporte
                {
                    Codigo = "TO",
                    Nombre = "Todos"
                },
                new TipoTicketParaReporte
                {
                    Codigo = "PR",
                    Nombre = "Proceso"
                },
                new TipoTicketParaReporte
                {
                    Codigo = "AT",
                    Nombre = "Auto Ticket"
                },
                new TipoTicketParaReporte
                {
                    Codigo = "TC",
                    Nombre = "Técnico"
                },
                new TipoTicketParaReporte
                {
                    Codigo = "MB",
                    Nombre = "Mobile"
                }
            };
            TipoTicketParaReporteSeleccionado = TiposTickets.FirstOrDefault();
        }

        private async void CargarQuioscos()
        {
            Quioscos = await _servicio.ObtenerKioscosActivosAsync();
            Quioscos.Add(new KIOSK { NAME = "Todos"});
            QuioscoSeleccionado = Quioscos.LastOrDefault();
        }

        private bool PuedoVerAcciones(object obj)
        {
            return TicketSeleccionado != null;
        }

        private async void VerAcciones(object obj)
        {
            var ventana = new VentanaAccionTicket { ViewModel = new VentanaAccionTicketViewModel(_servicio, TicketSeleccionado.IdTicket) };
            await ventana.ShowAsync();
        }
        #endregion
    }

    internal abstract class ManejadorBusqueda
    {
        protected ManejadorBusqueda Siguiente;

        protected ManejadorBusqueda EstablecerSiguiente(ManejadorBusqueda siguiente)
        {
            Siguiente = siguiente;
            return Siguiente;
        }

        internal abstract void AplicarFiltro(ref Dictionary<BusquedaTicketReporte, string> filtros, VentanaReporteTicketsViewModel viewModel);
    }

    internal class ManejadorBusquedaPorFecha : ManejadorBusqueda
    {
        internal override void AplicarFiltro(ref Dictionary<BusquedaTicketReporte, string> filtros, VentanaReporteTicketsViewModel viewModel)
        {
            var fechaDesde = viewModel.FechaDesde;
            var fechaHasta = viewModel.FechaHasta;
            filtros.Add(BusquedaTicketReporte.Fecha, string.Format("{0}-{1}-{2}:{3}-{4}-{5}", fechaDesde.Year, fechaDesde.Month.ToString("00"), fechaDesde.Day.ToString("00")
                , fechaHasta.Year, fechaHasta.Month.ToString("00"), fechaHasta.Day.ToString("00")));
            
            EstablecerSiguiente(new ManejadorBusquedaPorEstado());
            Siguiente.AplicarFiltro(ref filtros, viewModel);
        }
    }

    internal class ManejadorBusquedaPorEstado : ManejadorBusqueda
    {
        internal override void AplicarFiltro(ref Dictionary<BusquedaTicketReporte, string> filtros, VentanaReporteTicketsViewModel viewModel)
        {
            filtros.Add(BusquedaTicketReporte.Estado, viewModel.EstadoSeleccionado.Codigo);
            EstablecerSiguiente(new ManejadorBusquedaPorIdTicket());
            Siguiente.AplicarFiltro(ref filtros, viewModel);
        }
    }

    internal class ManejadorBusquedaPorIdTicket : ManejadorBusqueda
    {
        internal override void AplicarFiltro(ref Dictionary<BusquedaTicketReporte, string> filtros, VentanaReporteTicketsViewModel viewModel)
        {
            if (viewModel.IdTicket > 0)
                filtros.Add(BusquedaTicketReporte.Id, viewModel.IdTicket.ToString());
            EstablecerSiguiente(new ManejadorBusquedaPorUsuario());
            Siguiente.AplicarFiltro(ref filtros, viewModel);
        }
    }

    internal class ManejadorBusquedaPorUsuario : ManejadorBusqueda
    {
        internal override void AplicarFiltro(ref Dictionary<BusquedaTicketReporte, string> filtros, VentanaReporteTicketsViewModel viewModel)
        {
            if (!string.IsNullOrWhiteSpace(viewModel.UserName))
                filtros.Add(BusquedaTicketReporte.Usuario, viewModel.UserName);
            EstablecerSiguiente(new ManejadorBusquedaPorTipoTicket());
            Siguiente.AplicarFiltro(ref filtros, viewModel);
        }
    }

    internal class ManejadorBusquedaPorTipoTicket : ManejadorBusqueda
    {
        internal override void AplicarFiltro(ref Dictionary<BusquedaTicketReporte, string> filtros, VentanaReporteTicketsViewModel viewModel)
        {
            filtros.Add(BusquedaTicketReporte.TipoTicket, viewModel.TipoTicketParaReporteSeleccionado.Codigo);
            if (viewModel.TipoTicketParaReporteSeleccionado.Codigo == "PR")
            {
                EstablecerSiguiente(new ManejadorBusquedaPorContenedor());
                Siguiente.AplicarFiltro(ref filtros, viewModel);
            }
        }
    }

    internal class ManejadorBusquedaPorContenedor : ManejadorBusqueda
    {
        internal override void AplicarFiltro(ref Dictionary<BusquedaTicketReporte, string> filtros, VentanaReporteTicketsViewModel viewModel)
        {
            if (!string.IsNullOrWhiteSpace(viewModel.Contenedor))
                filtros.Add(BusquedaTicketReporte.Contenedor, viewModel.Contenedor);
            EstablecerSiguiente(new ManejadorBusquedaPorPlaca());
            Siguiente.AplicarFiltro(ref filtros, viewModel);
        }
    }

    internal class ManejadorBusquedaPorPlaca : ManejadorBusqueda
    {
        internal override void AplicarFiltro(ref Dictionary<BusquedaTicketReporte, string> filtros, VentanaReporteTicketsViewModel viewModel)
        {
            if (!string.IsNullOrWhiteSpace(viewModel.Placa))
                filtros.Add(BusquedaTicketReporte.Placa, viewModel.Placa);
            EstablecerSiguiente(new ManejadorBusquedaPorCedulaChofer());
            Siguiente.AplicarFiltro(ref filtros, viewModel);
        }
    }

    internal class ManejadorBusquedaPorCedulaChofer : ManejadorBusqueda
    {
        internal override void AplicarFiltro(ref Dictionary<BusquedaTicketReporte, string> filtros, VentanaReporteTicketsViewModel viewModel)
        {
            if (!string.IsNullOrWhiteSpace(viewModel.CedulaChofer))
                filtros.Add(BusquedaTicketReporte.CedulaChofer, viewModel.CedulaChofer);
            EstablecerSiguiente(new ManejadorBusquedaPorQuiosco());
            Siguiente.AplicarFiltro(ref filtros, viewModel);
        }
    }

    internal class ManejadorBusquedaPorQuiosco : ManejadorBusqueda
    {
        internal override void AplicarFiltro(ref Dictionary<BusquedaTicketReporte, string> filtros, VentanaReporteTicketsViewModel viewModel)
        {
            if (viewModel.QuioscoSeleccionado.KIOSK_ID != 0)
                filtros.Add(BusquedaTicketReporte.IdKiosco, viewModel.QuioscoSeleccionado.KIOSK_ID.ToString());
        }
    }
}

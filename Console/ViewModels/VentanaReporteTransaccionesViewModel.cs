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
    internal class VentanaReporteTransaccionesViewModel : Base
    {
        #region Variables
        private readonly Page _pagina;
        private ServicioConsoleClient _servicio;
        private RelayCommand _comandoRegresar;
        private RelayCommand _comandoBuscar;
        private ObservableCollection<PRE_GATE> _preGates;
        private ObservableCollection<TOS_PROCCESS> _procesosPreGate;
        private ObservableCollection<KIOSK_TRANSACTION> _transacciones;
        private ObservableCollection<PROCESS> _procesos;
        private PRE_GATE _preGateSeleccionado;
        private KIOSK_TRANSACTION _transaccionSeleccionada;
        private long _idPreGate;
        private DateTimeOffset _fechaDesde;
        private DateTimeOffset _fechaHasta;
        private string _userName;
        private string _cedulaChofer;
        private string _placa;
        private ObservableCollection<DEVICE> _dispositivos;
        private DEVICE _dispositivoSeleccionado;
        private ObservableCollection<EstadoTicket> _estados;
        private EstadoTicket _estadoSeleccionado;
        private ObservableCollection<TRANSACTION_TYPE> _tiposTransaciones;
        private TRANSACTION_TYPE _tipoTransaccionSeleccionada;
        private ObservableCollection<KIOSK> _kioscos;
        private KIOSK _kioscoSeleccionado;
        private ObservableCollection<ZONE> _zonas;
        private ZONE _zonaSeleccionada;
        #endregion

        #region Constructor
        internal VentanaReporteTransaccionesViewModel(Page pagina)
        {
            _pagina = pagina;
            PropertyChanged += (s, e) => DelegarEventosCambiosAEventoComando();
            InstanciarComandos();
            InicializarServicioConsole();
            EstablecerFechas();
            CargarDispositivosAsync();
            CargarEstados();
            CargarTiposTransaccionesAsync();
            CargarKioscosAsync();
            CargarZonasAsync();
        }

        private void DelegarEventosCambiosAEventoComando()
        {
            _comandoBuscar.RaiseCanExecuteChanged();
        }

        private void InstanciarComandos()
        {
            _comandoRegresar = new RelayCommand(Regresar);
            _comandoBuscar = new RelayCommand(ObtenerConsultaAsync);
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

        public ObservableCollection<PRE_GATE> PreGates
        {
            get
            {
                return _preGates;
            }
            set
            {
                SetProperty(ref _preGates, value);
            }
        }

        public PRE_GATE PreGateSeleccionado
        {
            get
            {
                return _preGateSeleccionado;
            }
            set
            {
                SetProperty(ref _preGateSeleccionado, value);
                if (_preGateSeleccionado != null)
                {
                    ObtenerTransacciones();
                    ObtenerProcesosPreGate();
                }
            }
        }

        public ObservableCollection<TOS_PROCCESS> ProcesosPreGate
        {
            get
            {
                return _procesosPreGate;
            }
            set
            {
                SetProperty(ref _procesosPreGate, value);
            }
        }

        public ObservableCollection<KIOSK_TRANSACTION> Transacciones
        {
            get
            {
                return _transacciones;
            }
            set
            {
                SetProperty(ref _transacciones, value);
            }
        }

        public KIOSK_TRANSACTION TransaccionSeleccionada
        {
            get
            {
                return _transaccionSeleccionada;
            }
            set
            {
                SetProperty(ref _transaccionSeleccionada, value);
                if (_transaccionSeleccionada != null)
                    ObtenerProcesos();
            }
        }

        public ObservableCollection<PROCESS> Procesos
        {
            get
            {
                return _procesos;
            }
            set
            {
                SetProperty(ref _procesos, value);
            }
        }

        public long IdPreGate
        {
            get
            {
                return _idPreGate;
            }
            set
            {
                if (_idPreGate == value)
                    return;
                _idPreGate = value;
                RaisePropertyChanged("IdPreGate");
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

        public ObservableCollection<DEVICE> Dispositivos
        {
            get
            {
                return _dispositivos;
            }
            set
            {
                SetProperty(ref _dispositivos, value);
            }
        }

        public DEVICE DispositivoSeleccionado
        {
            get
            {
                return _dispositivoSeleccionado;
            }
            set
            {
                SetProperty(ref _dispositivoSeleccionado, value);
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

        public ObservableCollection<TRANSACTION_TYPE> TiposTransaciones
        {
            get
            {
                return _tiposTransaciones;
            }
            set
            {
                SetProperty(ref _tiposTransaciones, value);
            }
        }

        public TRANSACTION_TYPE TipoTransaccionSeleccionada
        {
            get
            {
                return _tipoTransaccionSeleccionada;
            }
            set
            {
                SetProperty(ref _tipoTransaccionSeleccionada, value);
            }
        }

        public ObservableCollection<KIOSK> Kioscos
        {
            get
            {
                return _kioscos;
            }
            set
            {
                SetProperty(ref _kioscos, value);
            }
        }

        public KIOSK KioscoSeleccionado
        {
            get
            {
                return _kioscoSeleccionado;
            }
            set
            {
                SetProperty(ref _kioscoSeleccionado, value);
            }
        }

        public ObservableCollection<ZONE> Zonas
        {
            get
            {
                return _zonas;
            }
            set
            {
                SetProperty(ref _zonas, value);
            }
        }

        public ZONE ZonaSeleccionada
        {
            get
            {
                return _zonaSeleccionada;
            }
            set
            {
                SetProperty(ref _zonaSeleccionada, value);
            }
        }
        #endregion

        #region Metodos
        private void InicializarServicioConsole()
        {
            _servicio = (ServicioConsoleClient)App.Current.Resources["ServicioConsole"];
        }

        private void EstablecerFechas()
        {
            //FechaDesde = new DateTimeOffset(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day - 1, 0, 0, 0, TimeSpan.Zero);
            FechaDesde = DateTime.Today.AddDays(-1);
            FechaHasta = new DateTimeOffset(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, TimeSpan.Zero); ;
        }

        private async void CargarDispositivosAsync()
        {
            Dispositivos = await _servicio.ObtenerTabletsAsync();
            Dispositivos.Add(new DEVICE { NAME = "Todos" });
            DispositivoSeleccionado = Dispositivos.LastOrDefault();
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
                    Codigo = "N",
                    Nombre = "Nuevo"
                },
                new EstadoTicket
                {
                    Codigo = "C",
                    Nombre = "Cancelado"
                },
                new EstadoTicket
                {
                    Codigo = "P",
                    Nombre = "Proceso"
                },
                new EstadoTicket
                {
                    Codigo = "I",
                    Nombre = "Ingresado"
                },
                new EstadoTicket
                {
                    Codigo = "L",
                    Nombre = "Saliendo"
                },
                new EstadoTicket
                {
                    Codigo = "O",
                    Nombre = "Completado"
                }
            };
            EstadoSeleccionado = Estados.FirstOrDefault();
        }

        private async void CargarTiposTransaccionesAsync()
        {
            TiposTransaciones = await _servicio.ObtenerTiposTransaccionesAsync();
            TiposTransaciones.Add(new TRANSACTION_TYPE { DESCRIPTION = "TODAS" });
            TipoTransaccionSeleccionada = TiposTransaciones.LastOrDefault();
        }

        private async void CargarKioscosAsync()
        {
            Kioscos = await _servicio.ObtenerKioscosActivosAsync();
            Kioscos.Add(new KIOSK { NAME = "Todos" });
            KioscoSeleccionado = Kioscos.LastOrDefault();
        }

        private async void CargarZonasAsync()
        {
            Zonas = await _servicio.ObtenerZonasAsync();
            Zonas.Add(new ZONE { NAME = "Todas" });
            ZonaSeleccionada = Zonas.LastOrDefault();
        }

        private async void ObtenerConsultaAsync(object obj)
        {
            if (BotonPresionado)
                return;
            BotonPresionado = true;
            EstaOcupado = true;
            if (FechaHasta.Subtract(FechaDesde).TotalDays > 2)
            {
                var mensajeDialogo = new MessageDialog("El rango de fechas no debe exceder los 2 días.", "Consulta Transacciones.");
                await mensajeDialogo.ShowAsync();
            }
            else
            {
                LimpiarGrids();
                PreGates = await _servicio.ObtenerTransaccionesKioscoAsync(ObtenerFiltros());
                PreGateSeleccionado = PreGates.FirstOrDefault();
            }
            EstaOcupado = false;
            BotonPresionado = false;
        }

        private void LimpiarGrids()
        {
            Transacciones = null;
            TransaccionSeleccionada = null;
            Procesos = null;
            ProcesosPreGate = null;
        }

        private Dictionary<BusquedaReporteTransacciones, string> ObtenerFiltros()
        {
            var resultado = new Dictionary<BusquedaReporteTransacciones, string>();
            var manejadorBusqueda = new ManejadorBusquedaReportePorFecha();
            manejadorBusqueda.AplicarFiltro(ref resultado, this);
            return resultado;
        }

        private void ObtenerProcesosPreGate()
        {
            ProcesosPreGate = PreGateSeleccionado.TOS_PROCCESSES;
        }

        private void ObtenerTransacciones()
        {
            Transacciones = PreGateSeleccionado.KIOSK_TRANSACTIONS;
            TransaccionSeleccionada = Transacciones.FirstOrDefault();
        }

        private void ObtenerProcesos()
        {
            Procesos = TransaccionSeleccionada.PROCESSES;
        }

        private void Regresar(object obj)
        {
            _pagina.Frame.Navigate(typeof(VentanaSupervisor));
        }
        #endregion
    }

    internal abstract class ManejadorBusquedaReporte
    {
        protected ManejadorBusquedaReporte Siguiente;

        protected ManejadorBusquedaReporte EstablecerSiguiente(ManejadorBusquedaReporte siguiente)
        {
            Siguiente = siguiente;
            return Siguiente;
        }

        internal abstract void AplicarFiltro(ref Dictionary<BusquedaReporteTransacciones, string> filtros, VentanaReporteTransaccionesViewModel viewModel);
    }

    internal class ManejadorBusquedaReportePorFecha : ManejadorBusquedaReporte
    {
        internal override void AplicarFiltro(ref Dictionary<BusquedaReporteTransacciones, string> filtros, VentanaReporteTransaccionesViewModel viewModel)
        {
            var fechaDesde = viewModel.FechaDesde;
            var fechaHasta = viewModel.FechaHasta;
            filtros.Add(BusquedaReporteTransacciones.Fecha, string.Format("{0}-{1}-{2}:{3}-{4}-{5}", fechaDesde.Year, fechaDesde.Month.ToString("00"), fechaDesde.Day.ToString("00")
                , fechaHasta.Year, fechaHasta.Month.ToString("00"), fechaHasta.Day.ToString("00")));

            EstablecerSiguiente(new ManejadorBusquedaPorIdPreGate());
            Siguiente.AplicarFiltro(ref filtros, viewModel);
        }
    }

    internal class ManejadorBusquedaPorIdPreGate : ManejadorBusquedaReporte
    {
        internal override void AplicarFiltro(ref Dictionary<BusquedaReporteTransacciones, string> filtros, VentanaReporteTransaccionesViewModel viewModel)
        {
            if (viewModel.IdPreGate > 0)
                filtros.Add(BusquedaReporteTransacciones.GosTv, viewModel.IdPreGate.ToString());
            EstablecerSiguiente(new ManejadorBusquedaReportePorUsuario());
            Siguiente.AplicarFiltro(ref filtros, viewModel);
        }
    }

    internal class ManejadorBusquedaReportePorUsuario : ManejadorBusquedaReporte
    {
        internal override void AplicarFiltro(ref Dictionary<BusquedaReporteTransacciones, string> filtros, VentanaReporteTransaccionesViewModel viewModel)
        {
            if (!string.IsNullOrWhiteSpace(viewModel.UserName))
                filtros.Add(BusquedaReporteTransacciones.Usuario, viewModel.UserName);
            EstablecerSiguiente(new ManejadorBusquedaReportePorCedulaChofer());
            Siguiente.AplicarFiltro(ref filtros, viewModel);
        }
    }

    internal class ManejadorBusquedaReportePorCedulaChofer : ManejadorBusquedaReporte
    {
        internal override void AplicarFiltro(ref Dictionary<BusquedaReporteTransacciones, string> filtros, VentanaReporteTransaccionesViewModel viewModel)
        {
            if (!string.IsNullOrWhiteSpace(viewModel.CedulaChofer))
                filtros.Add(BusquedaReporteTransacciones.CedulaChofer, viewModel.CedulaChofer);
            EstablecerSiguiente(new ManejadorBusquedaReportePorPlaca());
            Siguiente.AplicarFiltro(ref filtros, viewModel);
        }
    }

    internal class ManejadorBusquedaReportePorPlaca : ManejadorBusquedaReporte
    {
        internal override void AplicarFiltro(ref Dictionary<BusquedaReporteTransacciones, string> filtros, VentanaReporteTransaccionesViewModel viewModel)
        {
            if (!string.IsNullOrWhiteSpace(viewModel.Placa))
                filtros.Add(BusquedaReporteTransacciones.Placa, viewModel.Placa);
            EstablecerSiguiente(new ManejadorBusquedaReportePorDispositivo());
            Siguiente.AplicarFiltro(ref filtros, viewModel);
        }
    }

    internal class ManejadorBusquedaReportePorDispositivo : ManejadorBusquedaReporte
    {
        internal override void AplicarFiltro(ref Dictionary<BusquedaReporteTransacciones, string> filtros, VentanaReporteTransaccionesViewModel viewModel)
        {
            if (viewModel.DispositivoSeleccionado.DEVICE_ID != 0)
                filtros.Add(BusquedaReporteTransacciones.IdDispositivo, viewModel.DispositivoSeleccionado.DEVICE_ID.ToString());
            EstablecerSiguiente(new ManejadorBusquedaReportePorEstado());
            Siguiente.AplicarFiltro(ref filtros, viewModel);
        }
    }

    internal class ManejadorBusquedaReportePorEstado : ManejadorBusquedaReporte
    {
        internal override void AplicarFiltro(ref Dictionary<BusquedaReporteTransacciones, string> filtros, VentanaReporteTransaccionesViewModel viewModel)
        {
            if (viewModel.EstadoSeleccionado.Codigo != "T")
                filtros.Add(BusquedaReporteTransacciones.Estado, viewModel.EstadoSeleccionado.Codigo);
            EstablecerSiguiente(new ManejadorBusquedaReportePorTipoTransaccion());
            Siguiente.AplicarFiltro(ref filtros, viewModel);
        }
    }

    internal class ManejadorBusquedaReportePorTipoTransaccion : ManejadorBusquedaReporte
    {
        internal override void AplicarFiltro(ref Dictionary<BusquedaReporteTransacciones, string> filtros, VentanaReporteTransaccionesViewModel viewModel)
        {
            if (viewModel.TipoTransaccionSeleccionada.ID != 0)
                filtros.Add(BusquedaReporteTransacciones.IdTipoTransaccion, viewModel.TipoTransaccionSeleccionada.ID.ToString());
            EstablecerSiguiente(new ManejadorBusquedaReportePorKiosco());
            Siguiente.AplicarFiltro(ref filtros, viewModel);
        }
    }

    internal class ManejadorBusquedaReportePorKiosco : ManejadorBusquedaReporte
    {
        internal override void AplicarFiltro(ref Dictionary<BusquedaReporteTransacciones, string> filtros, VentanaReporteTransaccionesViewModel viewModel)
        {
            if (viewModel.KioscoSeleccionado.KIOSK_ID != 0)
                filtros.Add(BusquedaReporteTransacciones.IdKiosco, viewModel.KioscoSeleccionado.KIOSK_ID.ToString());
            EstablecerSiguiente(new ManejadorBusquedaReportePorZona());
            Siguiente.AplicarFiltro(ref filtros, viewModel);
        }
    }

    internal class ManejadorBusquedaReportePorZona : ManejadorBusquedaReporte
    {
        internal override void AplicarFiltro(ref Dictionary<BusquedaReporteTransacciones, string> filtros, VentanaReporteTransaccionesViewModel viewModel)
        {
            if (viewModel.ZonaSeleccionada.ZONE_ID != 0)
                filtros.Add(BusquedaReporteTransacciones.IdZona, viewModel.ZonaSeleccionada.ZONE_ID.ToString());
        }
    }
}

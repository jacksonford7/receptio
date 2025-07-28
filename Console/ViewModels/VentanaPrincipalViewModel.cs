using Console.Modelos;
using Console.ServicioConsole;
using Console.Vistas;
using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using RECEPTIO.CapaPresentacion.UWP.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Console.ViewModels
{
    #region Informacion Básica
    internal partial class VentanaPrincipalViewModel : Base
    {
        #region Variables
        private readonly VentanaPrincipal _ventanaPrincipal;
        private ServicioConsoleClient _servicio;
        private ServicioTransactionQuiosco.ContratoClient _servicioQuiosco;
        private StorageFileQueryResult _consultaCarpeta;
        private IReadOnlyList<StorageFile> _archivos;
        private ObservableCollection<MOTIVE> _motivos;
        private MOTIVE _motivoSeleccionado;
        private ObservableCollection<SUB_MOTIVE> _subMotivos;
        private SUB_MOTIVE _subMotivoSeleccionado;
        #endregion

        #region Constructor
        internal VentanaPrincipalViewModel(VentanaPrincipal ventanaPrincipal)
        {
            _ventanaPrincipal = ventanaPrincipal;
            EstaHabilitado = true;
            InstanciarComandos();
            InicializarVariablesDispositivos();
            InstanciarComandosControlTicket();
            InstanciarComandosAduana();
            PropertyChanged += (s, e) => DelegarEventosCambiosAEventoComando();
            InicializarServicioConsole();
            InstanciarDispatcher();
            PrepararObservadorAsync();
            ObtenerTickets();
            ObtenerMotivos();
        }

        private void DelegarEventosCambiosAEventoComando()
        {
            _comandoAutoTicket.RaiseCanExecuteChanged();
            _comandoSuspenderTicket.RaiseCanExecuteChanged();
            _comandoActualizar.RaiseCanExecuteChanged();
            _comandoDescanso.RaiseCanExecuteChanged();
            _comandoCerrarSesion.RaiseCanExecuteChanged();
            _comandoAbrirBarrera.RaiseCanExecuteChanged();
            _comandoIrHome.RaiseCanExecuteChanged();
            _comandoReimprimirTicket.RaiseCanExecuteChanged();
            _comandoAceptarTicket.RaiseCanExecuteChanged();
            _comandoCerrarTicket.RaiseCanExecuteChanged();
        }
        #endregion

        #region Propiedades
        public int TicketsResueltos
        {
            get
            {
                return (_tickets == null) ? 0 : _tickets.Count(t => t.FechaFinalizacion.HasValue);
            }
        }

        public string TiempoPromedioResolucion
        {
            get
            {
                if (_tickets == null || _tickets.Count() == 0)
                    return "0";
                var tickets = _tickets.Where(t => t.FechaFinalizacion.HasValue);
                if(tickets == null || tickets.Count() == 0)
                    return "0";
                var minutos = tickets.Average(t => (t.FechaFinalizacion.Value - t.FechaAceptacion).Value.TotalMinutes);
                var segundos = minutos * 60;
                var minutosCompletos = Math.Truncate(minutos);
                return $"{minutosCompletos.ToString("00")} : {(segundos - (minutosCompletos * 60)).ToString("00")}";
            }
        }

        public string Zona
        {
            get
            {
                return ((DatosLogin)App.Current.Resources["DatosLogin"]).Zona;
            }
        }

        public ObservableCollection<MOTIVE> Motivos
        {
            get
            {
                return _motivos;
            }
            set
            {
                SetProperty(ref _motivos, value);
            }
        }

        public MOTIVE MotivoSeleccionado
        {
            get
            {
                return _motivoSeleccionado;
            }
            set
            {
                SetProperty(ref _motivoSeleccionado, value);
                SetProperty(ref _idMotivos, _motivoSeleccionado == null? 0: _motivoSeleccionado.MOTIVE_ID);
            }
            
        }

        public ObservableCollection<SUB_MOTIVE> SubMotivos
        {
            get
            {
                return _subMotivos;
            }
            set
            {
                SetProperty(ref _subMotivos, value);
            }
        }

        public SUB_MOTIVE SubMotivoSeleccionado
        {
            get
            {
                return _subMotivoSeleccionado;
            }
            set
            {
                SetProperty(ref _subMotivoSeleccionado, value);
                SetProperty(ref _idSubMotivo, _subMotivoSeleccionado == null ? 0 : _subMotivoSeleccionado.SUB_MOTIVE_ID);
            }
        }

        
        #endregion

        #region Metodos
        private void InicializarServicioConsole()
        {
            _servicio = (ServicioConsoleClient)App.Current.Resources["ServicioConsole"];
        }

        private async void PrepararObservadorAsync()
        {
            var filtros = new List<string>{".txt"};
            var opciones = new QueryOptions(CommonFileQuery.OrderByName, filtros);
            _consultaCarpeta = ApplicationData.Current.LocalFolder.CreateFileQueryWithOptions(opciones);
            _consultaCarpeta.ContentsChanged += Observador;
            _archivos = await _consultaCarpeta.GetFilesAsync();
        }

        private void Observador(IStorageQueryResultBase sender, object args)
        {
            var titulo = "Hay un problema";
            var contenido = LeerMensajeErrorArchivo();
            if (contenido == "")
                return;
            var logo = "../Imagenes/IconoReceptio.jpg";
            ToastVisual visual = new ToastVisual()
            {
                BindingGeneric = new ToastBindingGeneric()
                {
                    Children =
                    {
                        new AdaptiveText()
                        {
                            Text = titulo
                        },

                        new AdaptiveText()
                        {
                            Text = contenido
                        }
                    },
                    AppLogoOverride = new ToastGenericAppLogo()
                    {
                        Source = logo,
                        HintCrop = ToastGenericAppLogoCrop.Circle
                    }
                }
            };
            ToastActionsCustom actions = new ToastActionsCustom()
            {
                Buttons =
                {
                    new ToastButton("Ver", new QueryString()
                    {
                        { "action", "VerTickets" }
                    }.ToString())
                }
            };
            ToastContent toastContent = new ToastContent()
            {
                Visual = visual,
                Actions = actions
            };
            ToastNotification notification = new ToastNotification(toastContent.GetXml());
            ToastNotificationManager.CreateToastNotifier().Show(notification);
            BorrarArchivo();
        }

        private string LeerMensajeErrorArchivo()
        {
            return System.IO.File.Exists($@"{ApplicationData.Current.LocalFolder.Path}\Observador.txt") ?  System.IO.File.ReadAllText($@"{ApplicationData.Current.LocalFolder.Path}\Observador.txt") : "";
        }

        private void BorrarArchivo()
        {
            if (System.IO.File.Exists($@"{ApplicationData.Current.LocalFolder.Path}\Observador.txt"))
                System.IO.File.Delete($@"{ApplicationData.Current.LocalFolder.Path}\Observador.txt");
        }

        private void ActualizarPropiedadesBarraEstado()
        {
            RaisePropertyChanged("TicketsResueltos");
            RaisePropertyChanged("TiempoPromedioResolucion");
        }

        private async void ObtenerMotivos()
        {
            Motivos = await _servicio.ObtenerMotivosAsync(1);// parametro enviado es el type(tipo de lista de motivos)
            MotivoSeleccionado = Motivos.FirstOrDefault();
        }

        private async void ObtenerSubMotivosAsync(int idMotivo)
        {
            SubMotivos = await _servicio.ObtenerSubMotivosAsync(idMotivo);
            SubMotivoSeleccionado = SubMotivos.FirstOrDefault();
        }

        public void ObtenerSubMotivos()
        {
            ObtenerSubMotivosAsync(MotivoSeleccionado.MOTIVE_ID);
        }
        #endregion
    }
    #endregion

    #region Tickets
    internal partial class VentanaPrincipalViewModel
    {
        #region Variables
        private ObservableCollection<Ticket> _tickets;
        private Ticket _ticketSeleccionado;
        private DetalleTicket _detallesTicket;
        #endregion

        #region Propiedades
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
                if(_ticketSeleccionado != null)
                    ProcesosCuandoSeleccionaIndiceTicket();
            }
        }

        private void ProcesosCuandoSeleccionaIndiceTicket()
        {
            if (TicketSeleccionado.Tipo == TipoTicket.Proceso)
                ObtenerDetallesTicket();
            else if (TicketSeleccionado.Tipo == TipoTicket.Tecnico || TicketSeleccionado.Tipo == TipoTicket.Mobile)
            {
                _detallesTicket = new DetalleTicket { Procesos = new ObservableCollection<Proceso> { new Proceso { FechaProceso = TicketSeleccionado.FechaCreacion, MensajeUsuario = TicketSeleccionado.Mensaje, MensajeTecnico = TicketSeleccionado.Mensaje, MensajeEspecifico = TicketSeleccionado.Mensaje } } };
                ActualizarPropiedades();
            }
            else
            {
                _detallesTicket = null;
                ActualizarPropiedades();
            }
            ActualizarPropiedadesControlTicket();
            EstablecerTiempo();
            ObtenerDispositivos();
            TransaccionContenedores = null;
            MensajesSmdt = null;
        }

        public string TipoTransaccion
        {
            get
            {
                return (_detallesTicket == null) ? "" : _detallesTicket.TipoTransaccion;
            }
        }

        public string Cedula
        {
            get
            {
                return (_detallesTicket == null) ? "" : _detallesTicket.CedulaChofer;
            }
        }

        public string Placa
        {
            get
            {
                return (_detallesTicket == null) ? "" : _detallesTicket.PlacaCamion;
            }
        }

        public string Contenedores
        {
            get
            {
                return (_detallesTicket == null) ? "" : _detallesTicket.Containers;
            }
        }

        public ObservableCollection<Proceso> Procesos
        {
            get
            {
                return (_detallesTicket == null) ? null : _detallesTicket.Procesos;
            }
        }
        #endregion

        #region Metodos
        private async void ObtenerTickets()
        {
            if (BotonPresionado)
                return;
            BotonPresionado = true;
            EstaOcupado = true;
            _detallesTicket = null;
            TransaccionContenedores = null;
            MensajesSmdt = null;
            Tickets = await _servicio.ObtenerTicketsAsync(((DatosLogin)App.Current.Resources["DatosLogin"]).IdSesion);
            ActualizarPropiedadesBarraEstado();
            _dispatcher.Stop();
            EstaOcupado = false;
            BotonPresionado = false;
        }

        private async void ObtenerDetallesTicket()
        {
            EstaHabilitado = false;
            EstaOcupado = true;
            _detallesTicket = await _servicio.ObtenerDetallesTicketAsync(TicketSeleccionado.IdTransaccionQuiosco);
            ActualizarPropiedades();
            ObtenerTransaccionContenedores();
            EstaOcupado = false;
            EstaHabilitado = true;
        }

        private void ActualizarPropiedades()
        {
            RaisePropertyChanged("TipoTransaccion");
            RaisePropertyChanged("Cedula");
            RaisePropertyChanged("Placa");
            RaisePropertyChanged("Contenedores");
            RaisePropertyChanged("Procesos");
        }
        #endregion
    }
    #endregion

    #region Control Ticket
    internal partial class VentanaPrincipalViewModel
    {
        #region Variables
        private DispatcherTimer _dispatcher;
        private string _tiempo;
        private string _notas;
        private int _idMotivos;
        private int _idSubMotivo;
        private bool _estaHabilitadoNotas;
        private RelayCommand _comandoAceptarTicket;
        private RelayCommand _comandoCerrarTicket;
        private Visibility _esVisibleAceptarTicket;
        private Visibility _esVisibleCerrarTicket;
        private int _alturaNotas;
        #endregion

        #region Constructor
        private void InstanciarComandosControlTicket()
        {
            _comandoAceptarTicket = new RelayCommand(AceptarTicket, PuedoAceptarTicket);
            _comandoCerrarTicket = new RelayCommand(CerrarTicket, PuedoCerrarTicket);
            EsVisibleAceptarTicket = Visibility.Collapsed;
            EsVisibleCerrarTicket = Visibility.Collapsed;
            AlturaNotas = 160;
        }

        private void InstanciarDispatcher()
        {
            _dispatcher = new DispatcherTimer { Interval = new TimeSpan(0, 0, 1) };
            _dispatcher.Stop();
            _dispatcher.Tick += Temporizador;
        }
        #endregion

        #region Propiedades
        public RelayCommand ComandoAceptarTicket
        {
            get
            {
                return _comandoAceptarTicket;
            }
            set
            {
                SetProperty(ref _comandoAceptarTicket, value);
            }
        }

        public RelayCommand ComandoCerrarTicket
        {
            get
            {
                return _comandoCerrarTicket;
            }
            set
            {
                SetProperty(ref _comandoCerrarTicket, value);
            }
        }

        public long IdTicket
        {
            get
            {
                return (TicketSeleccionado == null) ? 0 : TicketSeleccionado.IdTicket;
            }
        }

        public string FechaCreacion
        {
            get
            {
                return (TicketSeleccionado == null) ? "" : TicketSeleccionado.FechaCreacion.ToString();
            }
        }

        public string FechaAsignacion
        {
            get
            {
                return (TicketSeleccionado == null) ? "" : TicketSeleccionado.FechaAsignacion.ToString();
            }
        }

        public string FechaAceptacion
        {
            get
            {
                return (TicketSeleccionado == null || !TicketSeleccionado.FechaAceptacion.HasValue) ? "" : TicketSeleccionado.FechaAceptacion.Value.ToString();
            }
        }

        public string FechaFinalizacion
        {
            get
            {
                return (TicketSeleccionado == null || !TicketSeleccionado.FechaFinalizacion.HasValue) ? "" : TicketSeleccionado.FechaFinalizacion.Value.ToString();
            }
        }

        public string Tiempo
        {
            get
            {
                return _tiempo;
            }
            set
            {
                if (_tiempo == value)
                    return;
                _tiempo = value;
                RaisePropertyChanged("Tiempo");
            }
        }

        public string Notas
        {
            get
            {
                return _notas;
            }
            set
            {
                if (_notas == value)
                    return;
                _notas = value;
                RaisePropertyChanged("Notas");
            }
        }

        public int IdMotivo
        {
            get
            {
                return _idMotivos;
            }
            set
            {
                if (_idMotivos == value)
                    return;
                _idMotivos = value;
                RaisePropertyChanged("IdMotivo");
            }
        }

        public int IdSubMotivo
        {
            get
            {
                return _idSubMotivo;
            }
            set
            {
                if (_idSubMotivo == value)
                    return;
                _idSubMotivo = value;
                RaisePropertyChanged("IdSubMotivo");
            }
        }
        public bool EstaHabilitadoNotas
        {
            get
            {
                return _estaHabilitadoNotas;
            }
            set
            {
                if (_estaHabilitadoNotas == value)
                    return;
                _estaHabilitadoNotas = value;
                RaisePropertyChanged("EstaHabilitadoNotas");
            }
        }

        public Visibility EsVisibleAceptarTicket
        {
            get
            {
                return _esVisibleAceptarTicket;
            }
            set
            {
                if (_esVisibleAceptarTicket == value)
                    return;
                _esVisibleAceptarTicket = value;
                RaisePropertyChanged("EsVisibleAceptarTicket");
            }
        }

        public Visibility EsVisibleCerrarTicket
        {
            get
            {
                return _esVisibleCerrarTicket;
            }
            set
            {
                if (_esVisibleCerrarTicket == value)
                    return;
                _esVisibleCerrarTicket = value;
                RaisePropertyChanged("EsVisibleCerrarTicket");
            }
        }

        public int AlturaNotas
        {
            get
            {
                return _alturaNotas;
            }
            set
            {
                if (_alturaNotas == value)
                    return;
                _alturaNotas = value;
                RaisePropertyChanged("AlturaNotas");
            }
        }
        #endregion

        #region Metodos
        private void Temporizador(object sender, object e)
        {
            var minutos = (DateTime.Now - TicketSeleccionado.FechaAceptacion.Value).TotalMinutes;
            var segundos = (DateTime.Now - TicketSeleccionado.FechaAceptacion.Value).TotalSeconds;
            var minutosCompletos = Math.Truncate(minutos);
            Tiempo = $"{minutosCompletos.ToString("00")}: {(segundos - (minutosCompletos * 60)).ToString("00")}";
        }

        private void ActualizarPropiedadesControlTicket()
        {
            EstaHabilitadoNotas = TicketSeleccionado != null && !string.IsNullOrWhiteSpace(FechaAceptacion) && string.IsNullOrWhiteSpace(FechaFinalizacion);
            EsVisibleAceptarTicket = TicketSeleccionado != null && string.IsNullOrWhiteSpace(FechaAceptacion) ? Visibility.Visible : Visibility.Collapsed;
            EsVisibleCerrarTicket = TicketSeleccionado != null && !string.IsNullOrWhiteSpace(FechaAceptacion) && string.IsNullOrWhiteSpace(FechaFinalizacion) ? Visibility.Visible : Visibility.Collapsed;
            Notas = TicketSeleccionado.Notas;

            AlturaNotas = EsVisibleAceptarTicket == Visibility.Visible || EsVisibleCerrarTicket == Visibility.Visible ? 113 : 160;
            RaisePropertyChanged("IdTicket");
            RaisePropertyChanged("FechaCreacion");
            RaisePropertyChanged("FechaAsignacion");
            RaisePropertyChanged("FechaAceptacion");
            RaisePropertyChanged("FechaFinalizacion");
        }

        private void EstablecerTiempo()
        {
            _dispatcher.Stop();
            if (string.IsNullOrWhiteSpace(FechaAceptacion))
                Tiempo = "";
            else if (!string.IsNullOrWhiteSpace(FechaFinalizacion))
            {
                var minutos = (TicketSeleccionado.FechaFinalizacion.Value - TicketSeleccionado.FechaAceptacion.Value).TotalMinutes;
                var segundos = (TicketSeleccionado.FechaFinalizacion.Value - TicketSeleccionado.FechaAceptacion.Value).TotalSeconds;
                var minutosCompletos = Math.Truncate(minutos);
                Tiempo = $"{minutosCompletos.ToString("00")}: {(segundos - (minutosCompletos * 60)).ToString("00")}";
            }
            else
                _dispatcher.Start();
        }

        private bool PuedoAceptarTicket(object obj)
        {
            return TicketSeleccionado != null && string.IsNullOrWhiteSpace(FechaAceptacion);
        }

        private async void AceptarTicket(object obj)
        {
            await _servicio.AceptarTicketAsync(IdTicket);
            EstaHabilitadoNotas = false;
            EsVisibleAceptarTicket = Visibility.Collapsed;
            EsVisibleCerrarTicket = Visibility.Collapsed;
            AlturaNotas = 160;
            ObtenerTickets();
        }

        private bool PuedoCerrarTicket(object obj)
        {
            return TicketSeleccionado != null && !string.IsNullOrWhiteSpace(FechaAceptacion) && string.IsNullOrWhiteSpace(FechaFinalizacion) && !string.IsNullOrWhiteSpace(Notas);
        }

        private async void CerrarTicket(object obj)
        {

            await _servicio.CerrarTicketAsync(IdTicket, Notas,IdMotivo, IdSubMotivo);
            _dispatcher.Stop();
            EstaHabilitadoNotas = false;
            EsVisibleAceptarTicket = Visibility.Collapsed;
            EsVisibleCerrarTicket = Visibility.Collapsed;
            AlturaNotas = 160;
            ObtenerTickets();
            EsVisibleAgregarTransaccionManual = Visibility.Collapsed;
            EsVisibleCambiarEstado = Visibility.Collapsed;
        }
        #endregion
    }
    #endregion

    #region Dispositivos
    internal partial class VentanaPrincipalViewModel
    {
        #region Variables
        private List<Dispositivo> _dispositivos;
        private Visibility _esVisibleDispositivos;
        private SolidColorBrush _colorBotonAntena;
        private SolidColorBrush _colorBotonBarrera;
        private SolidColorBrush _colorBotonImpresora;
        #endregion

        #region Constructor
        private void InicializarVariablesDispositivos()
        {
            EsVisibleDispositivos = Visibility.Collapsed;
        }
        #endregion

        #region Propiedades
        public Visibility EsVisibleDispositivos
        {
            get
            {
                return _esVisibleDispositivos;
            }
            set
            {
                if (_esVisibleDispositivos == value)
                    return;
                _esVisibleDispositivos = value;
                RaisePropertyChanged("EsVisibleDispositivos");
            }
        }
        public SolidColorBrush ColorBotonAntena
        {
            get
            {
                return _colorBotonAntena;
            }
            set
            {
                if (_colorBotonAntena == value)
                    return;
                _colorBotonAntena = value;
                RaisePropertyChanged("ColorBotonAntena");
            }
        }

        public SolidColorBrush ColorBotonBarrera
        {
            get
            {
                return _colorBotonBarrera;
            }
            set
            {
                if (_colorBotonBarrera == value)
                    return;
                _colorBotonBarrera = value;
                RaisePropertyChanged("ColorBotonBarrera");
            }
        }

        public SolidColorBrush ColorBotonImpresora
        {
            get
            {
                return _colorBotonImpresora;
            }
            set
            {
                if (_colorBotonImpresora == value)
                    return;
                _colorBotonImpresora = value;
                RaisePropertyChanged("ColorBotonImpresora");
            }
        }
        #endregion

        #region Metodos
        private void ObtenerDispositivos()
        {
            _dispositivos = new List<Dispositivo>();
            if (TicketSeleccionado.Tipo == TipoTicket.Proceso && TicketSeleccionado.FechaAceptacion.HasValue && !TicketSeleccionado.FechaFinalizacion.HasValue)
                ObtenerDispositivosEnLinea();
            else
                ObtenerDispositivosFueraLinea();
        }

        private void ObtenerDispositivosEnLinea()
        {
            EsVisibleDispositivos = Visibility.Visible;
            InicializarServicioQuiosco();
            EstadoAntena();
            EstadoBarrera();
            EstadoImpresora();
        }

        private void InicializarServicioQuiosco()
        {
            var binding = new BasicHttpBinding();
            var endpoint = $"http://{TicketSeleccionado.IpQuiosco}:17101/ServicioTransactionQuiosco/";
            _servicioQuiosco = new ServicioTransactionQuiosco.ContratoClient(binding, new EndpointAddress(endpoint));
        }

        private async void EstadoAntena()
        {
            var estadoAntena = await _servicioQuiosco.EstadoAntenaAsync();
            _dispositivos.Add(
                new Dispositivo
                {
                    NombreDispositivo = "Antena RFID",
                    Estado = estadoAntena.Item1,
                    Mensaje = estadoAntena.Item2
                });
            ColorBotonAntena = estadoAntena.Item1 ? new SolidColorBrush(Colors.LightGreen) : new SolidColorBrush(Colors.Red);
        }

        private async void EstadoBarrera()
        {
            var estadoBarrera = await _servicioQuiosco.EstadoBarreraAsync();
            _dispositivos.Add(
                new Dispositivo
                {
                    NombreDispositivo = "Barrera (Usb Relay)",
                    Estado = estadoBarrera.Item1,
                    Mensaje = estadoBarrera.Item2
                });
            ColorBotonBarrera = estadoBarrera.Item1 ? new SolidColorBrush(Colors.LightGreen) : new SolidColorBrush(Colors.Red);
        }

        private async void EstadoImpresora()
        {
            var estadoImpresora = await _servicioQuiosco.EstadoImpresoraAsync();
            var mensajesErrores = estadoImpresora.Item1.Aggregate("", (actual, item) => (actual == "" ? "" : actual + ",") + item);
            var mensajesAdvertencias = estadoImpresora.Item2.Aggregate("", (actual, item) => (actual == "" ? "" : actual + ",") + item);
            var mensaje = "";
            if (mensajesErrores == "" && mensajesAdvertencias == "")
            {
                mensaje = "Impresora Ok.";
                ColorBotonImpresora = new SolidColorBrush(Colors.LightGreen);
            }
            else if(mensajesErrores == "" && mensajesAdvertencias != "")
            {
                mensaje = $"Advertencias : {mensajesAdvertencias}";
                ColorBotonImpresora = new SolidColorBrush(Colors.LightYellow);
            }
            else
            {
                mensaje = $"Errores: {mensajesErrores}///Advertencias : {mensajesAdvertencias}";
                ColorBotonImpresora = new SolidColorBrush(Colors.Red);
            }
            _dispositivos.Add(
                new Dispositivo
                {
                    NombreDispositivo = "Impresora",
                    Estado = estadoImpresora.Item1.Count() == 0,
                    Mensaje = mensaje
                });
        }

        private void ObtenerDispositivosFueraLinea()
        {
            EsVisibleDispositivos = Visibility.Collapsed;
            ColorBotonAntena = new SolidColorBrush(Colors.Gray);
            ColorBotonBarrera = new SolidColorBrush(Colors.Gray);
            ColorBotonImpresora = new SolidColorBrush(Colors.Gray);
        }

        public async void MensajeAntena()
        {
            var mensajeDialogo = new MessageDialog(_dispositivos.FirstOrDefault(d => d.NombreDispositivo == "Antena RFID").Mensaje, "Estado Antena RFID");
            await mensajeDialogo.ShowAsync();
        }

        public async void MensajeBarrera()
        {
            var mensajeDialogo = new MessageDialog(_dispositivos.FirstOrDefault(d => d.NombreDispositivo == "Barrera (Usb Relay)").Mensaje, "Estado Barrera (Usb Relay)");
            await mensajeDialogo.ShowAsync();
        }

        public async void MensajesImpresora()
        {
            var mensajeDialogo = new MessageDialog(_dispositivos.FirstOrDefault(d => d.NombreDispositivo == "Impresora").Mensaje, "Estado Impresora");
            await mensajeDialogo.ShowAsync();
        }
        #endregion
    }
    #endregion

    #region Botones
    internal partial class VentanaPrincipalViewModel
    {
        #region Variables
        private RelayCommand _comandoAutoTicket;
        private RelayCommand _comandoSuspenderTicket;
        private RelayCommand _comandoActualizar;
        private RelayCommand _comandoDescanso;
        private RelayCommand _comandoCerrarSesion;
        private RelayCommand _comandoAbrirBarrera;
        private RelayCommand _comandoIrHome;
        private RelayCommand _comandoReimprimirTicket;
        #endregion

        #region Constructor
        private void InstanciarComandos()
        {
            _comandoAutoTicket = new RelayCommand(AutoTicket);
            _comandoSuspenderTicket = new RelayCommand(SuspenderTicket, PuedoSuspenderTicket);
            _comandoActualizar = new RelayCommand(Actualizar);
            _comandoDescanso = new RelayCommand(Descanso);
            _comandoCerrarSesion = new RelayCommand(CerrarSesion, PuedoCerrarSesion);
            _comandoAbrirBarrera = new RelayCommand(AbrirBarrera, PuedoEjecutarAccion);
            _comandoIrHome = new RelayCommand(IrHome, PuedoEjecutarAccion);
            _comandoReimprimirTicket = new RelayCommand(ReimprimirTicket, PuedoEjecutarAccion);
        }
        #endregion

        #region Propiedades
        public RelayCommand ComandoAutoTicket
        {
            get
            {
                return _comandoAutoTicket;
            }
            set
            {
                SetProperty(ref _comandoAutoTicket, value);
            }
        }

        public RelayCommand ComandoSuspenderTicket
        {
            get
            {
                return _comandoSuspenderTicket;
            }
            set
            {
                SetProperty(ref _comandoSuspenderTicket, value);
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

        public RelayCommand ComandoDescanso
        {
            get
            {
                return _comandoDescanso;
            }
            set
            {
                SetProperty(ref _comandoDescanso, value);
            }
        }

        public RelayCommand ComandoCerrarSesion
        {
            get
            {
                return _comandoCerrarSesion;
            }
            set
            {
                SetProperty(ref _comandoCerrarSesion, value);
            }
        }

        public RelayCommand ComandoAbrirBarrera
        {
            get
            {
                return _comandoAbrirBarrera;
            }
            set
            {
                SetProperty(ref _comandoAbrirBarrera, value);
            }
        }

        public RelayCommand ComandoIrHome
        {
            get
            {
                return _comandoIrHome;
            }
            set
            {
                SetProperty(ref _comandoIrHome, value);
            }
        }

        public RelayCommand ComandoReimprimirTicket
        {
            get
            {
                return _comandoReimprimirTicket;
            }
            set
            {
                SetProperty(ref _comandoReimprimirTicket, value);
            }
        }
        #endregion

        #region Metodos
        private async void AutoTicket(object obj)
        {
            var ventana = new VentanaMotivosAutoTicket { ViewModel = new VentanaMotivosAutoTicketViewModel(_servicio) };
            await ventana.ShowAsync();
            ObtenerTickets();
        }

        private bool PuedoSuspenderTicket(object obj)
        {
            return TicketSeleccionado != null && TicketSeleccionado.Tipo == TipoTicket.Proceso && !TicketSeleccionado.FechaFinalizacion.HasValue;
        }

        private async void SuspenderTicket(object obj)
        {
            var mensajeDialogo = new MessageDialog("¿Está seguro de suspender el ticket?", "Suspensión de Ticket.");
            mensajeDialogo.Commands.Add(new UICommand("Sí", new UICommandInvokedHandler(RealizarAccionSuspender)));
            mensajeDialogo.Commands.Add(new UICommand("No"));
            mensajeDialogo.DefaultCommandIndex = 1;
            mensajeDialogo.CancelCommandIndex = 1;
            await mensajeDialogo.ShowAsync();
        }

        private async void RealizarAccionSuspender(IUICommand command)
        {
            await _servicio.SuspenderTicketAsync(IdTicket);
            ObtenerTickets();
        }

        private void Actualizar(object obj)
        {
            ObtenerTickets();
        }

        private async void Descanso(object obj)
        {
            var viewModel = new VentanaTiposDescansosViewModel(_servicio);
            var ventana = new VentanaTiposDescansos { ViewModel = viewModel };
            await ventana.ShowAsync();
            if (viewModel.IdDescanso > 0)
                _ventanaPrincipal.Frame.Navigate(typeof(VentanaDescanso), viewModel.IdDescanso);
        }

        private bool PuedoCerrarSesion(object obj)
        {
            return Tickets == null || Tickets.All(t => t.FechaFinalizacion.HasValue);
        }

        private async void CerrarSesion(object obj)
        {
            await _servicio.CerrarSesionAsync(((DatosLogin)App.Current.Resources["DatosLogin"]).IdSesion);
            _ventanaPrincipal.Frame.Navigate(typeof(VentanaAutenticacion));
            App.Current.Resources.Remove("DatosLogin");
        }

        private bool PuedoEjecutarAccion(object obj)
        {
            return TicketSeleccionado != null && TicketSeleccionado.Tipo == TipoTicket.Proceso && TicketSeleccionado.FechaAceptacion.HasValue && !TicketSeleccionado.FechaFinalizacion.HasValue;
        }

        private async void AbrirBarrera(object obj)
        {
            var resultado = await _servicioQuiosco.AbrirBarreraTroubleDeskAsync();
            await _servicio.RegistrarAccionAsync(new ACTION { TT_ID = TicketSeleccionado.IdTicket, ACTION_NAME = "ABRIR_BARRERA"});
            if (!resultado.Item1)
            {
                var mensajeDialogo = new MessageDialog(resultado.Item2, "Abrir Barrera.");
                await mensajeDialogo.ShowAsync();
            }
        }

        private async void IrHome(object obj)
        {
            var resultado = await _servicioQuiosco.IrHomeQuioscoAsync();
            await _servicio.RegistrarAccionAsync(new ACTION { TT_ID = TicketSeleccionado.IdTicket, ACTION_NAME = "IR_HOME" });
            if (!resultado.Item1)
            {
                var mensajeDialogo = new MessageDialog(resultado.Item2, "IR AL HOME.");
                await mensajeDialogo.ShowAsync();
            }
        }

        private async void ReimprimirTicket(object obj)
        {
            var resultado = await _servicioQuiosco.ReimprimirTicketAsync();
            await _servicio.RegistrarAccionAsync(new ACTION { TT_ID = TicketSeleccionado.IdTicket, ACTION_NAME = "REIMPRIMIR" });
            var mensajeDialogo = new MessageDialog(resultado.Item2, "Reimpresión Ticket.");
            await mensajeDialogo.ShowAsync();
        }
        #endregion
    }
    #endregion

    #region Aduana
    internal partial class VentanaPrincipalViewModel
    {
        #region Variables
        private RelayCommand _comandoCambiarEstado;
        private RelayCommand _comandoAgregarTransaccionManual;
        private Visibility _esVisibleCambiarEstado;
        private Visibility _esVisibleAgregarTransaccionManual;
        private ObservableCollection<TransaccionContenedor> _transaccionContenedores;
        private TransaccionContenedor _transaccionContenedorSeleccionada;
        private ObservableCollection<mb_get_ecuapass_message_pass_Result> _mensajesSmdt;
        #endregion

        #region Constructor
        private void InstanciarComandosAduana()
        {
            _comandoCambiarEstado = new RelayCommand(CambiarEstado, PuedoCambiarEstado);
            _comandoAgregarTransaccionManual = new RelayCommand(AgregarTransaccionManual);
            EsVisibleCambiarEstado = Visibility.Collapsed;
            EsVisibleAgregarTransaccionManual = Visibility.Collapsed;
        }
        #endregion

        #region Propiedades
        public RelayCommand ComandoCambiarEstado
        {
            get
            {
                return _comandoCambiarEstado;
            }
            set
            {
                SetProperty(ref _comandoCambiarEstado, value);
            }
        }

        public RelayCommand ComandoAgregarTransaccionManual
        {
            get
            {
                return _comandoAgregarTransaccionManual;
            }
            set
            {
                SetProperty(ref _comandoAgregarTransaccionManual, value);
            }
        }

        public Visibility EsVisibleCambiarEstado
        {
            get
            {
                return _esVisibleCambiarEstado;
            }
            set
            {
                if (_esVisibleCambiarEstado == value)
                    return;
                _esVisibleCambiarEstado = value;
                RaisePropertyChanged("EsVisibleCambiarEstado");
            }
        }

        public Visibility EsVisibleAgregarTransaccionManual
        {
            get
            {
                return _esVisibleAgregarTransaccionManual;
            }
            set
            {
                if (_esVisibleAgregarTransaccionManual == value)
                    return;
                _esVisibleAgregarTransaccionManual = value;
                RaisePropertyChanged("EsVisibleAgregarTransaccionManual");
            }
        }

        public ObservableCollection<TransaccionContenedor> TransaccionContenedores
        {
            get
            {
                return _transaccionContenedores;
            }
            set
            {
                SetProperty(ref _transaccionContenedores, value);
            }
        }

        public TransaccionContenedor TransaccionContenedorSeleccionada
        {
            get
            {
                return _transaccionContenedorSeleccionada;
            }
            set
            {
                SetProperty(ref _transaccionContenedorSeleccionada, value);
                if (_transaccionContenedorSeleccionada != null)
                    ObtenerMensajesAduana();
            }
        }

        public ObservableCollection<mb_get_ecuapass_message_pass_Result> MensajesSmdt
        {
            get
            {
                return _mensajesSmdt;
            }
            set
            {
                SetProperty(ref _mensajesSmdt, value);
            }
        }
        #endregion

        #region Metodos
        private void ObtenerTransaccionContenedores()
        {
            if (TicketSeleccionado.Tipo == TipoTicket.Proceso && _detallesTicket != null && TicketSeleccionado.FechaAceptacion.HasValue)
                TransaccionContenedores = _detallesTicket.TransaccionContenedores;
        }

        private async void ObtenerMensajesAduana()
        {
            EstaHabilitado = false;
            EstaOcupado = true;
            if (TicketSeleccionado.FechaAceptacion.HasValue && !TicketSeleccionado.FechaFinalizacion.HasValue)
            {
                EsVisibleAgregarTransaccionManual = Visibility.Visible;
                EsVisibleCambiarEstado = Visibility.Visible;
            }
            MensajesSmdt = await _servicio.ObtenerMensajesSmdtAduanaAsync(TransaccionContenedorSeleccionada.NumeroTransaccion);
            EstaOcupado = false;
            EstaHabilitado = true;
        }

        private bool PuedoCambiarEstado(object obj)
        {
            return MensajesSmdt != null && MensajesSmdt.Count() > 0;
        }

        private async void CambiarEstado(object obj)
        {
            var resultado = await _servicio.CambiarEstadoSmdtAsync(MensajesSmdt.FirstOrDefault().CODIGO_TRANSACCION, ((DatosLogin)App.Current.Resources["DatosLogin"]).Usuario);
            MessageDialog mensajeDialogo;
            if (resultado.HasValue && resultado.Value == 2)
                mensajeDialogo = new MessageDialog("Proceso Ok.", "Cambio Estado SMDT");
            else
                mensajeDialogo = new MessageDialog($"No se actualizó el estado.{Environment.NewLine}Contactese con IT.", "Cambio Estado SMDT");
            await mensajeDialogo.ShowAsync();
        }

        private async void AgregarTransaccionManual(object obj)
        {
            var ventana = new VentanaTransaccionManual { ViewModel = new VentanaTransaccionManualViewModel(_servicio, new DatosTransaccionManual { Contenedor = TransaccionContenedorSeleccionada.Contenedor, ObjetoSolicita = "webservice", UsuarioSolicita = ((DatosLogin)App.Current.Resources["DatosLogin"]).Usuario, GKeyUnidad = Convert.ToInt64(TransaccionContenedorSeleccionada.NumeroTransaccion), TipoCarga = TransaccionContenedorSeleccionada.Contenedor == "" ? "BRBK" : "CNTR" }) };
            await ventana.ShowAsync();
        }
        #endregion
    }
    #endregion
}

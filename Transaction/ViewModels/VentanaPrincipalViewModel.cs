using RECEPTIO.CapaPresentacion.UI.ImpresoraZebra;
using RECEPTIO.CapaPresentacion.UI.Interfaces.Impresora;
using RECEPTIO.CapaPresentacion.UI.Interfaces.RFID;
using RECEPTIO.CapaPresentacion.UI.Interfaces.UsbRelay;
using RECEPTIO.CapaPresentacion.UI.MVVM;
using Spring.Context.Support;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Transaction.ServicioTransaction;
using KIOSK = Transaction.ServicioTransaction.KIOSK;
using MESSAGE = Transaction.ServicioTransaction.MESSAGE;
using DEPOT = Transaction.ServicioTransaction.DEPOT;

namespace Transaction.ViewModels
{
    public partial class VentanaPrincipalViewModel : ViewModelBase, IDisposable
    {
        #region Campos
        internal readonly Frame Contenedor;
        internal readonly ServicioTransactionClient Servicio;
        internal readonly ServicioAnuncianteProblema.ServicioAnuncianteProblemaClient ServicioAnuncianteProblema;
        internal IEnumerable<MESSAGE> Mensajes;
        internal KIOSK Quiosco;
        internal DEPOT Depot;
        internal IAntena Antena;
        internal IBarrera Barrera;
        internal Tuple<List<string>, List<string>> MensajesEstadoImpresora;
        internal BrushConverter Convertidor;
        private EstadoProceso _estado;
        private bool _estaOcupado;
        private string _mensajeBusy;
        private Brush _inicioBackground;
        private Brush _huellaBackground;
        private Brush _rfidBackground;
        private Brush _procesoBackground;
        private Brush _ticketBackground;
        private Brush _barreraBackground;
        private string _rutaImagenInicio;
        private string _rutaImagenHuella;
        private string _rutaImagenRfid;
        private string _rutaImagenProceso;
        private string _rutaImagenTicket;
        private string _rutaImagenBarrera;
        private string _fecha;
        private bool _disposed;
        private DispatcherTimer _dispatcherInputs = new DispatcherTimer { Interval = new TimeSpan(0, 0, 1), IsEnabled = true };
        private bool _estaLeyendoTag;
        private BitArray _inputs = new BitArray(16);
        private bool _input1;
        private bool _input2;
        private bool _input3;
        private bool _input4;
        private string _listaTags;
        private string _tagReal;
        private Visibility _esVisibleSensores;
        internal bool PuedoIrHome;
        internal bool LeerSinLoop;
        internal bool VentanaAutorizacionDisponible;
        private string _tipoTransaccion;
        #endregion

        #region Constructor
        public VentanaPrincipalViewModel(Frame contenedor)
        {
            Contenedor = contenedor;
            Servicio = new ServicioTransactionClient();
            ServicioAnuncianteProblema = new ServicioAnuncianteProblema.ServicioAnuncianteProblemaClient();
            Convertidor = new BrushConverter();
            ObtenerQuiosco();
            ObtenerMensajes();
            ConectarAntena();
            ConectarUsbRelay();
            if (ConfigurationManager.AppSettings["ValidarImpresora"] == "1")
            {
                VerEstadoImpresora();
            }
            HostearServicio();
            IrHomeQuiosco();
            _dispatcherInputs.Tick += LeerInfinitamenteInputs;
            _dispatcherInputs.Start();
            _dispatcher.Stop();
            _dispatcher.Tick += Temporizador;
            _dispatcherImpresora.Stop();
            _dispatcherImpresora.Tick += TemporizadorImpresion;
            EsVisibleSensores = Convert.ToInt32(ConfigurationManager.AppSettings["EsVisibleSensores"]) == 0 ? Visibility.Collapsed : Visibility.Visible;
        }
        #endregion

        #region Propiedades
        internal DatosPreGate DatosPreGate { get; set; }

        internal DatosPreGateSalida DatosPreGateSalida { get; set; }

        internal string DatoHuella { get; set; }

        internal DatosN4 DatosN4 { get; set; }

        public bool EstaOcupado
        {
            get
            {
                return _estaOcupado;
            }
            set
            {
                if (_estaOcupado == value) return;
                _estaOcupado = value;
                OnPropertyChanged("EstaOcupado");
            }
        }

        public string MensajeBusy
        {
            get
            {
                return _mensajeBusy;
            }
            set
            {
                if (_mensajeBusy == value) return;
                _mensajeBusy = value;
                OnPropertyChanged("MensajeBusy");
            }
        }

        public Brush InicioBackground
        {
            get
            {
                return _inicioBackground;
            }
            set
            {
                if (Equals(_inicioBackground, value)) return;
                _inicioBackground = value;
                OnPropertyChanged("InicioBackground");
            }
        }

        public Brush HuellaBackground
        {
            get
            {
                return _huellaBackground;
            }
            set
            {
                if (Equals(_huellaBackground, value)) return;
                _huellaBackground = value;
                OnPropertyChanged("HuellaBackground");
            }
        }

        public Brush RfidBackground
        {
            get
            {
                return _rfidBackground;
            }
            set
            {
                if (Equals(_rfidBackground, value)) return;
                _rfidBackground = value;
                OnPropertyChanged("RfidBackground");
            }
        }

        public Brush ProcesoBackground
        {
            get
            {
                return _procesoBackground;
            }
            set
            {
                if (Equals(_procesoBackground, value)) return;
                _procesoBackground = value;
                OnPropertyChanged("ProcesoBackground");
            }
        }

        public Brush TicketBackground
        {
            get
            {
                return _ticketBackground;
            }
            set
            {
                if (Equals(_ticketBackground, value)) return;
                _ticketBackground = value;
                OnPropertyChanged("TicketBackground");
            }
        }

        public Brush BarreraBackground
        {
            get
            {
                return _barreraBackground;
            }
            set
            {
                if (Equals(_barreraBackground, value)) return;
                _barreraBackground = value;
                OnPropertyChanged("BarreraBackground");
            }
        }

        public string RutaImagenInicio
        {
            get
            {
                return _rutaImagenInicio;
            }
            set
            {
                if (_rutaImagenInicio == value) return;
                _rutaImagenInicio = value;
                OnPropertyChanged("RutaImagenInicio");
            }
        }

        public string RutaImagenHuella
        {
            get
            {
                return _rutaImagenHuella;
            }
            set
            {
                if (_rutaImagenHuella == value) return;
                _rutaImagenHuella = value;
                OnPropertyChanged("RutaImagenHuella");
            }
        }

        public string RutaImagenRfid
        {
            get
            {
                return _rutaImagenRfid;
            }
            set
            {
                if (_rutaImagenRfid == value) return;
                _rutaImagenRfid = value;
                OnPropertyChanged("RutaImagenRfid");
            }
        }

        public string RutaImagenProceso
        {
            get
            {
                return _rutaImagenProceso;
            }
            set
            {
                if (_rutaImagenProceso == value) return;
                _rutaImagenProceso = value;
                OnPropertyChanged("RutaImagenProceso");
            }
        }

        public string RutaImagenTicket
        {
            get
            {
                return _rutaImagenTicket;
            }
            set
            {
                if (_rutaImagenTicket == value) return;
                _rutaImagenTicket = value;
                OnPropertyChanged("RutaImagenTicket");
            }
        }

        public string RutaImagenBarrera
        {
            get
            {
                return _rutaImagenBarrera;
            }
            set
            {
                if (_rutaImagenBarrera == value) return;
                _rutaImagenBarrera = value;
                OnPropertyChanged("RutaImagenBarrera");
            }
        }

        public string Fecha
        {
            get
            {
                return _fecha;
            }
            set
            {
                if (_fecha == value) return;
                _fecha = value;
                OnPropertyChanged("Fecha");
            }
        }

        public string NumeroKiosco
        {
            get
            {
                return Quiosco == null ? "" : Quiosco.NAME.Split(' ')[1];
            }
        }

        public string ObtenerDeposito(int id)
        {
            string v_result = string.Empty;
            Depot = Servicio.ObtenerDepot(id);
            if (Depot == null)
            {
                v_result = string.Empty;
            }
            else
            {
                v_result = Depot.NAME;
            }

            return v_result;
        }

        public bool Input1
        {
            get
            {
                return _input1;
            }
            set
            {
                if (_input1 == value) return;
                _input1 = value;
                OnPropertyChanged("Input1");
            }
        }

        public bool Input2
        {
            get
            {
                return _input2;
            }
            set
            {
                if (_input2 == value) return;
                _input2 = value;
                OnPropertyChanged("Input2");
            }
        }

        public bool Input3
        {
            get
            {
                return _input3;
            }
            set
            {
                if (_input3 == value) return;
                _input3 = value;
                OnPropertyChanged("Input3");
            }
        }

        public bool Input4
        {
            get
            {
                return _input4;
            }
            set
            {
                if (_input4 == value) return;
                _input4 = value;
                OnPropertyChanged("Input4");
            }
        }

        public string ListaTags
        {
            get
            {
                return _listaTags;
            }
            set
            {
                if (_listaTags == value) return;
                _listaTags = value;
                OnPropertyChanged("ListaTags");
            }
        }

        public string TagReal
        {
            get
            {
                return _tagReal;
            }
            set
            {
                if (_tagReal == value) return;
                _tagReal = value;
                OnPropertyChanged("TagReal");
            }
        }

        public Visibility EsVisibleSensores
        {
            get
            {
                return _esVisibleSensores;
            }
            set
            {
                if (_esVisibleSensores == value) return;
                _esVisibleSensores = value;
                OnPropertyChanged("EsVisibleSensores");
            }
        }

        public string TipoTransaccion
        {
            get
            {
                return _tipoTransaccion;
            }
            set
            {
                if (_tipoTransaccion == value) return;
                _tipoTransaccion = value;
                OnPropertyChanged("TipoTransaccion");
            }
        }
        #endregion

        #region Metodos
        private void ObtenerMensajes()
        {
            Mensajes = Servicio.ObtenerMensajesErrores();
            if (Mensajes.Count() == 0)
                CerrarAplicacion("No existe catálogo de Mensajes.");
        }

        private void ObtenerQuiosco()
        {
            var ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
            Quiosco = Servicio.ObtenerQuiosco(ip);
            if (Quiosco == null)
                CerrarAplicacion($"No existe un quiosco registrado para esta ip : {ip}.");
            if (!Quiosco.IS_ACTIVE)
                CerrarAplicacion("El quiosco no está activo.");
            App.Current.Resources.Add("Quiosco", Quiosco);
            OnPropertyChanged("NumeroKiosco");
        }

        private void CerrarAplicacion(string mensaje)
        {
            MessageBox.Show($@"{mensaje}{Environment.NewLine}La aplicación se cerrará.",
                "TRANSACTION", MessageBoxButton.OK, MessageBoxImage.Error);
            Environment.Exit(0);
        }

        private void ConectarAntena()
        {
            var ctx = new XmlApplicationContext("~/Springs/SpringAntena.xml");
            Antena = (IAntena)ctx["AdministradorAntena"];
            if (!Antena.ConectarAntena())
                CerrarAplicacion("Antena Rfid no conectada.");
        }

        private void ConectarUsbRelay()
        {
            var ctx = new XmlApplicationContext("~/Springs/SpringBarrera.xml");
            Barrera = (IBarrera)ctx["AdministradorBarrera"];
            if (!Barrera.Conectar())
                CerrarAplicacion("UsbRelay no conectado.");
        }

        private void VerEstadoImpresora(bool cerrarAplicacion = true)
        {
            IEstadoImpresora estadoImpresora = new EstadoImpresora();
            MensajesEstadoImpresora = estadoImpresora.VerEstado();
            if (MensajesEstadoImpresora.Item1.Count != 0)
            {
                var mensajesErrores = MensajesEstadoImpresora.Item1.Aggregate("", (actual, item) => (actual == "" ? "" : actual + "\n") + item);
                var mensajesAdvertencias = MensajesEstadoImpresora.Item2.Aggregate("", (actual, item) => (actual == "" ? "" : actual + "\n") + item);
                if (cerrarAplicacion)
                    CerrarAplicacion($"La impresora tiene errores que se detallan a continuación:{Environment.NewLine}Errores: {mensajesErrores}{Environment.NewLine}Advertencias: {mensajesAdvertencias}");
                else
                    MessageBox.Show($"La impresora tiene errores que se detallan a continuación:{Environment.NewLine}Errores: {mensajesErrores}{Environment.NewLine}Advertencias: {mensajesAdvertencias}", "TRANSACTION", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LeerInfinitamenteInputs(object sender, EventArgs e)
        {
            Fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            _inputs = Barrera.ObtenerInputs();
            if ((_inputs[0] || LeerSinLoop) && !_estaLeyendoTag)
            {
                Antena.IniciarLectura();
                _estaLeyendoTag = true;
            }
            Input1 = _inputs[0];
            Input2 = _inputs[1];
            Input3 = _inputs[2];
            Input4 = _inputs[3];
            if (Antena.ObtenerTagsLeidos() != null && _estaLeyendoTag)
                ListaTags = $"Tags leídos : {Antena.ObtenerTagsLeidos().Aggregate("", (actual, item) => (actual == "" ? "" : actual + ",") + item)}.";
            if (!_inputs[0] && !LeerSinLoop && _estaLeyendoTag)
            {
                Antena.TerminarLectura();
                _estaLeyendoTag = false;
                ListaTags = "";
            }
        }

        internal void SetearEstado(EstadoProceso estado)
        {
            _estado = estado;
            _estado.EstablecerControles(this);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                Servicio.Close();
                ServicioAnuncianteProblema.Close();
                Antena.DesconectarAntena();
                Antena.Dispose();
                Barrera.Dispose();
                if (_servicio != null && (_servicio.State == CommunicationState.Created || _servicio.State == CommunicationState.Opened || _servicio.State == CommunicationState.Opening))
                    _servicio.Close();
            }
            _disposed = true;
        }
        #endregion
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    public partial class VentanaPrincipalViewModel : IContrato
    {
        private ServiceHost _servicio;
        private DispatcherTimer _dispatcher = new DispatcherTimer { Interval = new TimeSpan(0, 0, 2), IsEnabled = true };
        private DispatcherTimer _dispatcherImpresora = new DispatcherTimer { Interval = new TimeSpan(0, 0, 4), IsEnabled = true };
        internal string PasoActual;

        private void HostearServicio()
        {
            try
            {
                _servicio = new ServiceHost(this);
                _servicio.Open();
            }
            catch (Exception ex)
            {
                CerrarAplicacion(ex.Message);
            }
        }

        public Tuple<List<string>, List<string>> EstadoImpresora()
        {
            IEstadoImpresora estadoImpresora = new EstadoImpresora();
            return estadoImpresora.VerEstado();
        }

        public void AbrirBarreraLider()
        {
            Barrera.LevantarBarrera();
            _dispatcher.Start();
        }

        public Tuple<bool, string> AbrirBarreraTroubleDesk()
        {
            if (PasoActual == "IMPRESION" || PasoActual == "BARRERA")
            {
                if (EstaOcupado)
                    return new Tuple<bool, string>(false, $"No se puede levantar la barrera porque el kiosco está realizando un proceso.{Environment.NewLine}Vuelva a intentarlo en unos segundos.");
                else
                {
                    Barrera.LevantarBarrera();
                    _dispatcher.Start();
                    return new Tuple<bool, string>(true, "");
                }
            }
            else
                return new Tuple<bool, string>(false, $"No se puede levantar la barrera en el paso {PasoActual} del kiosco.");
        }

        private void Temporizador(object sender, EventArgs e)
        {
            _dispatcher.Stop();
            Barrera.BajarBarrera();
        }

        public Tuple<bool, string> IrHomeQuiosco()
        {
            if (PasoActual == "PRE_GATE")
                return new Tuple<bool, string>(false, $"No se puede ir al home del kiosco, porque ya está en él.");
            else
            {
                if (EstaOcupado)
                    return new Tuple<bool, string>(false, $"No se puede ir al home porque el kiosco está realizando un proceso.{Environment.NewLine}Vuelva a intentarlo en unos segundos.");
                else
                {
                    if (Quiosco.IS_IN)
                        SetearEstado(new PaginaInicioViewModel());
                    else
                        SetearEstado(new PaginaSalidaInicioViewModel());
                    return new Tuple<bool, string>(true, "");
                }
            }
        }

        public Tuple<bool, string> ReimprimirTicket()
        {
            if (PasoActual != "IMPRESION")
                return new Tuple<bool, string>(false, $"No se puede imprimir ticket en el paso {PasoActual} del kiosco.");
            else if (EstaOcupado)
                return new Tuple<bool, string>(false, $"No se puede imprimir ticket porque el kiosco está realizando un proceso.{Environment.NewLine}Vuelva a intentarlo en unos segundos.");
            if (DatosN4 == null)
                return new Tuple<bool, string>(false, "No hay información para imprimir el ticket.");
            else
            {
                Imprimir();
                return new Tuple<bool, string>(true, "Se envió a reimprimir el ticket, verifique si hubo errores en la pantalla del kiosco, o de lo contrario verifique si el ticket se imprimió.");
            }
        }

        private void Imprimir()
        {
            if (Quiosco.IS_IN)
            {
                var imprimirTicketEntrada = new ImprimirEntradaDeliveryImportFull();
                imprimirTicketEntrada.Procesar(this);
                _dispatcherImpresora.Start();
            }
            else
            {
                var imprimirTicketSalida = new ImprimirSalidaDeliveryImportFull();
                imprimirTicketSalida.Procesar(this);
                _dispatcherImpresora.Start();
            }
        }

        private void TemporizadorImpresion(object sender, EventArgs e)
        {
            _dispatcherImpresora.Stop();
            VerEstadoImpresora(false);
        }

        public Tuple<bool, string> ReimprimirCualquierTicket(RECEPTIO.CapaDominio.Nucleo.Entidades.PRE_GATE preGate, string xml)
        {
            DatosN4 = new DatosN4 { Xml = xml };
            DatoHuella = $"{preGate.DRIVER_ID}:{preGate.DRIVER_ID}:{preGate.DRIVER_ID}";
            if (Quiosco.IS_IN)
                DatosPreGate = new DatosPreGate { PreGate = ConversionPreGate(preGate) };
            else
                DatosPreGateSalida = new DatosPreGateSalida { PreGate = ConversionPreGate(preGate) };
            if (EstaOcupado)
                return new Tuple<bool, string>(false, $"No se puede imprimir ticket porque el kiosco está realizando un proceso.{Environment.NewLine}Vuelva a intentarlo en unos segundos.");
            else
            {
                Imprimir();
                return new Tuple<bool, string>(true, "Se envió a reimprimir el ticket, verifique si hubo errores en la pantalla del kiosco, o de lo contrario verifique si el ticket se imprimió.");
            }
        }

        private PRE_GATE ConversionPreGate(RECEPTIO.CapaDominio.Nucleo.Entidades.PRE_GATE preGate)
        {
            var resultado = new PRE_GATE
            {
                PRE_GATE_ID = preGate.PRE_GATE_ID,
                CREATION_DATE = preGate.CREATION_DATE,
                DEVICE_ID = preGate.DEVICE_ID,
                DRIVER_ID = preGate.DRIVER_ID,
                STATUS = preGate.STATUS,
                TRUCK_LICENCE = preGate.TRUCK_LICENCE,
                USER = preGate.USER,
                KIOSK_TRANSACTIONS = new List<KIOSK_TRANSACTION>(),
                PRE_GATE_DETAILS = new List<PRE_GATE_DETAIL>()
            };
            foreach (var item in preGate.PRE_GATE_DETAILS)
            {
                var detalle = new PRE_GATE_DETAIL
                {
                    DOCUMENT_ID = item.DOCUMENT_ID,
                    PRE_GATE_ID = item.PRE_GATE_ID,
                    REFERENCE_ID = item.REFERENCE_ID,
                    STATUS = item.STATUS,
                    TRANSACTION_NUMBER = item.TRANSACTION_NUMBER,
                    TRANSACTION_TYPE_ID = item.TRANSACTION_TYPE_ID,
                    ID = item.ID
                };
                detalle.CONTAINERS = new List<CONTAINER>();
                foreach (var cont in item.CONTAINERS)
                    detalle.CONTAINERS.Add(new CONTAINER { NUMBER = cont.NUMBER });
                resultado.PRE_GATE_DETAILS.Add(detalle);
            }
            foreach (var item in preGate.KIOSK_TRANSACTIONS)
            {
                resultado.KIOSK_TRANSACTIONS.Add(new KIOSK_TRANSACTION
                {
                    END_DATE = item.END_DATE,
                    IS_OK = item.IS_OK,
                    KIOSK = new KIOSK
                    {
                        IP = item.KIOSK.IP,
                        IS_ACTIVE = item.KIOSK.IS_ACTIVE,
                        IS_IN = item.KIOSK.IS_IN,
                        KIOSK_ID = item.KIOSK.KIOSK_ID,
                        NAME = item.KIOSK.NAME,
                        ZONE_ID = item.KIOSK.ZONE_ID
                    },
                    KIOSK_ID = item.KIOSK_ID,
                    PRE_GATE_ID = item.PRE_GATE_ID,
                    START_DATE = item.START_DATE,
                    TRANSACTION_ID = item.TRANSACTION_ID
                });
            }
            return resultado;
        }

        public Tuple<bool, string> EstadoAntena()
        {
            return new Tuple<bool, string>(true, "Antena Rfid Ok.");
        }

        public Tuple<bool, string> EstadoBarrera()
        {
            return Input4 ? new Tuple<bool, string>(true, "Barrera Ok.") : new Tuple<bool, string>(false, "Barrera Apagada");
        }
    }
}

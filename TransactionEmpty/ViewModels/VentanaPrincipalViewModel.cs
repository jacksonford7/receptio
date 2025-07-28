using RECEPTIO.CapaPresentacion.UI.ImpresoraZebra;
using RECEPTIO.CapaPresentacion.UI.Interfaces.Impresora;
using RECEPTIO.CapaPresentacion.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using TransactionEmpty.ServicioTransactionEmpty;
using KIOSK = TransactionEmpty.ServicioTransactionEmpty.KIOSK;
using MESSAGE = TransactionEmpty.ServicioTransactionEmpty.MESSAGE;
using DEPOT = TransactionEmpty.ServicioTransactionEmpty.DEPOT;
using System.Configuration;

namespace TransactionEmpty.ViewModels
{
    public partial class VentanaPrincipalViewModel : ViewModelBase, IDisposable
    {
        #region Campos
        internal readonly Frame Contenedor;
        internal readonly ServicioTransactionEmptyClient Servicio;
        internal readonly ServicioAnuncianteProblema.ServicioAnuncianteProblemaClient ServicioAnuncianteProblema;
        internal IEnumerable<MESSAGE> Mensajes;
        internal KIOSK Quiosco;
        internal DEPOT Depot;
        internal Tuple<List<string>, List<string>> MensajesEstadoImpresora;
        internal BrushConverter Convertidor;
        private EstadoProceso _estado;
        private bool _estaOcupado;
        private string _mensajeBusy;
        private Brush _inicioBackground;
        private Brush _ticketBackground;
        private string _rutaImagenInicio;
        private string _rutaImagenTicket;
        private string _fecha;
        private bool _disposed;
        #endregion

        #region Constructor
        public VentanaPrincipalViewModel(Frame contenedor)
        {
            Contenedor = contenedor;
            Servicio = new ServicioTransactionEmptyClient();
            ServicioAnuncianteProblema = new ServicioAnuncianteProblema.ServicioAnuncianteProblemaClient();
            Convertidor = new BrushConverter();
            ObtenerQuiosco();
            ObtenerMensajes();
            if (ConfigurationManager.AppSettings["ValidarImpresora"] == "1")
            {
                VerEstadoImpresora();
            }
            HostearServicio();
            IrHomeQuiosco();
            _dispatcherImpresora.Stop();
            _dispatcherImpresora.Tick += TemporizadorImpresion;
        }
        #endregion

        #region Propiedades
        internal RespuestaN4 DatosN4 { get; set; }

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
                CerrarAplicacion($"No existe un kiosco registrado para esta ip : {ip}.");
            if (!Quiosco.IS_ACTIVE)
                CerrarAplicacion("El kiosco no está activo.");
            App.Current.Resources.Add("Quiosco", Quiosco);
            OnPropertyChanged("NumeroKiosco");
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

        private void CerrarAplicacion(string mensaje)
        {
            MessageBox.Show($@"{mensaje}{Environment.NewLine}La aplicación se cerrará.",
                "TRANSACTION EMPTY", MessageBoxButton.OK, MessageBoxImage.Error);
            Environment.Exit(0);
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
                    MessageBox.Show($"La impresora tiene errores que se detallan a continuación:{Environment.NewLine}Errores: {mensajesErrores}{Environment.NewLine}Advertencias: {mensajesAdvertencias}", "TRANSACTION EMPTY", MessageBoxButton.OK, MessageBoxImage.Error);
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
        private DispatcherTimer _dispatcherImpresora = new DispatcherTimer { Interval = new TimeSpan(0, 0, 3), IsEnabled = true };
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
            var imprimirTicketEntrada = new ImprimirTicketEntregaVacio();
            imprimirTicketEntrada.Procesar(this);
            _dispatcherImpresora.Start();
        }

        private void TemporizadorImpresion(object sender, EventArgs e)
        {
            _dispatcherImpresora.Stop();
            VerEstadoImpresora(false);
        }

        public Tuple<bool, string> EstadoAntena()
        {
            return new Tuple<bool, string>(true, "Antena Rfid Ok.");
        }

        public Tuple<bool, string> EstadoBarrera()
        {
            return new Tuple<bool, string>(true, "Barrera Ok.");
        }

        public void AbrirBarreraLider()
        {
        }

        public Tuple<bool, string> AbrirBarreraTroubleDesk()
        {
            return new Tuple<bool, string>(false, $"El kiosco de vacios no posee barrera.");
        }

        public Tuple<bool, string> IrHomeQuiosco()
        {
            if (PasoActual == "INICIO")
                return new Tuple<bool, string>(false, $"No se puede ir al home del kiosco, porque ya está en él.");
            else
            {
                if (EstaOcupado)
                    return new Tuple<bool, string>(false, $"No se puede ir al home porque el kiosco está realizando un proceso.{Environment.NewLine}Vuelva a intentarlo en unos segundos.");
                else
                {
                    SetearEstado(new PaginaInicioViewModel());
                    return new Tuple<bool, string>(true, "");
                }
            }
        }

        public Tuple<bool, string> ReimprimirCualquierTicket(RECEPTIO.CapaDominio.Nucleo.Entidades.PRE_GATE preGate, string xml)
        {
            return new Tuple<bool, string>(false, $"Funcionalidad no desarrollada aún.");
        }
    }
}
using RECEPTIO.CapaPresentacion.UI.Biometrico;
using RECEPTIO.CapaPresentacion.UI.Interfaces.Biometrico;
using RECEPTIO.CapaPresentacion.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using Transaction.Properties;
using Transaction.ServicioTransaction;
using Transaction.Views;

namespace Transaction.ViewModels
{
    internal class PaginaBiometricoViewModel : EstadoProceso
    {
        #region Campos
        private string _licencia;
        private string _nombreConductor;
        protected VentanaPrincipalViewModel ViewModel;
        protected bool PuedoIntentar;
        #endregion

        #region Constructor
        internal PaginaBiometricoViewModel()
        {
        }
        #endregion

        #region Propiedades
        public ICommand ComandoIntentarNuevamenteHuella
        {
            get
            {
                return new RelayCommand(IntentarNuevamenteHuella, PuedoIntentarNuevamente);
            }
        }

        public string Licencia
        {
            get
            {
                return _licencia;
            }
            set
            {
                if (_licencia == value) return;
                _licencia = value;
                OnPropertyChanged("Licencia");
            }
        }

        public string NombreConductor
        {
            get
            {
                return _nombreConductor;
            }
            set
            {
                if (_nombreConductor == value) return;
                _nombreConductor = value;
                OnPropertyChanged("NombreConductor");
            }
        }
        #endregion

        #region Metodos
        internal override void EstablecerControles(VentanaPrincipalViewModel viewModel)
        {
            ViewModel = viewModel;
            Worker.DoWork += IniciarHilo;
            Worker.ProgressChanged += Progreso;
            Worker.RunWorkerCompleted += ProcesoCompletado;
            Dispatcher.Tick += Temporizador;
            Dispatcher.Stop();
            EstablecerPropiedadesViewModel();
            Navegar();
            Procesar();
        }

        private void Temporizador(object sender, EventArgs e)
        {
            Dispatcher.Stop();
            CambiarEstado();
        }

        private void EstablecerPropiedadesViewModel()
        {
            ColorTextoMensaje = (Brush)ViewModel.Convertidor.ConvertFromString("#191007");
            ViewModel.InicioBackground = Brushes.Transparent;
            ViewModel.HuellaBackground = (Brush)ViewModel.Convertidor.ConvertFromString("#EF6C00");
            ViewModel.RfidBackground = Brushes.Transparent;
            ViewModel.ProcesoBackground = Brushes.Transparent;
            ViewModel.TicketBackground = Brushes.Transparent;
            ViewModel.BarreraBackground = Brushes.Transparent;
            ViewModel.Fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            ViewModel.PuedoIrHome = true;
            Mensaje = Resources.MensajeBiometrico;
            ViewModel.PasoActual = "HUELLA";
            EstablecerIconos();
        }

        private void EstablecerIconos()
        {
            ViewModel.RutaImagenInicio = @"..\Imagenes\Ingreso.png";
            ViewModel.RutaImagenHuella = @"..\Imagenes\Huella_Blanco.png";
            ViewModel.RutaImagenRfid = @"..\Imagenes\Rfid.png";
            ViewModel.RutaImagenProceso = @"..\Imagenes\Proceso.png";
            ViewModel.RutaImagenTicket = @"..\Imagenes\Ticket.png";
            ViewModel.RutaImagenBarrera = @"..\Imagenes\Barrera.png";
        }

        private void Navegar()
        {
            var paso = new PaginaHuella { DataContext = this };
            paso.KeepAlive = false;
            ViewModel.Contenedor.Navigate(paso);
        }

        private bool PuedoIntentarNuevamente()
        {
            return PuedoIntentar;
        }

        private void IntentarNuevamenteHuella()
        {
            if (BotonPresionado)
                return;
            BotonPresionado = true;
            Procesar();
        }

        private void Procesar()
        {
            Worker.RunWorkerAsync();
        }

        internal void Progreso(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            ViewModel.EstaOcupado = true;
            ViewModel.MensajeBusy = Resources.Procesando;
        }

        internal void ProcesoCompletado(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            ViewModel.EstaOcupado = false;
            if (e.Error == null)
                MostrarResultadoEnPantalla();
            else
            {
                Mensaje = ViewModel.Mensajes.FirstOrDefault(m => m.MESSAGE_ID == 32).USER_MESSAGE;
                ColorTextoMensaje = Brushes.Red;
                throw e.Error;
            }
        }

        protected virtual void IniciarHilo(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Worker.ReportProgress(0);

            if (ViewModel.DatosPreGate.PreGate.PRE_GATE_DETAILS.All(pr => pr.TRANSACTION_TYPE_ID != 17))
            {
                IBiometrico biometrico = new Biometrico();
                ViewModel.DatoHuella = biometrico.ProcesoHuella(ViewModel.DatosPreGate.PreGate.DRIVER_ID);
                ViewModel.Servicio.RegistrarProceso(new KIOSK_TRANSACTION
                {
                    TRANSACTION_ID = ViewModel.DatosPreGate.IdTransaccion,
                    PRE_GATE_ID = ViewModel.DatosPreGate.PreGate.PRE_GATE_ID,
                    PROCESSES = new List<PROCESS> { new PROCESS
                    {
                        STEP = "HUELLA",
                        RESPONSE = ViewModel.DatoHuella,
                        IS_OK = ViewModel.DatoHuella.Contains(ViewModel.DatosPreGate.PreGate.DRIVER_ID),
                        MESSAGE_ID = ObtenerIdMensaje(ViewModel.DatoHuella)
                    } },
                    KIOSK = ViewModel.Quiosco
                });
            }
        }

        protected virtual int ObtenerIdMensaje(string mensaje)
        {
            if (mensaje == "ERROR CON EL DISPOSITIVO")
                return 5;
            else if (mensaje == "HUELLA NO CONCUERDA")
                return 6;
            else if (ViewModel.DatoHuella.Contains(ViewModel.DatosPreGate.PreGate.DRIVER_ID))
                return 8;
            else
                return 7;
        }

        protected virtual void MostrarResultadoEnPantalla()
        {
            if (ViewModel.DatosPreGate.PreGate.PRE_GATE_DETAILS.All(pr => pr.TRANSACTION_TYPE_ID != 17))
            {
                var mensaje = ObtenerMensajeBiometrico(ViewModel.DatoHuella);
                Mensaje = mensaje.USER_MESSAGE;
                ColorTextoMensaje = ViewModel.DatoHuella.Contains(ViewModel.DatosPreGate.PreGate.DRIVER_ID) ? (Brush)ViewModel.Convertidor.ConvertFromString("#191007") : Brushes.Red;
                if (ViewModel.DatoHuella.Contains(ViewModel.DatosPreGate.PreGate.DRIVER_ID))
                {
                    Licencia = ViewModel.DatoHuella.Split(':')[0];
                    NombreConductor = ViewModel.DatoHuella.Split(':')[1];
                    Dispatcher.Start();
                }
                else
                {
                    mensaje.ContadorIntentos++;
                    if (mensaje.ATTEMPTS == mensaje.ContadorIntentos)
                    {
                        var id = Convert.ToInt32(Resources.IdMensajeGeneralEnvioProblema);
                        Mensaje = ViewModel.Mensajes.FirstOrDefault(m => m.MESSAGE_ID == id).USER_MESSAGE;
                        ViewModel.ServicioAnuncianteProblema.AnunciarProblema(ViewModel.DatosPreGate.IdTransaccion);
                    }
                    BotonPresionado = false;
                    PuedoIntentar = true;
                    ComandoIntentarNuevamenteHuella.CanExecute(null);
                }
            }
            else
            {
                Dispatcher.Start();
            }
        }

        protected virtual MESSAGE ObtenerMensajeBiometrico(string mensaje)
        {
            if (mensaje == "ERROR CON EL DISPOSITIVO")
                return ViewModel.Mensajes.FirstOrDefault(m => m.MESSAGE_ID == 5);
            else if (mensaje == "HUELLA NO CONCUERDA")
                return ViewModel.Mensajes.FirstOrDefault(m => m.MESSAGE_ID == 6);
            else if (ViewModel.DatoHuella.Contains(ViewModel.DatosPreGate.PreGate.DRIVER_ID))
                return ViewModel.Mensajes.FirstOrDefault(m => m.MESSAGE_ID == 8);
            else
                return ViewModel.Mensajes.FirstOrDefault(m => m.MESSAGE_ID == 7);
        }

        internal override void CambiarEstado()
        {
            ViewModel.SetearEstado(new PaginaRfidViewModel());
        }
        #endregion
    }

    internal class PaginaSalidaBiometricoViewModel : PaginaBiometricoViewModel
    {
        #region Campos
        #endregion

        #region Constructor
        internal PaginaSalidaBiometricoViewModel()
        {
        }
        #endregion

        #region Propiedades
        #endregion

        #region Metodos
        protected override void IniciarHilo(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Worker.ReportProgress(0);
            if (ViewModel.DatosPreGateSalida.PreGate.PRE_GATE_DETAILS.All(pr => pr.TRANSACTION_TYPE_ID != 17))
            {
                IBiometrico biometrico = new Biometrico();
                ViewModel.DatoHuella = biometrico.ProcesoHuella(ViewModel.DatosPreGateSalida.PreGate.DRIVER_ID);
                ViewModel.Servicio.RegistrarProceso(new KIOSK_TRANSACTION
                {
                    TRANSACTION_ID = Convert.ToInt32(ViewModel.DatosPreGateSalida.IdTransaccion),
                    PRE_GATE_ID = ViewModel.DatosPreGateSalida.PreGate.PRE_GATE_ID,
                    PROCESSES = new List<PROCESS> { new PROCESS
                    {
                        STEP = "HUELLA",
                        RESPONSE = ViewModel.DatoHuella,
                        IS_OK = ViewModel.DatoHuella.Contains(ViewModel.DatosPreGateSalida.PreGate.DRIVER_ID),
                        MESSAGE_ID = ObtenerIdMensaje(ViewModel.DatoHuella)
                    } },
                    KIOSK = ViewModel.Quiosco
                });
            }
        }

        protected override MESSAGE ObtenerMensajeBiometrico(string mensaje)
        {
            if (mensaje == "ERROR CON EL DISPOSITIVO")
                return ViewModel.Mensajes.FirstOrDefault(m => m.MESSAGE_ID == 5);
            else if (mensaje == "HUELLA NO CONCUERDA")
                return ViewModel.Mensajes.FirstOrDefault(m => m.MESSAGE_ID == 6);
            else if (ViewModel.DatoHuella.Contains(ViewModel.DatosPreGateSalida.PreGate.DRIVER_ID))
                return ViewModel.Mensajes.FirstOrDefault(m => m.MESSAGE_ID == 8);
            else
                return ViewModel.Mensajes.FirstOrDefault(m => m.MESSAGE_ID == 7);
        }

        protected override int ObtenerIdMensaje(string mensaje)
        {
            if (mensaje == "ERROR CON EL DISPOSITIVO")
                return 5;
            else if (mensaje == "HUELLA NO CONCUERDA")
                return 6;
            else if (ViewModel.DatoHuella.Contains(ViewModel.DatosPreGateSalida.PreGate.DRIVER_ID))
                return 8;
            else
                return 7;
        }

        protected override void MostrarResultadoEnPantalla()
        {
            if (ViewModel.DatosPreGateSalida.PreGate.PRE_GATE_DETAILS.All(pr => pr.TRANSACTION_TYPE_ID != 17))
            {
                var mensaje = ObtenerMensajeBiometrico(ViewModel.DatoHuella);
                Mensaje = mensaje.USER_MESSAGE;
                ColorTextoMensaje = ViewModel.DatoHuella.Contains(ViewModel.DatosPreGateSalida.PreGate.DRIVER_ID) ? (Brush)ViewModel.Convertidor.ConvertFromString("#191007") : Brushes.Red;
                if (ViewModel.DatoHuella.Contains(ViewModel.DatosPreGateSalida.PreGate.DRIVER_ID))
                {
                    Licencia = ViewModel.DatoHuella.Split(':')[0];
                    NombreConductor = $"{ViewModel.DatoHuella.Split(':')[1]} {ViewModel.DatoHuella.Split(':')[2]}";
                    Dispatcher.Start();
                }
                else
                {
                    mensaje.ContadorIntentos++;
                    if (mensaje.ATTEMPTS == mensaje.ContadorIntentos)
                    {
                        var id = Convert.ToInt32(Resources.IdMensajeGeneralEnvioProblema);
                        Mensaje = ViewModel.Mensajes.FirstOrDefault(m => m.MESSAGE_ID == id).USER_MESSAGE;
                        ViewModel.ServicioAnuncianteProblema.AnunciarProblema(Convert.ToInt32(ViewModel.DatosPreGateSalida.IdTransaccion));
                    }
                    BotonPresionado = false;
                    PuedoIntentar = true;
                    ComandoIntentarNuevamenteHuella.CanExecute(null);
                }
            }
            else
            {
                Dispatcher.Start();
            }
            
        }

        internal override void CambiarEstado()
        {
            ViewModel.SetearEstado(new PaginaSalidaRfidViewModel());
        }
        #endregion
    }
}
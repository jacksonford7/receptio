using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Transaction.Properties;
using Transaction.ServicioTransaction;
using Transaction.Views;

namespace Transaction.ViewModels
{
    internal class PaginaRfidViewModel : EstadoProceso
    {
        #region Campos
        protected VentanaPrincipalViewModel ViewModel;
        protected bool FueOk;
        protected MESSAGE MensajeBaseDatos;
        protected DispatcherTimer DispatcherRfid = new DispatcherTimer { Interval = new TimeSpan(0, 0, 10), IsEnabled = true };
        protected int Intentos;
        #endregion

        #region Constructor
        internal PaginaRfidViewModel()
        {
            EsVisibleBoton = Visibility.Collapsed;
            Titulo = "ESCANEO DE TAG RFID";
            DispatcherRfid.Stop();
            DispatcherRfid.Tick += TemporizadorRfid;
        }
        #endregion

        #region Propiedades
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
            Mensaje = Resources.MensajeRfid;
            Navegar();
            Procesar();
        }

        private void Temporizador(object sender, EventArgs e)
        {
            Dispatcher.Stop();
            ViewModel.EstaOcupado = false;
            CambiarEstado();
        }

        private void EstablecerPropiedadesViewModel()
        {
            ColorTextoMensaje = (Brush)ViewModel.Convertidor.ConvertFromString("#191007");
            ViewModel.InicioBackground = Brushes.Transparent;
            ViewModel.HuellaBackground = Brushes.Transparent;
            ViewModel.RfidBackground = (Brush)ViewModel.Convertidor.ConvertFromString("#EF6C00");
            ViewModel.ProcesoBackground = Brushes.Transparent;
            ViewModel.TicketBackground = Brushes.Transparent;
            ViewModel.BarreraBackground = Brushes.Transparent;
            ViewModel.Fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            ViewModel.PuedoIrHome = true;
            ViewModel.PasoActual = "RFID";
            EstablecerIconos();
        }

        private void EstablecerIconos()
        {
            ViewModel.RutaImagenInicio = @"..\Imagenes\Ingreso.png";
            ViewModel.RutaImagenHuella = @"..\Imagenes\Huella.png";
            ViewModel.RutaImagenRfid = @"..\Imagenes\Rfid_Blanco.png";
            ViewModel.RutaImagenProceso = @"..\Imagenes\Proceso.png";
            ViewModel.RutaImagenTicket = @"..\Imagenes\Ticket.png";
            ViewModel.RutaImagenBarrera = @"..\Imagenes\Barrera.png";
        }

        private void Navegar()
        {
            var paso = new PaginaMultiuso { DataContext = this };
            paso.KeepAlive = false;
            ViewModel.Contenedor.Navigate(paso);
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
                var transaccion = new KIOSK_TRANSACTION
                {
                    PRE_GATE_ID = ViewModel.DatosPreGate.PreGate.PRE_GATE_ID,
                    TRANSACTION_ID = ViewModel.DatosPreGate.IdTransaccion,
                    PROCESSES = new List<PROCESS> { new PROCESS
                    {
                        STEP = "RFID",
                        RESPONSE = ""
                    } },
                    KIOSK = ViewModel.Quiosco
                };
                var tagBaseDatos = ViewModel.Servicio.ObtenerTag(ViewModel.DatosPreGate.PreGate.TRUCK_LICENCE);
                ViewModel.TagReal = $"Tag Base Datos : {tagBaseDatos}";
                ProcesoAntena(transaccion, tagBaseDatos);
        }

        protected void ProcesoAntena(KIOSK_TRANSACTION transaccion, string tagBaseDatos)
        {
                if (string.IsNullOrWhiteSpace(tagBaseDatos))
                {
                    transaccion.PROCESSES.FirstOrDefault().MESSAGE_ID = 9;
                    MensajeBaseDatos = ViewModel.Mensajes.FirstOrDefault(m => m.MESSAGE_ID == 9);
                    Mensaje = MensajeBaseDatos.USER_MESSAGE;
                    Intentos = 6;
                }
                else
                {
                    var tags = ViewModel.Antena.ObtenerTagsLeidos();
                    if (tags.Count() == 0)
                    {
                        transaccion.PROCESSES.FirstOrDefault().MESSAGE_ID = 10;
                        MensajeBaseDatos = ViewModel.Mensajes.FirstOrDefault(m => m.MESSAGE_ID == 10);
                        Mensaje = MensajeBaseDatos.USER_MESSAGE;
                    }
                    if (tags.Contains(tagBaseDatos))
                    {
                        transaccion.PROCESSES.FirstOrDefault().MESSAGE_ID = 12;
                        transaccion.PROCESSES.FirstOrDefault().IS_OK = true;
                        transaccion.PROCESSES.FirstOrDefault().RESPONSE = $"Tags leídos : {tags.Aggregate("", (actual, item) => (actual == "" ? "" : actual + ",") + item)}. Tag de base datos : {tagBaseDatos}";
                        MensajeBaseDatos = ViewModel.Mensajes.FirstOrDefault(m => m.MESSAGE_ID == 12);
                        Mensaje = MensajeBaseDatos.USER_MESSAGE;
                        FueOk = true;
                        ViewModel.TagReal = "";
                    }
                    else
                    {
                        transaccion.PROCESSES.FirstOrDefault().MESSAGE_ID = 11;
                        transaccion.PROCESSES.FirstOrDefault().RESPONSE = $"Tags leídos : {tags.Aggregate("", (actual, item) => (actual == "" ? "" : actual + ",") + item)}. Tag de base datos : {tagBaseDatos}";
                        MensajeBaseDatos = ViewModel.Mensajes.FirstOrDefault(m => m.MESSAGE_ID == 11);
                        Mensaje = MensajeBaseDatos.USER_MESSAGE;
                    }
                }
                ViewModel.Servicio.RegistrarProceso(transaccion);
        }

        protected virtual void MostrarResultadoEnPantalla()
        {
            if (ViewModel.DatosPreGate.PreGate.PRE_GATE_DETAILS.All(pr => pr.TRANSACTION_TYPE_ID != 17))
            {
                ColorTextoMensaje = FueOk ? (Brush)ViewModel.Convertidor.ConvertFromString("#191007") : Brushes.Red;
                if (FueOk)
                {
                    DispatcherRfid.Stop();
                    Dispatcher.Start();
                }
                else
                {
                    MensajeBaseDatos.ContadorIntentos++;
                    if (MensajeBaseDatos.ATTEMPTS == MensajeBaseDatos.ContadorIntentos)
                        ViewModel.ServicioAnuncianteProblema.AnunciarProblema(ViewModel.DatosPreGate.IdTransaccion);
                    if (Intentos == 0)
                        DispatcherRfid.Start();
                    Intentos++;
                }
            }
            else
            {
                DispatcherRfid.Stop();
                Dispatcher.Start();
            }
        }

        private void TemporizadorRfid(object sender, EventArgs e)
        {
            if (Intentos == 6)
            {
                DispatcherRfid.Stop();
                var id = Convert.ToInt32(Resources.IdMensajeGeneralEnvioProblema);
                Mensaje = ViewModel.Mensajes.FirstOrDefault(m => m.MESSAGE_ID == id).USER_MESSAGE;
                ViewModel.VentanaAutorizacionDisponible = true;
                ViewModel.EstaOcupado = false;
            }
            else
                Procesar();
        }

        internal override void CambiarEstado()
        {
            ViewModel.SetearEstado(new PaginaN4ViewModel());
        }
        #endregion
    }

    internal class PaginaSalidaRfidViewModel : PaginaRfidViewModel
    {
        #region Campos
        #endregion

        #region Constructor
        internal PaginaSalidaRfidViewModel() : base()
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
                var transaccion = new KIOSK_TRANSACTION
                {
                    PRE_GATE_ID = ViewModel.DatosPreGateSalida.PreGate.PRE_GATE_ID,
                    TRANSACTION_ID = Convert.ToInt32(ViewModel.DatosPreGateSalida.IdTransaccion),
                    PROCESSES = new List<PROCESS> { new PROCESS
                {
                    STEP = "RFID",
                    RESPONSE = ""
                } },
                    KIOSK = ViewModel.Quiosco
                };
                var tagBaseDatos = ViewModel.Servicio.ObtenerTag(ViewModel.DatosPreGateSalida.PreGate.TRUCK_LICENCE);
                ViewModel.TagReal = $"Tag Base Datos : {tagBaseDatos}";
                ProcesoAntena(transaccion, tagBaseDatos);
            }
                
        }

        protected override void MostrarResultadoEnPantalla()
        {
            if (ViewModel.DatosPreGateSalida.PreGate.PRE_GATE_DETAILS.All(pr => pr.TRANSACTION_TYPE_ID != 17))
            {
                ColorTextoMensaje = FueOk ? (Brush)ViewModel.Convertidor.ConvertFromString("#191007") : Brushes.Red;
                if (FueOk)
                {
                    DispatcherRfid.Stop();
                    Dispatcher.Start();
                }
                else
                {
                    MensajeBaseDatos.ContadorIntentos++;
                    if (MensajeBaseDatos.ATTEMPTS == MensajeBaseDatos.ContadorIntentos)
                        ViewModel.ServicioAnuncianteProblema.AnunciarProblema(Convert.ToInt32(ViewModel.DatosPreGateSalida.IdTransaccion));
                    if (Intentos == 0)
                        DispatcherRfid.Start();
                    Intentos++;
                }
            }
            else
            {
                DispatcherRfid.Stop();
                Dispatcher.Start();
            }
        }

        internal override void CambiarEstado()
        {
            ViewModel.SetearEstado(new PaginaSalidaN4ViewModel());
        }
        #endregion
    }
}
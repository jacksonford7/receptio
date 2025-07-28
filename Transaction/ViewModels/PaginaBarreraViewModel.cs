using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Transaction.Properties;
using Transaction.ServicioTransaction;
using Transaction.Views;

namespace Transaction.ViewModels
{
    internal class PaginaBarreraViewModel : EstadoProceso
    {
        #region Campos
        protected VentanaPrincipalViewModel ViewModel;
        #endregion

        #region Constructor
        internal PaginaBarreraViewModel()
        {
            EsVisibleBoton = System.Windows.Visibility.Collapsed;
            Titulo = "BARRERA";
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
            ViewModel.Fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            ColorTextoMensaje = (Brush)ViewModel.Convertidor.ConvertFromString("#191007");
            EstablecerPropiedadesViewModel();
            Mensaje = Resources.MensajeBarrera;
            Navegar();
            ViewModel.Barrera.LevantarBarrera();
            Procesar();
        }

        private void Temporizador(object sender, EventArgs e)
        {
            Dispatcher.Stop();
            ViewModel.Barrera.BajarBarrera();
            RegistarFinProceso();
            ViewModel.EstaOcupado = false;
            CambiarEstado();
        }

        protected virtual void RegistarFinProceso()
        {
            var transaccion = new KIOSK_TRANSACTION
            {
                TRANSACTION_ID = ViewModel.DatosPreGate.IdTransaccion,
                IS_OK = true,
                PRE_GATE_ID = ViewModel.DatosPreGate.PreGate.PRE_GATE_ID,
                PROCESSES = new List<PROCESS> { new PROCESS
                        {
                            STEP = "BARRERA",
                            RESPONSE = "",
                            IS_OK = true,
                            MESSAGE_ID = 17
                        } },
                KIOSK = ViewModel.Quiosco
            };
            ViewModel.Servicio.RegistrarProceso(transaccion);
        }

        private void EstablecerPropiedadesViewModel()
        {
            ColorTextoMensaje = (Brush)ViewModel.Convertidor.ConvertFromString("#191007");
            ViewModel.InicioBackground = Brushes.Transparent;
            ViewModel.HuellaBackground = Brushes.Transparent;
            ViewModel.RfidBackground = Brushes.Transparent;
            ViewModel.ProcesoBackground = Brushes.Transparent;
            ViewModel.TicketBackground = Brushes.Transparent;
            ViewModel.BarreraBackground = (Brush)ViewModel.Convertidor.ConvertFromString("#EF6C00");
            ViewModel.Fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            ViewModel.PuedoIrHome = false;
            ViewModel.PasoActual = "BARRERA";
            EstablecerIconos();
        }

        private void EstablecerIconos()
        {
            ViewModel.RutaImagenInicio = @"..\Imagenes\Ingreso.png";
            ViewModel.RutaImagenHuella = @"..\Imagenes\Huella.png";
            ViewModel.RutaImagenRfid = @"..\Imagenes\Rfid.png";
            ViewModel.RutaImagenProceso = @"..\Imagenes\Proceso.png";
            ViewModel.RutaImagenTicket = @"..\Imagenes\Ticket.png";
            ViewModel.RutaImagenBarrera = @"..\Imagenes\Barrera_Blanco.png";
        }

        private void Navegar()
        {
            var paso = new PaginaMultiuso { DataContext = this };
            paso.KeepAlive = false;
            ViewModel.Contenedor.Navigate(paso);
        }

        private void Procesar()
        {
            if (!ViewModel.Input1)
                ViewModel.LeerSinLoop = false;
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
                Dispatcher.Start();
            else
            {
                Mensaje = ViewModel.Mensajes.FirstOrDefault(m => m.MESSAGE_ID == 32).USER_MESSAGE;
                ColorTextoMensaje = Brushes.Red;
                throw e.Error;
            }
        }

        private void IniciarHilo(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Worker.ReportProgress(0);
        }

        internal override void CambiarEstado()
        {
            ViewModel.SetearEstado(new PaginaInicioViewModel());
        }
        #endregion
    }

    internal class PaginaSalidaBarreraViewModel : PaginaBarreraViewModel
    {
        #region Campos
        #endregion

        #region Constructor
        internal PaginaSalidaBarreraViewModel() : base()
        {
        }
        #endregion

        #region Propiedades
        #endregion

        #region Metodos
        protected override void RegistarFinProceso()
        {
            var transaccion = new KIOSK_TRANSACTION
            {
                TRANSACTION_ID = Convert.ToInt32(ViewModel.DatosPreGateSalida.IdTransaccion),
                IS_OK = true,
                PRE_GATE_ID = ViewModel.DatosPreGateSalida.PreGate.PRE_GATE_ID,
                PROCESSES = new List<PROCESS> { new PROCESS
                        {
                            STEP = "BARRERA",
                            RESPONSE = "",
                            IS_OK = true,
                            MESSAGE_ID = 17
                        } },
                KIOSK = ViewModel.Quiosco
            };
            ViewModel.Servicio.RegistrarProceso(transaccion);
        }

        internal override void CambiarEstado()
        {
            ViewModel.SetearEstado(new PaginaSalidaInicioViewModel());
        }
        #endregion
    }
}
using System;
using System.Linq;
using System.Windows.Media;
using Transaction.Properties;
using Transaction.ServicioTransaction;
using Transaction.Views;

namespace Transaction.ViewModels
{
    internal class PaginaN4ViewModel : EstadoProceso
    {
        #region Campos
        internal VentanaPrincipalViewModel ViewModel;
        #endregion

        #region Constructor
        internal PaginaN4ViewModel()
        {
            EsVisibleBoton = System.Windows.Visibility.Collapsed;
            Titulo = "PROCESOS";
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
            Mensaje = Resources.MensajeProcesosN4;
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
            ViewModel.HuellaBackground = Brushes.Transparent;
            ViewModel.RfidBackground = Brushes.Transparent;
            ViewModel.ProcesoBackground = (Brush)ViewModel.Convertidor.ConvertFromString("#EF6C00");
            ViewModel.TicketBackground = Brushes.Transparent;
            ViewModel.BarreraBackground = Brushes.Transparent;
            ViewModel.Fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            ViewModel.PuedoIrHome = false;
            ViewModel.PasoActual = "N4";
            EstablecerIconos();
        }

        private void EstablecerIconos()
        {
            ViewModel.RutaImagenInicio = @"..\Imagenes\Ingreso.png";
            ViewModel.RutaImagenHuella = @"..\Imagenes\Huella.png";
            ViewModel.RutaImagenRfid = @"..\Imagenes\Rfid.png";
            ViewModel.RutaImagenProceso = @"..\Imagenes\Proceso_Blanco.png";
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
            var procesosN4Entrada = new ProcesoEntradaDeliveryImportReceiveExportFullBrBkCfs();
            procesosN4Entrada.Procesar(this);
        }

        protected virtual void MostrarResultadoEnPantalla()
        {
            var id = Convert.ToInt32(ViewModel.DatosN4.Mensaje);
            Mensaje = ViewModel.Mensajes.FirstOrDefault(m => m.MESSAGE_ID == id).USER_MESSAGE;
            ColorTextoMensaje = ViewModel.DatosN4.FueOk ? (Brush)ViewModel.Convertidor.ConvertFromString("#191007") : Brushes.Red;
            if (ViewModel.DatosN4.FueOk)
                Dispatcher.Start();
            else
                ViewModel.ServicioAnuncianteProblema.AnunciarProblema(ViewModel.DatosPreGate.IdTransaccion);
        }

        internal override void CambiarEstado()
        {
            ViewModel.SetearEstado(new PaginaImpresionViewModel());
        }
        #endregion
    }

    internal class PaginaSalidaN4ViewModel : PaginaN4ViewModel
    {
        #region Campos
        #endregion

        #region Constructor
        internal PaginaSalidaN4ViewModel() : base()
        {
        }
        #endregion

        #region Propiedades
        #endregion

        #region Metodos
        protected override void IniciarHilo(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Worker.ReportProgress(0);
            var procesosN4Salida = new ProcesoSalidaDeliveryImportReceiveExportFullBrBkCfs();
            procesosN4Salida.Procesar(this);
        }

        protected override void MostrarResultadoEnPantalla()
        {
            var id = Convert.ToInt32(ViewModel.DatosN4.Mensaje);
            Mensaje = ViewModel.Mensajes.FirstOrDefault(m => m.MESSAGE_ID == id).USER_MESSAGE;
            ColorTextoMensaje = ViewModel.DatosN4.FueOk ? (Brush)ViewModel.Convertidor.ConvertFromString("#191007") : Brushes.Red;
            if (ViewModel.DatosN4.FueOk)
                Dispatcher.Start();
            else
                ViewModel.ServicioAnuncianteProblema.AnunciarProblema(Convert.ToInt32(ViewModel.DatosPreGateSalida.IdTransaccion));
        }

        internal override void CambiarEstado()
        {
            ViewModel.SetearEstado(new PaginaSalidaImpresionViewModel());
        }
        #endregion
    }

    internal abstract class ProcesosN4Entrada
    {
        protected ProcesosN4Entrada Siguiente { get; set; }

        protected void EstablecerSiguiente(ProcesosN4Entrada siguiente)
        {
            Siguiente = siguiente;
        }

        internal abstract void Procesar(PaginaN4ViewModel viewModel);
    }

    internal class ProcesoEntradaDeliveryImportReceiveExportFullBrBkCfs : ProcesosN4Entrada
    {
        internal override void Procesar(PaginaN4ViewModel viewModel)
        {
            viewModel.ViewModel.DatosN4 = viewModel.ViewModel.Servicio.EjecutarProcesosEntrada(new DatosEntradaN4
            {
                CedulaChofer = viewModel.ViewModel.DatosPreGate.PreGate.DRIVER_ID,
                IdTransaccion = viewModel.ViewModel.DatosPreGate.IdTransaccion.ToString(),
                NombreQuiosco = viewModel.ViewModel.Quiosco.NAME,
                PlacaVehiculo = viewModel.ViewModel.DatosPreGate.PreGate.TRUCK_LICENCE,
                IdPreGate = viewModel.ViewModel.DatosPreGate.PreGate.PRE_GATE_ID,
                Peso = viewModel.ViewModel.DatosPreGate.Peso,
                TipoTransaccion = viewModel.ViewModel.DatosPreGate.PreGate.PRE_GATE_DETAILS.FirstOrDefault().TRANSACTION_TYPE_ID
            });
        }
    }

    internal abstract class ProcesosN4Salida
    {
        protected ProcesosN4Salida Siguiente { get; set; }

        protected void EstablecerSiguiente(ProcesosN4Salida siguiente)
        {
            Siguiente = siguiente;
        }

        internal abstract void Procesar(PaginaSalidaN4ViewModel viewModel);
    }

    internal class ProcesoSalidaDeliveryImportReceiveExportFullBrBkCfs : ProcesosN4Salida
    {
        internal override void Procesar(PaginaSalidaN4ViewModel viewModel)
        {
            viewModel.ViewModel.DatosN4 = viewModel.ViewModel.Servicio.EjecutarProcesosSalida(new DatosPreGateSalida
            {
                PreGate = viewModel.ViewModel.DatosPreGateSalida.PreGate,
                IdTransaccion = viewModel.ViewModel.DatosPreGateSalida.IdTransaccion,
                NombreQuiosco = viewModel.ViewModel.Quiosco.NAME,
                Peso = viewModel.ViewModel.DatosPreGateSalida.Peso,
                TipoTransaccion = viewModel.ViewModel.DatosPreGateSalida.PreGate.PRE_GATE_DETAILS.FirstOrDefault().TRANSACTION_TYPE_ID
            });
        }
    }
}
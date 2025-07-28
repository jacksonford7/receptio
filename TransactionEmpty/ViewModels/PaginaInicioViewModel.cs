using RECEPTIO.CapaPresentacion.UI.MVVM;
using System;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using TransactionEmpty.Properties;
using TransactionEmpty.Views;

namespace TransactionEmpty.ViewModels
{
    internal class PaginaInicioViewModel : EstadoProceso
    {
        #region Campos
        private string _numero;
        private VentanaPrincipalViewModel _viewModel;
        #endregion

        #region Constructor
        internal PaginaInicioViewModel()
        {
        }
        #endregion

        #region Propiedades
        public ICommand Comando0
        {
            get
            {
                return new RelayCommand(AgregarDigito0);
            }
        }

        public ICommand Comando1
        {
            get
            {
                return new RelayCommand(AgregarDigito1);
            }
        }

        public ICommand Comando2
        {
            get
            {
                return new RelayCommand(AgregarDigito2);
            }
        }

        public ICommand Comando3
        {
            get
            {
                return new RelayCommand(AgregarDigito3);
            }
        }

        public ICommand Comando4
        {
            get
            {
                return new RelayCommand(AgregarDigito4);
            }
        }

        public ICommand Comando5
        {
            get
            {
                return new RelayCommand(AgregarDigito5);
            }
        }

        public ICommand Comando6
        {
            get
            {
                return new RelayCommand(AgregarDigito6);
            }
        }

        public ICommand Comando7
        {
            get
            {
                return new RelayCommand(AgregarDigito7);
            }
        }

        public ICommand Comando8
        {
            get
            {
                return new RelayCommand(AgregarDigito8);
            }
        }

        public ICommand Comando9
        {
            get
            {
                return new RelayCommand(AgregarDigito9);
            }
        }

        public ICommand ComandoRetroceder
        {
            get
            {
                return new RelayCommand(Retroceder);
            }
        }

        public ICommand ComandoLimpiar
        {
            get
            {
                return new RelayCommand(Limpiar);
            }
        }

        public ICommand ComandoOk
        {
            get
            {
                return new RelayCommand(Procesar, PuedoProcesar);
            }
        }

        public string Numero
        {
            get
            {
                return _numero;
            }
            set
            {
                if (_numero == value) return;
                _numero = value;
                OnPropertyChanged("Numero");
            }
        }
        #endregion

        #region Metodos
        internal override void EstablecerControles(VentanaPrincipalViewModel viewModel)
        {
            _viewModel = viewModel;
            Worker.DoWork += IniciarHilo;
            Worker.ProgressChanged += Progreso;
            Worker.RunWorkerCompleted += ProcesoCompletado;
            Dispatcher.Tick += Temporizador;
            Dispatcher.Stop();
            LimpiarVariables();
            EstablecerPropiedadesViewModel();
            EstablecerTituloYMensaje();
            Numero = "";
            Navegar();
            EstablecerContadores();
        }

        private void LimpiarVariables()
        {
            _viewModel.DatosN4 = null;
        }

        private void EstablecerContadores()
        {
            foreach (var mensaje in _viewModel.Mensajes)
                mensaje.ContadorIntentos = 0;
        }

        private void Temporizador(object sender, EventArgs e)
        {
            Dispatcher.Stop();
            CambiarEstado();
        }

        private void EstablecerPropiedadesViewModel()
        {
            ColorTextoMensaje = (Brush)_viewModel.Convertidor.ConvertFromString("#191007");
            _viewModel.InicioBackground = (Brush)_viewModel.Convertidor.ConvertFromString("#EF6C00");
            _viewModel.TicketBackground = Brushes.Transparent;
            _viewModel.Fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            _viewModel.PasoActual = "HOME";
            EstablecerIconos();
        }

        private void EstablecerIconos()
        {
            _viewModel.RutaImagenInicio = @"..\Imagenes\Ingreso_Blanco.png";
            _viewModel.RutaImagenTicket = @"..\Imagenes\Ticket.png";
        }

        private void EstablecerTituloYMensaje()
        {
            Titulo = "INGRESO DE CODIGO";
            Mensaje = Resources.MensajeInicio;
        }

        private void Navegar()
        {
            var paso = new PaginaInicio { DataContext = this };
            paso.KeepAlive = false;
            _viewModel.Contenedor.Navigate(paso);
        }

        private void AgregarDigito0()
        {
            AgregarDigito(0);
        }

        private void AgregarDigito1()
        {
            AgregarDigito(1);
        }

        private void AgregarDigito2()
        {
            AgregarDigito(2);
        }

        private void AgregarDigito3()
        {
            AgregarDigito(3);
        }

        private void AgregarDigito4()
        {
            AgregarDigito(4);
        }

        private void AgregarDigito5()
        {
            AgregarDigito(5);
        }

        private void AgregarDigito6()
        {
            AgregarDigito(6);
        }

        private void AgregarDigito7()
        {
            AgregarDigito(7);
        }

        private void AgregarDigito8()
        {
            AgregarDigito(8);
        }

        private void AgregarDigito9()
        {
            AgregarDigito(9);
        }

        private void AgregarDigito(byte digito)
        {
            if (Numero.Length == 10) return;
            Numero = $"{Numero}{digito}";
        }

        private void Retroceder()
        {
            if (string.IsNullOrWhiteSpace(Numero) || Numero.Length == 0)
                return;
            else if (Numero.Length == 1)
                Numero = "";
            else
                Numero = Numero.Substring(0, Numero.Length - 1);
        }

        private void Limpiar()
        {
            Numero = "";
            Mensaje = Resources.MensajeInicio;
            ColorTextoMensaje = (Brush)_viewModel.Convertidor.ConvertFromString("#191007");
        }

        private bool PuedoProcesar()
        {
            return !string.IsNullOrWhiteSpace(Numero);
        }

        internal void Procesar()
        {
            if (BotonPresionado)
                return;
            BotonPresionado = true;
            Worker.RunWorkerAsync();
        }

        private void Progreso(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            _viewModel.EstaOcupado = true;
            _viewModel.MensajeBusy = Resources.Procesando;
        }

        private void ProcesoCompletado(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            _viewModel.EstaOcupado = false;
            if (e.Error == null)
                MostrarResultadoEnPantalla();
            else
            {
                Mensaje = _viewModel.Mensajes.FirstOrDefault(m => m.MESSAGE_ID == 32).USER_MESSAGE;
                ColorTextoMensaje = Brushes.Red;
                BotonPresionado = false;
                throw e.Error;
            }
        }

        private void IniciarHilo(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Worker.ReportProgress(0);
            _viewModel.DatosN4 = _viewModel.Servicio.Procesar(Convert.ToInt64(Numero), _viewModel.Quiosco);
        }

        private void MostrarResultadoEnPantalla()
        {
            var v_mensaje = _viewModel.DatosN4.Mensaje;
            string[] separadas = null;
            try
            {
                separadas = v_mensaje.Split('-');
                v_mensaje = separadas[0].Trim();
            }
            catch
            {
                v_mensaje = _viewModel.DatosN4.Mensaje;
            }

            if (_viewModel.DatosN4.FueOk && v_mensaje == "ERROR_IIE")
            {
                try
                {
                    var mensaje = _viewModel.Mensajes.FirstOrDefault(m => m.MESSAGE_ID == 13);
                    Mensaje = _viewModel.Mensajes.FirstOrDefault(m => m.MESSAGE_ID == 13).USER_MESSAGE;

                    _viewModel.ServicioAnuncianteProblema.AnunciarProblema(_viewModel.DatosN4.IdTransaccion);

                    Dispatcher.Start();
                }
                catch
                {
                    Dispatcher.Start();
                }
            }
            else
            {
                int id = 0;
                try
                {
                    v_mensaje = separadas.Count() > 1 ? separadas[1].Trim() : separadas[0].Trim();
                    id = Convert.ToInt32(v_mensaje);
                    Mensaje = _viewModel.Mensajes.FirstOrDefault(m => m.MESSAGE_ID == id).USER_MESSAGE;
                    ColorTextoMensaje = _viewModel.DatosN4.FueOk ? (Brush)_viewModel.Convertidor.ConvertFromString("#191007") : Brushes.Red;
                }
                catch (Exception ex)
                { LogEventos("Error PaginainicioViewModel - Linea 360 " + ex.Message + " - " + ex.Source); }

                if (_viewModel.DatosN4.FueOk)
                    Dispatcher.Start();
                else
                {
                    var mensaje = _viewModel.Mensajes.FirstOrDefault(m => m.MESSAGE_ID == id);
                    mensaje.ContadorIntentos++;
                    if (mensaje.ATTEMPTS == mensaje.ContadorIntentos)
                    {
                        id = Convert.ToInt32(Resources.IdMensajeGeneralEnvioProblema);
                        Mensaje = _viewModel.Mensajes.FirstOrDefault(m => m.MESSAGE_ID == id).USER_MESSAGE;
                        try { _viewModel.ServicioAnuncianteProblema.AnunciarProblema(_viewModel.DatosN4.IdTransaccion); } catch (Exception e) { BotonPresionado = false; throw new Exception("Error al intentar notificar Problema desde Kiosko/Vacio - " + e.Message); }
                    }
                    BotonPresionado = false;
                }
            }
        }

        internal override void CambiarEstado()
        {
            _viewModel.SetearEstado(new PaginaImpresionViewModel());
        }

        public static void LogEventos(string text)
        {
            try
            {
                string misDatos = System.IO.Path.Combine(System.Configuration.ConfigurationManager.AppSettings["RutaLog"].ToString()) + "\\LogEvent.txt";
                System.IO.StreamWriter escritor;
                escritor = System.IO.File.AppendText(misDatos);
                escritor.Write(DateTime.Now.ToString() + " - " + text + "\n");
                escritor.Flush();
                escritor.Close();
            }
            catch
            {
            }
        }
        #endregion
    }
}

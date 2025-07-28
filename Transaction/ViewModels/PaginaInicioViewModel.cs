using RECEPTIO.CapaPresentacion.UI.MVVM;
using System;
using System.Windows.Input;
using System.Windows.Media;
using Transaction.Properties;
using Transaction.Views;
using System.Linq;
using System.Collections.Generic;
using Transaction.ServicioTransaction;
using System.Configuration;

namespace Transaction.ViewModels
{
    internal class PaginaInicioViewModel : EstadoProceso
    {
        #region Campos
        private string _numero;
        protected VentanaPrincipalViewModel ViewModel;
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
            ViewModel = viewModel;
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
            ViewModel.DatosPreGate = null;
            ViewModel.DatosPreGateSalida = null;
            ViewModel.DatoHuella = null;
            ViewModel.DatosN4 = null;
        }

        private void EstablecerContadores()
        {
            foreach (var mensaje in ViewModel.Mensajes)
                mensaje.ContadorIntentos = 0;
        }

        private void Temporizador(object sender, EventArgs e)
        {
            Dispatcher.Stop();
            CambiarEstado();
        }

        private void EstablecerPropiedadesViewModel()
        {
            ColorTextoMensaje = (Brush)ViewModel.Convertidor.ConvertFromString("#191007");
            ViewModel.InicioBackground = (Brush)ViewModel.Convertidor.ConvertFromString("#EF6C00");
            ViewModel.HuellaBackground = Brushes.Transparent;
            ViewModel.RfidBackground = Brushes.Transparent;
            ViewModel.ProcesoBackground = Brushes.Transparent;
            ViewModel.TicketBackground = Brushes.Transparent;
            ViewModel.BarreraBackground = Brushes.Transparent;
            ViewModel.Fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            ViewModel.PuedoIrHome = false;
            ViewModel.VentanaAutorizacionDisponible = false;
            ViewModel.LeerSinLoop = false;
            ViewModel.TagReal = "";
            ViewModel.TipoTransaccion = "";
            ViewModel.PasoActual = "PRE_GATE";
            EstablecerIconos();
        }

        private void EstablecerIconos()
        {
            ViewModel.RutaImagenInicio = @"..\Imagenes\Ingreso_Blanco.png";
            ViewModel.RutaImagenHuella = @"..\Imagenes\Huella.png";
            ViewModel.RutaImagenRfid = @"..\Imagenes\Rfid.png";
            ViewModel.RutaImagenProceso = @"..\Imagenes\Proceso.png";
            ViewModel.RutaImagenTicket = @"..\Imagenes\Ticket.png";
            ViewModel.RutaImagenBarrera = @"..\Imagenes\Barrera.png";
        }

        protected virtual void EstablecerTituloYMensaje()
        {
            Titulo = "INGRESO DE CEDULA";
            Mensaje = Resources.MensajeInicioEntrada;
        }

        private void Navegar()
        {
            var paso = new PaginaInicio { DataContext = this };
            paso.KeepAlive = false;
            ViewModel.Contenedor.Navigate(paso);
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

        protected virtual void Limpiar()
        {
            Numero = "";
            Mensaje = Resources.MensajeInicioEntrada;
            ColorTextoMensaje = (Brush)ViewModel.Convertidor.ConvertFromString("#191007");
            ViewModel.LeerSinLoop = false;
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
            if (!ViewModel.Input1)
                ViewModel.LeerSinLoop = true;
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
            var sensoresKiosco = new Dictionary<short, bool>
            {
                { 1, ViewModel.Input1 },
                { 2, ViewModel.Input2 },
                { 3, ViewModel.Input3 },
                { 4, ViewModel.Input4 }
            };
            ViewModel.DatosPreGate = ViewModel.Servicio.ValidarPreGate(Numero, ViewModel.Quiosco.KIOSK_ID, sensoresKiosco);
        }

        protected virtual void MostrarResultadoEnPantalla()
        {
            var id = Convert.ToInt32(ViewModel.DatosPreGate.Mensaje);
            Mensaje = ViewModel.Mensajes.FirstOrDefault(m => m.MESSAGE_ID == id).USER_MESSAGE;
            ColorTextoMensaje = ViewModel.DatosPreGate.FueOk ? (Brush)ViewModel.Convertidor.ConvertFromString("#191007") : Brushes.Red;
            if (ViewModel.DatosPreGate.FueOk)
            {
                ViewModel.TipoTransaccion = ObtenerTipoTransaccion(ViewModel.DatosPreGate.PreGate);
                if ((ViewModel.DatosPreGate.PreGate.BY_PASS == null || !ViewModel.DatosPreGate.PreGate.BY_PASS.IS_ENABLED) && ConfigurationManager.AppSettings["ValidarSensorAltura"].ToString() == "1" && (ViewModel.DatosPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 1) || ViewModel.DatosPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 3) || ViewModel.DatosPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 7) || ViewModel.DatosPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 10) || ViewModel.DatosPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 11)))
                {
                    var cadenaResponsabilidadSensores = new DeliveryImportEntradaValidarSensores();
                    var resultado = cadenaResponsabilidadSensores.Procesar(ViewModel.DatosPreGate.PreGate, ViewModel);
                    if (resultado.Item1)
                        Dispatcher.Start();
                    else
                    {
                        ViewModel.Servicio.RegistrarProceso(new KIOSK_TRANSACTION
                        {
                            TRANSACTION_ID = ViewModel.DatosPreGate.IdTransaccion,
                            PRE_GATE_ID = ViewModel.DatosPreGate.PreGate.PRE_GATE_ID,
                            PROCESSES = new List<PROCESS> { new PROCESS
                            {
                                STEP = "SENSOR_ALTURA",
                                IS_OK = false,
                                RESPONSE = "",
                                MESSAGE_ID = resultado.Item2
                            } },
                            KIOSK = ViewModel.Quiosco
                        });
                        ColorTextoMensaje = Brushes.Red;
                        ViewModel.ServicioAnuncianteProblema.AnunciarProblema(ViewModel.DatosPreGate.IdTransaccion);
                        Mensaje = ViewModel.Mensajes.FirstOrDefault(m => m.MESSAGE_ID == resultado.Item2).USER_MESSAGE;
                        BotonPresionado = false;
                    }
                }
                else
                    Dispatcher.Start();
            }
            else
            {
                var mensaje = ViewModel.Mensajes.FirstOrDefault(m => m.MESSAGE_ID == id);
                mensaje.ContadorIntentos++;
                if (mensaje.ATTEMPTS == mensaje.ContadorIntentos)
                {
                    id = Convert.ToInt32(Resources.IdMensajeGeneralEnvioProblema);
                    Mensaje = ViewModel.Mensajes.FirstOrDefault(m => m.MESSAGE_ID == id).USER_MESSAGE;
                    ViewModel.ServicioAnuncianteProblema.AnunciarProblema(ViewModel.DatosPreGate.IdTransaccion);
                }
                BotonPresionado = false;
            }
        }

        protected string ObtenerTipoTransaccion(PRE_GATE preGate)
        {
            if (preGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 1))
                return "Importación Full";
            if (preGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 2))
                return "Importación BrBk";
            else if (preGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 3))
                return "Exportación Full";
            else if (preGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 4))
                return "Export. Break Bulk";
            else if (preGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 5))
                return "Exportación CFS";
            else if (preGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 6))
                return "Desestimiento ExpoF";
            else if (preGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 7))
                return "Recepción Vacíos";
            else if (preGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 8))
                return "Exportación Banano";
            else if (preGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 9))
                return "Export. Banano CFS";
            else if (preGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 10))
                return "Recepción Vacíos Mix";
            else if (preGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 11))
                return "Entrega Vacíos";
            else if (preGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 16))
                return "Entrega Vacíos SAV";//FARBEN
            else if (preGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 17))
                return "OPACIFIC ZAL II";//FARBEN
            else if (preGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 18))
                return "P2D";//P2D
            else if (preGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 19))
                return "Recepción Full Mix Depot";
            else
                return "Desconocido";
        }

        internal override void CambiarEstado()
        {
            ViewModel.SetearEstado(new PaginaBiometricoViewModel());
        }
        #endregion
    }

    internal class PaginaSalidaInicioViewModel : PaginaInicioViewModel
    {
        #region Campos
        #endregion

        #region Constructor
        internal PaginaSalidaInicioViewModel()
        {
        }
        #endregion

        #region Propiedades
        #endregion

        #region Metodos
        protected override void EstablecerTituloYMensaje()
        {
            Titulo = "SALIDA DE VEHICULO PESADO";
            Mensaje = Resources.MensajeInicioSalida;
        }

        protected override void Limpiar()
        {
            Numero = "";
            Mensaje = Resources.MensajeInicioSalida;
            ColorTextoMensaje = (Brush)ViewModel.Convertidor.ConvertFromString("#191007");
            ViewModel.LeerSinLoop = false;
        }

        protected override void IniciarHilo(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (!ViewModel.Input1)
                ViewModel.LeerSinLoop = true;
            Worker.ReportProgress(0);
            var sensoresKiosco = new Dictionary<short, bool>
            {
                { 1, ViewModel.Input1 },
                { 2, ViewModel.Input2 },
                { 3, ViewModel.Input3 },
                { 4, ViewModel.Input4 }
            };
            ViewModel.DatosPreGateSalida = ViewModel.Servicio.ValidarEntradaQuiosco(Convert.ToInt64(Numero), ViewModel.Quiosco.KIOSK_ID, sensoresKiosco);
        }

        protected override void MostrarResultadoEnPantalla()
        {
            var id = Convert.ToInt32(ViewModel.DatosPreGateSalida.Mensaje);
            Mensaje = ViewModel.Mensajes.FirstOrDefault(m => m.MESSAGE_ID == id).USER_MESSAGE;
            ColorTextoMensaje = ViewModel.DatosPreGateSalida.FueOk ? (Brush)ViewModel.Convertidor.ConvertFromString("#191007") : Brushes.Red;
            if (ViewModel.DatosPreGateSalida.FueOk)
            {
                ViewModel.TipoTransaccion = ObtenerTipoTransaccion(ViewModel.DatosPreGateSalida.PreGate);
                if ((ViewModel.DatosPreGateSalida.PreGate.BY_PASS == null || !ViewModel.DatosPreGateSalida.PreGate.BY_PASS.IS_ENABLED) && ConfigurationManager.AppSettings["ValidarSensorAltura"].ToString() == "1" && (ViewModel.DatosPreGateSalida.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 1) || ViewModel.DatosPreGateSalida.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 3) || ViewModel.DatosPreGateSalida.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 7) || ViewModel.DatosPreGateSalida.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 10) || ViewModel.DatosPreGateSalida.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 11)))
                {
                    var cadenaResponsabilidadSensores = new DeliveryImportSalidaValidarSensores();
                    var resultado = cadenaResponsabilidadSensores.Procesar(ViewModel.DatosPreGateSalida.PreGate, ViewModel);
                    if (resultado.Item1)
                        Dispatcher.Start();
                    else
                    {
                        ViewModel.Servicio.RegistrarProceso(new KIOSK_TRANSACTION
                        {
                            TRANSACTION_ID = Convert.ToInt32(ViewModel.DatosPreGateSalida.IdTransaccion),
                            PRE_GATE_ID = ViewModel.DatosPreGateSalida.PreGate.PRE_GATE_ID,
                            PROCESSES = new List<PROCESS> { new PROCESS
                            {
                                STEP = "SENSOR_ALTURA",
                                IS_OK = false,
                                RESPONSE = "",
                                MESSAGE_ID = resultado.Item2
                            } },
                            KIOSK = ViewModel.Quiosco
                        });
                        ColorTextoMensaje = Brushes.Red;
                        ViewModel.ServicioAnuncianteProblema.AnunciarProblema(Convert.ToInt32(ViewModel.DatosPreGateSalida.IdTransaccion));
                        Mensaje = ViewModel.Mensajes.FirstOrDefault(m => m.MESSAGE_ID == resultado.Item2).USER_MESSAGE;
                        BotonPresionado = false;
                    }
                }
                else
                    Dispatcher.Start();
            }
            else
            {
                var mensaje = ViewModel.Mensajes.FirstOrDefault(m => m.MESSAGE_ID == id);
                mensaje.ContadorIntentos++;
                if (mensaje.ATTEMPTS == mensaje.ContadorIntentos)
                {
                    if (mensaje.ATTEMPTS != 1)
                    {
                        id = Convert.ToInt32(Resources.IdMensajeGeneralEnvioProblema);
                        Mensaje = ViewModel.Mensajes.FirstOrDefault(m => m.MESSAGE_ID == id).USER_MESSAGE;
                    }
                    ViewModel.ServicioAnuncianteProblema.AnunciarProblema(Convert.ToInt32(ViewModel.DatosPreGateSalida.IdTransaccion));
                }
                BotonPresionado = false;
            }
        }

        internal override void CambiarEstado()
        {
            ViewModel.SetearEstado(new PaginaSalidaBiometricoViewModel());
        }
        #endregion
    }

    internal abstract class CadenaResponsabilidadSensores
    {
        protected CadenaResponsabilidadSensores Siguiente { get; set; }

        protected void EstablecerSiguiente(CadenaResponsabilidadSensores siguiente)
        {
            Siguiente = siguiente;
        }

        internal abstract Tuple<bool, int> Procesar(PRE_GATE pregate, VentanaPrincipalViewModel viewModel);
    }

    internal class DeliveryImportEntradaValidarSensores : CadenaResponsabilidadSensores
    {
        internal override Tuple<bool, int> Procesar(PRE_GATE pregate, VentanaPrincipalViewModel viewModel)
        {
            if (pregate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 1))
                return viewModel.Input2 ? new Tuple<bool, int>(false, 27) : new Tuple<bool, int>(true, 0);
            else
            {
                EstablecerSiguiente(new ReceiveExportEntradaValidarSensores());
                return Siguiente.Procesar(pregate, viewModel);
            }
        }
    }

    internal class ReceiveExportEntradaValidarSensores : CadenaResponsabilidadSensores
    {
        internal override Tuple<bool, int> Procesar(PRE_GATE pregate, VentanaPrincipalViewModel viewModel)
        {
            if (pregate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 3))
                return viewModel.Input2 ? new Tuple<bool, int>(true, 0) : new Tuple<bool, int>(false, 28);
            else
            {
                EstablecerSiguiente(new ReceiveExportEmptyEntradaValidarSensores());
                return Siguiente.Procesar(pregate, viewModel);
            }
        }
    }

    internal class ReceiveExportEmptyEntradaValidarSensores : CadenaResponsabilidadSensores
    {
        internal override Tuple<bool, int> Procesar(PRE_GATE pregate, VentanaPrincipalViewModel viewModel)
        {
            if (pregate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 7) || pregate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 10))
                return viewModel.Input2 ? new Tuple<bool, int>(true, 0) : new Tuple<bool, int>(false, 41);
            else
            {
                EstablecerSiguiente(new DeliveryImportEmptyEntradaValidarSensores());
                return Siguiente.Procesar(pregate, viewModel);
            }
        }
    }

    internal class DeliveryImportEmptyEntradaValidarSensores : CadenaResponsabilidadSensores
    {
        internal override Tuple<bool, int> Procesar(PRE_GATE pregate, VentanaPrincipalViewModel viewModel)
        {
            if (pregate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 11))
                return viewModel.Input2 ? new Tuple<bool, int>(false, 42) : new Tuple<bool, int>(true, 0);
            return new Tuple<bool, int>(false, 24);
        }
    }

    internal class DeliveryImportSalidaValidarSensores : CadenaResponsabilidadSensores
    {
        internal override Tuple<bool, int> Procesar(PRE_GATE pregate, VentanaPrincipalViewModel viewModel)
        {
            if (pregate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 1))
                return viewModel.Input2 ? new Tuple<bool, int>(true, 0) : new Tuple<bool, int>(false, 29);
            else
            {
                EstablecerSiguiente(new ReceiveExportSalidaValidarSensores());
                return Siguiente.Procesar(pregate, viewModel);
            }
        }
    }

    internal class ReceiveExportSalidaValidarSensores : CadenaResponsabilidadSensores
    {
        internal override Tuple<bool, int> Procesar(PRE_GATE pregate, VentanaPrincipalViewModel viewModel)
        {
            if (pregate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 3))
                return viewModel.Input2 ? new Tuple<bool, int>(false, 30) : new Tuple<bool, int>(true, 0);
            else
            {
                EstablecerSiguiente(new ReceiveExportEmptySalidaValidarSensores());
                return Siguiente.Procesar(pregate, viewModel);
            }
        }
    }

    internal class ReceiveExportEmptySalidaValidarSensores : CadenaResponsabilidadSensores
    {
        internal override Tuple<bool, int> Procesar(PRE_GATE pregate, VentanaPrincipalViewModel viewModel)
        {
            if (pregate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 7) || pregate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 10))
                return viewModel.Input2 ? new Tuple<bool, int>(false, 43) : new Tuple<bool, int>(true, 0);
            else
            {
                EstablecerSiguiente(new DeliveryImportEmptySalidaValidarSensores());
                return Siguiente.Procesar(pregate, viewModel);
            }
        }
    }

    internal class DeliveryImportEmptySalidaValidarSensores : CadenaResponsabilidadSensores
    {
        internal override Tuple<bool, int> Procesar(PRE_GATE pregate, VentanaPrincipalViewModel viewModel)
        {
            if (pregate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 11))
                return viewModel.Input2 ? new Tuple<bool, int>(true, 0) : new Tuple<bool, int>(false, 44);
            return new Tuple<bool, int>(false, 24);
        }
    }
}

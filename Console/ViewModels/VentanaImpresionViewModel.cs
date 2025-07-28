using Console.ServicioConsole;
using Console.ServicioTransactionQuiosco;
using Console.Vistas;
using RECEPTIO.CapaPresentacion.UWP.MVVM;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using KIOSK = Console.ServicioConsole.KIOSK;
using PRE_GATE = Console.ServicioConsole.PRE_GATE;

namespace Console.ViewModels
{
    internal class VentanaImpresionViewModel : Base
    {
        #region Variables
        private readonly Page _pagina;
        private ServicioConsoleClient _servicio;
        private ServicioTransactionQuiosco.ContratoClient _servicioQuiosco;
        private RelayCommand _comandoRegresar;
        private RelayCommand _comandoConsultar;
        private RelayCommand _comandoLimpiar;
        private RelayCommand _comandoImprimir;
        private long _idPreGate;
        private bool _esEntrada;
        private bool _estaHabilitadoRadioButtons;
        private ObservableCollection<KIOSK> _todosLosQuioscos;
        private ObservableCollection<KIOSK> _quioscos;
        private KIOSK _quioscoSeleccionado;
        private PRE_GATE _preGate;
        #endregion

        #region Constructor
        internal VentanaImpresionViewModel(Page pagina)
        {
            _pagina = pagina;
            PropertyChanged += (s, e) => DelegarEventosCambiosAEventoComando();
            InstanciarComandos();
            InicializarServicioConsole();
            CargarQuioscosAsync();
            EstaHabilitadoRadioButtons = true;
        }

        private void DelegarEventosCambiosAEventoComando()
        {
            _comandoConsultar.RaiseCanExecuteChanged();
            _comandoLimpiar.RaiseCanExecuteChanged();
            _comandoImprimir.RaiseCanExecuteChanged();
        }

        private void InstanciarComandos()
        {
            _comandoRegresar = new RelayCommand(Regresar);
            _comandoConsultar = new RelayCommand(ConsultarAsync, PuedoConsultar);
            _comandoLimpiar = new RelayCommand(Limpiar, PuedoLimpiar);
            _comandoImprimir = new RelayCommand(ImprimirAsync, PuedoImprimir);
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

        public RelayCommand ComandoConsultar
        {
            get
            {
                return _comandoConsultar;
            }
            set
            {
                SetProperty(ref _comandoConsultar, value);
            }
        }

        public RelayCommand ComandoLimpiar
        {
            get
            {
                return _comandoLimpiar;
            }
            set
            {
                SetProperty(ref _comandoLimpiar, value);
            }
        }

        public RelayCommand ComandoImprimir
        {
            get
            {
                return _comandoImprimir;
            }
            set
            {
                SetProperty(ref _comandoImprimir, value);
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

        public bool EsEntrada
        {
            get
            {
                return _esEntrada;
            }
            set
            {
                _esEntrada = value;
                RaisePropertyChanged("EsEntrada");
                RaisePropertyChanged("EsSalida");
                FiltrarQuioscos();
            }
        }

        public bool EsSalida
        {
            get
            {
                return !_esEntrada;
            }
            set
            {
                _esEntrada = !value;
                RaisePropertyChanged("EsSalida");
            }
        }

        private void FiltrarQuioscos()
        {
            Quioscos = new ObservableCollection<KIOSK>();
            foreach (var item in _todosLosQuioscos.Where(q => q.IS_IN == EsEntrada))
                Quioscos.Add(item);
        }

        public bool EstaHabilitadoRadioButtons
        {
            get
            {
                return _estaHabilitadoRadioButtons;
            }
            set
            {
                if (_estaHabilitadoRadioButtons == value)
                    return;
                _estaHabilitadoRadioButtons = value;
                RaisePropertyChanged("EstaHabilitadoRadioButtons");
            }
        }

        public ObservableCollection<KIOSK> Quioscos
        {
            get
            {
                return _quioscos;
            }
            set
            {
                SetProperty(ref _quioscos, value);
            }
        }

        public KIOSK QuioscoSeleccionado
        {
            get
            {
                return _quioscoSeleccionado;
            }
            set
            {
                SetProperty(ref _quioscoSeleccionado, value);
            }
        }
        #endregion

        #region Metodos
        private void InicializarServicioConsole()
        {
            _servicio = (ServicioConsoleClient)App.Current.Resources["ServicioConsole"];
        }

        private void Regresar(object obj)
        {
            _pagina.Frame.Navigate(typeof(VentanaSupervisor));
        }

        private async void CargarQuioscosAsync()
        {
            _todosLosQuioscos = await _servicio.ObtenerKioscosActivosAsync();
            EsEntrada = true;
        }

        private bool PuedoConsultar(object obj)
        {
            return _idPreGate > 0;
        }

        private async void ConsultarAsync(object obj)
        {
            var resultado = await _servicio.ObtenerInformacionParaReimpresionTicketAsync(IdPreGate, EsEntrada);
            if (resultado.Item1)
            {
                EstaHabilitadoRadioButtons = false;
                _preGate = resultado.Item3;
                var mensajeDialogo = new MessageDialog("El ID es correcto. Ahora seleccione un quiosco y luego presione imprimir.", "ReImpresion Ticket");
                await mensajeDialogo.ShowAsync();
            }
            else
            {
                var mensajeDialogo = new MessageDialog(resultado.Item2, "Reimpresion Ticket");
                await mensajeDialogo.ShowAsync();
            }
        }

        private bool PuedoLimpiar(object obj)
        {
            return !_estaHabilitadoRadioButtons;
        }

        private void Limpiar(object obj)
        {
            EstaHabilitadoRadioButtons = true;
            _preGate = null;
            IdPreGate = 0;
        }

        private bool PuedoImprimir(object obj)
        {
            return _quioscoSeleccionado != null && !_estaHabilitadoRadioButtons;
        }

        private async void ImprimirAsync(object obj)
        {
            var resultado = QuioscoTieneConeccion(QuioscoSeleccionado.IP);
            if (resultado.Item1)
            {
                _servicioQuiosco = resultado.Item2;
                var xml = _preGate.KIOSK_TRANSACTIONS.FirstOrDefault(kt => kt.KIOSK.IS_IN == EsEntrada && kt.PROCESSES.Any(p => p.IS_OK && p.STEP == "PROCESO_N4")).PROCESSES.FirstOrDefault(p => p.IS_OK && p.STEP == "PROCESO_N4").RESPONSE;
                
                if (xml == null)
                {
                    var v_mensaje = new MessageDialog("Reimpresión de ticket no procesada, transacción de entrada con novedad.", "ReImpresion Ticket");
                    await v_mensaje.ShowAsync();
                    return;
                }

                var resultadoImpresion = await _servicioQuiosco.ReimprimirCualquierTicketAsync(ConversionPreGate(_preGate), xml);
                var mensajeDialogo = new MessageDialog(resultadoImpresion.Item2, $"Reimpresion Ticket");
                await mensajeDialogo.ShowAsync();
                await _servicio.RegistrarReimpresionAsync(new ServicioConsole.REPRINT
                {
                    KIOSK_ID = QuioscoSeleccionado.KIOSK_ID,
                    PRE_GATE_ID = _preGate.PRE_GATE_ID,
                    TTU_ID = ((DatosLogin)App.Current.Resources["DatosLogin"]).IdUsuario
                });
                Limpiar(null);
            }
            else
            {
                var mensajeDialogo = new MessageDialog($"El {QuioscoSeleccionado.NAME} cuya ip es {QuioscoSeleccionado.IP} está sin conección.", "Kiosco Sin Conección.");
                await mensajeDialogo.ShowAsync();
            }
        }

        private ServicioTransactionQuiosco.PRE_GATE ConversionPreGate(PRE_GATE preGate)
        {
            var resultado = new ServicioTransactionQuiosco.PRE_GATE
            {
                PRE_GATE_ID = preGate.PRE_GATE_ID,
                CREATION_DATE = preGate.CREATION_DATE,
                DEVICE_ID = preGate.DEVICE_ID,
                DRIVER_ID = preGate.DRIVER_ID,
                STATUS = preGate.STATUS,
                TRUCK_LICENCE = preGate.TRUCK_LICENCE,
                USER = preGate.USER,
                KIOSK_TRANSACTIONS = new ObservableCollection<ServicioTransactionQuiosco.KIOSK_TRANSACTION>(),
                PRE_GATE_DETAILS = new ObservableCollection<ServicioTransactionQuiosco.PRE_GATE_DETAIL>()
            };
            foreach (var item in preGate.PRE_GATE_DETAILS)
            {
                var detalle = new ServicioTransactionQuiosco.PRE_GATE_DETAIL
                {
                    DOCUMENT_ID = item.DOCUMENT_ID,
                    PRE_GATE_ID = item.PRE_GATE_ID,
                    REFERENCE_ID = item.REFERENCE_ID,
                    STATUS = item.STATUS,
                    TRANSACTION_NUMBER = item.TRANSACTION_NUMBER,
                    TRANSACTION_TYPE_ID = item.TRANSACTION_TYPE_ID,
                    ID = item.ID
                };
                detalle.CONTAINERS = new ObservableCollection<ServicioTransactionQuiosco.CONTAINER>();
                foreach (var cont in item.CONTAINERS)
                    detalle.CONTAINERS.Add(new ServicioTransactionQuiosco.CONTAINER { NUMBER = cont.NUMBER });
                resultado.PRE_GATE_DETAILS.Add(detalle);
            }
            foreach (var item in preGate.KIOSK_TRANSACTIONS)
            {
                resultado.KIOSK_TRANSACTIONS.Add(new ServicioTransactionQuiosco.KIOSK_TRANSACTION
                {
                    END_DATE = item.END_DATE,
                    IS_OK = item.IS_OK,
                    KIOSK = new ServicioTransactionQuiosco.KIOSK
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

        #endregion
    }
}

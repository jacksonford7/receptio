using Console.ServicioConsole;
using Console.Vistas;
using RECEPTIO.CapaPresentacion.UWP.MVVM;
using System;
using System.Collections.ObjectModel;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace Console.ViewModels
{
    internal class VentanaKioscosViewModel : Base
    {
        #region Variables
        private readonly Page _pagina;
        private ServicioConsoleClient _servicio;
        private ServicioTransactionQuiosco.ContratoClient _servicioQuiosco;
        private RelayCommand _comandoRegresar;
        private RelayCommand _comandoAbrirBarrera;
        private ObservableCollection<KIOSK> _quioscos;
        private KIOSK _quioscoSeleccionado;
        #endregion

        #region Constructor
        internal VentanaKioscosViewModel(Page pagina)
        {
            _pagina = pagina;
            PropertyChanged += (s, e) => DelegarEventosCambiosAEventoComando();
            InstanciarComandos();
            InicializarServicioConsole();
            CargarQuioscos();
        }

        private void DelegarEventosCambiosAEventoComando()
        {
            _comandoAbrirBarrera.RaiseCanExecuteChanged();
        }

        private void InstanciarComandos()
        {
            _comandoRegresar = new RelayCommand(Regresar);
            _comandoAbrirBarrera = new RelayCommand(AbrirBarrera, PuedoAbrirBarrera);
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

        private async void CargarQuioscos()
        {
            Quioscos = await _servicio.ObtenerKioscosActivosAsync();
        }

        private bool PuedoAbrirBarrera(object obj)
        {
            return QuioscoSeleccionado != null;
        }

        private async void AbrirBarrera(object obj)
        {
            var resultado = QuioscoTieneConeccion(QuioscoSeleccionado.IP);
            if (resultado.Item1)
            {
                _servicioQuiosco = resultado.Item2;
                var ventana = new VentanaAutorizacion { ViewModel = new VentanaAutorizacionViewModel(_servicio, $"Abrir Barrera {QuioscoSeleccionado.NAME}") };
                await ventana.ShowAsync();
                if (ventana.ViewModel.Resultado == null)
                    return;
                if (ventana.ViewModel.Resultado.Item1)
                {
                    await _servicioQuiosco.AbrirBarreraLiderAsync();
                    await _servicio.RegistrarAperturaBarreraAsync(new LIFT_UP_BARRIER { KIOSK_ID = QuioscoSeleccionado.KIOSK_ID, REASON = ventana.ViewModel.Motivo, PREGATE_ID = ventana.ViewModel.IdPreGate, MOTIVE_ID = ventana.ViewModel.MotivoSeleccionado.MOTIVE_ID , TTU_ID = ((DatosLogin)App.Current.Resources["DatosLogin"]).IdUsuario });
                    var mensajeDialogo = new MessageDialog("Barrera Abierta", "Barrera Abierta");
                    await mensajeDialogo.ShowAsync();
                }
                else
                {
                    if (ventana.ViewModel.Resultado.Item2 != "")
                    {
                        var mensajeDialogo = new MessageDialog(ventana.ViewModel.Resultado.Item2, "Autorización Fallida");
                        await mensajeDialogo.ShowAsync();
                    }
                }
            }
            else
            {
                var mensajeDialogo = new MessageDialog($"El {QuioscoSeleccionado.NAME} cuya ip es {QuioscoSeleccionado.IP} está sin conección.", "Kiosco Sin Conección.");
                await mensajeDialogo.ShowAsync();
            }
        }
        #endregion
    }
}

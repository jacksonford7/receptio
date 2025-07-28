using Console.ServicioConsole;
using Console.Vistas;
using RECEPTIO.CapaPresentacion.UWP.MVVM;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Console.ViewModels
{
    internal class VentanaDescansoViewModel : Base
    {
        #region Variables
        private readonly Page _ventana;
        private readonly int _idDescanso;
        private ServicioConsoleClient _servicio;
        private RelayCommand _comandoTerminarDescanso;
        private DispatcherTimer _dispatcher;
        private string _horas;
        private string _minutos;
        private string _segundos;
        private DateTime _fechaInicio;
        #endregion

        #region Constructor
        internal VentanaDescansoViewModel(Page ventana, int idDescanso)
        {
            _ventana = ventana;
            _idDescanso = idDescanso;
            _fechaInicio = DateTime.Now;
            PropertyChanged += (s, e) => _comandoTerminarDescanso.RaiseCanExecuteChanged();
            InstanciarComandos();
            InicializarServicioConsole();
            InstanciarDispatcher();
        }

        private void InstanciarComandos()
        {
            _comandoTerminarDescanso = new RelayCommand(TerminarDescanso);
        }

        private void InicializarServicioConsole()
        {
            _servicio = (ServicioConsoleClient)App.Current.Resources["ServicioConsole"];
        }

        private void InstanciarDispatcher()
        {
            _dispatcher = new DispatcherTimer { Interval = new TimeSpan(0, 0, 1) };
            _dispatcher.Tick += Temporizador;
            _dispatcher.Start();
        }
        #endregion

        #region Propiedades
        public RelayCommand ComandoTerminarDescanso
        {
            get
            {
                return _comandoTerminarDescanso;
            }
            set
            {
                SetProperty(ref _comandoTerminarDescanso, value);
            }
        }

        public string Horas
        {
            get
            {
                return _horas;
            }
            set
            {
                if (_horas == value)
                    return;
                _horas = value;
                RaisePropertyChanged("Horas");
            }
        }

        public string Minutos
        {
            get
            {
                return _minutos;
            }
            set
            {
                if (_minutos == value)
                    return;
                _minutos = value;
                RaisePropertyChanged("Minutos");
            }
        }

        public string Segundos
        {
            get
            {
                return _segundos;
            }
            set
            {
                if (_segundos == value)
                    return;
                _segundos = value;
                RaisePropertyChanged("Segundos");
            }
        }
        #endregion

        #region Metodos
        private void Temporizador(object sender, object e)
        {
            var tiempo = (DateTime.Now - _fechaInicio).Duration();
            Horas = tiempo.Hours.ToString("00");
            Minutos = tiempo.Minutes.ToString("00");
            Segundos = tiempo.Seconds.ToString("00");
        }

        private async void TerminarDescanso(object obj)
        {
            if (BotonPresionado)
                return;
            BotonPresionado = true;
            await _servicio.FinalizarDescansoAsync(_idDescanso);
            _ventana.Frame.Navigate(typeof(VentanaPrincipal));
            BotonPresionado = false;
        }
        #endregion
    }
}

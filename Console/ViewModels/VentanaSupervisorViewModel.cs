using Console.ServicioConsole;
using Console.Vistas;
using RECEPTIO.CapaPresentacion.UWP.MVVM;
using System;

namespace Console.ViewModels
{
    internal class VentanaSupervisorViewModel : Base
    {
        #region Variables
        private readonly VentanaSupervisor _ventanaSupervisor;
        private ServicioConsoleClient _servicio;
        private RelayCommand _comandoTicketsNoAsignados;
        private RelayCommand _comandoTicketsSuspendidos;
        private RelayCommand _comandoReporteTransacciones;
        private RelayCommand _comandoReporteTickets;
        private RelayCommand _comandoQuioscos;
        private RelayCommand _comandoSesionesAbiertas;
        private RelayCommand _comandoReimpresion;
        private RelayCommand _comandoByPass;
        private RelayCommand _comandoPregateCancel;
        private RelayCommand _comandoCerrarSesion;
        #endregion

        #region Constructor
        internal VentanaSupervisorViewModel(VentanaSupervisor ventanaSupervisor)
        {
            _ventanaSupervisor = ventanaSupervisor;
            InstanciarComandos();
            InicializarServicioConsole();
        }

        private void InstanciarComandos()
        {
            _comandoTicketsNoAsignados = new RelayCommand(VerTicketsNoAsignados);
            _comandoTicketsSuspendidos = new RelayCommand(VerTicketsSuspendidos);
            _comandoReporteTransacciones = new RelayCommand(ReporteTransacciones);
            _comandoReporteTickets = new RelayCommand(ReporteTickets);
            _comandoQuioscos = new RelayCommand(Quioscos);
            _comandoSesionesAbiertas = new RelayCommand(SesionesAbiertas);
            _comandoReimpresion = new RelayCommand(Reimpresion);
            _comandoByPass = new RelayCommand(ByPass);
            _comandoPregateCancel = new RelayCommand(PregateCancel);
            _comandoCerrarSesion = new RelayCommand(CerrarSesion);
        }
        #endregion

        #region Propiedades
        public RelayCommand ComandoTicketsNoAsignados
        {
            get
            {
                return _comandoTicketsNoAsignados;
            }
            set
            {
                SetProperty(ref _comandoTicketsNoAsignados, value);
            }
        }

        public RelayCommand ComandoTicketsSuspendidos
        {
            get
            {
                return _comandoTicketsSuspendidos;
            }
            set
            {
                SetProperty(ref _comandoTicketsSuspendidos, value);
            }
        }

        public RelayCommand ComandoReporteTransacciones
        {
            get
            {
                return _comandoReporteTransacciones;
            }
            set
            {
                SetProperty(ref _comandoReporteTransacciones, value);
            }
        }

        public RelayCommand ComandoReporteTickets
        {
            get
            {
                return _comandoReporteTickets;
            }
            set
            {
                SetProperty(ref _comandoReporteTickets, value);
            }
        }

        public RelayCommand ComandoQuioscos
        {
            get
            {
                return _comandoQuioscos;
            }
            set
            {
                SetProperty(ref _comandoQuioscos, value);
            }
        }

        public RelayCommand ComandoSesionesAbiertas
        {
            get
            {
                return _comandoSesionesAbiertas;
            }
            set
            {
                SetProperty(ref _comandoSesionesAbiertas, value);
            }
        }

        public RelayCommand ComandoReimpresion
        {
            get
            {
                return _comandoReimpresion;
            }
            set
            {
                SetProperty(ref _comandoReimpresion, value);
            }
        }

        public RelayCommand ComandoByPass
        {
            get
            {
                return _comandoByPass;
            }
            set
            {
                SetProperty(ref _comandoByPass, value);
            }
        }

        public RelayCommand ComandoPregateCancel
        {
            get
            {
                return _comandoPregateCancel;
            }
            set
            {
                SetProperty(ref _comandoPregateCancel, value);
            }
        }
        public RelayCommand ComandoCerrarSesion
        {
            get
            {
                return _comandoCerrarSesion;
            }
            set
            {
                SetProperty(ref _comandoCerrarSesion, value);
            }
        }
        #endregion

        #region Metodos
        private void InicializarServicioConsole()
        {
            _servicio = (ServicioConsoleClient)App.Current.Resources["ServicioConsole"];
        }

        private void VerTicketsNoAsignados(object obj)
        {
            _ventanaSupervisor.Frame.Navigate(typeof(VentanaTicketsNoAsignados), 0);
        }

        private void VerTicketsSuspendidos(object obj)
        {
            _ventanaSupervisor.Frame.Navigate(typeof(VentanaTicketsNoAsignados), 1);
        }

        private void ReporteTransacciones(object obj)
        {
            _ventanaSupervisor.Frame.Navigate(typeof(VentanaReporteTransacciones));
        }

        private void ReporteTickets(object obj)
        {
            _ventanaSupervisor.Frame.Navigate(typeof(VentanaReporteTickets));
        }

        private void Quioscos(object obj)
        {
            _ventanaSupervisor.Frame.Navigate(typeof(VentanaKioscos));
        }

        private void SesionesAbiertas(object obj)
        {
            _ventanaSupervisor.Frame.Navigate(typeof(VentanaSesionesUsuarios));
        }

        private void Reimpresion(object obj)
        {
            _ventanaSupervisor.Frame.Navigate(typeof(VentanaImpresion));
        }

        private async void ByPass(object obj)
        {
            var ventana = new VentanaByPass { ViewModel = new VentanaByPassViewModel() };
            await ventana.ShowAsync();
        }

        private async void PregateCancel(object obj)
        {
            var ventana = new VentanaPreGateCancel { ViewModel = new VentanaPreGateCancelViewModel() };
            await ventana.ShowAsync();
        }

        private async void CerrarSesion(object obj)
        {
            await _servicio.CerrarSesionAsync(((DatosLogin)App.Current.Resources["DatosLogin"]).IdSesion);
            _ventanaSupervisor.Frame.Navigate(typeof(VentanaAutenticacion));
            App.Current.Resources.Remove("DatosLogin");
        }
        #endregion
    }
}

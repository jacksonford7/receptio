using Console.ServicioConsole;
using Console.Vistas;
using RECEPTIO.CapaPresentacion.UWP.MVVM;
using System;
using System.Collections.ObjectModel;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace Console.ViewModels
{
    internal class VentanaSesionesUsuariosViewModel : Base
    {
        #region Variables
        private readonly Page _pagina;
        private ServicioConsoleClient _servicio;
        private RelayCommand _comandoRegresar;
        private RelayCommand _comandoCerrarSesion;
        private ObservableCollection<USER_SESSION> _sesionesUsuarios;
        private USER_SESSION _sesionUsuarioSeleccionada;
        #endregion

        #region Constructor
        internal VentanaSesionesUsuariosViewModel(Page pagina)
        {
            _pagina = pagina;
            PropertyChanged += (s, e) => DelegarEventosCambiosAEventoComando();
            InstanciarComandos();
            InicializarServicioConsole();
            CargarSesionesUsuariosAsync();
        }

        private void DelegarEventosCambiosAEventoComando()
        {
            _comandoCerrarSesion.RaiseCanExecuteChanged();
        }

        private void InstanciarComandos()
        {
            _comandoRegresar = new RelayCommand(Regresar);
            _comandoCerrarSesion = new RelayCommand(CerrarSesionAsync, PuedoCerrarSesion);
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

        public ObservableCollection<USER_SESSION> SesionesUsuarios
        {
            get
            {
                return _sesionesUsuarios;
            }
            set
            {
                SetProperty(ref _sesionesUsuarios, value);
            }
        }

        public USER_SESSION SesionUsuarioSeleccionada
        {
            get
            {
                return _sesionUsuarioSeleccionada;
            }
            set
            {
                SetProperty(ref _sesionUsuarioSeleccionada, value);
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

        private async void CargarSesionesUsuariosAsync()
        {
            SesionesUsuarios = await _servicio.ObtenerSesionesUsuariosAsync();
        }

        private bool PuedoCerrarSesion(object obj)
        {
            return SesionUsuarioSeleccionada != null;
        }

        private async void CerrarSesionAsync(object obj)
        {
            var mensajeDialogo = new MessageDialog($"¿Está seguro de cerrar la sesión de {SesionUsuarioSeleccionada.TROUBLE_DESK_USER.USER_NAME}?", $"Cerrar Sesión Usuario {SesionUsuarioSeleccionada.TROUBLE_DESK_USER.USER_NAME}");
            mensajeDialogo.Commands.Add(new UICommand("Sí", new UICommandInvokedHandler(RealizarAccionCerrarSesionAsync)));
            mensajeDialogo.Commands.Add(new UICommand("No"));
            mensajeDialogo.DefaultCommandIndex = 1;
            mensajeDialogo.CancelCommandIndex = 1;
            await mensajeDialogo.ShowAsync();
        }

        private async void RealizarAccionCerrarSesionAsync(IUICommand command)
        {
            await _servicio.CerrarSesionAsync(SesionUsuarioSeleccionada.ID);
            var mensajeDialogo = new MessageDialog("Proceso Ok.", $"Cerrar Sesión Usuario {SesionUsuarioSeleccionada.TROUBLE_DESK_USER.USER_NAME}");
            await mensajeDialogo.ShowAsync();
            CargarSesionesUsuariosAsync();
        }
        #endregion
    }
}

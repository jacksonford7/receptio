using Console.ServicioConsole;
using RECEPTIO.CapaPresentacion.UWP.MVVM;
using System.Collections.ObjectModel;
using System.Linq;

namespace Console.ViewModels
{
    internal class VentanaTiposDescansosViewModel : NotificationBase
    {
        #region Variables
        private readonly ServicioConsoleClient _servicio;
        private ObservableCollection<BREAK_TYPE> _tiposDescansos;
        private RelayCommand _comandoAceptar;
        private BREAK_TYPE _tipoDescansoSeleccionado;
        internal int IdDescanso;
        #endregion

        #region Constructor
        internal VentanaTiposDescansosViewModel(ServicioConsoleClient servicio)
        {
            _servicio = servicio;
            _comandoAceptar = new RelayCommand(Aceptar);
            PropertyChanged += (s, e) => _comandoAceptar.RaiseCanExecuteChanged();
            ObtenerTiposDescansos();
        }
        #endregion

        #region Propiedades
        public RelayCommand ComandoAceptar
        {
            get
            {
                return _comandoAceptar;
            }
            set
            {
                SetProperty(ref _comandoAceptar, value);
            }
        }

        public ObservableCollection<BREAK_TYPE> TiposDescansos
        {
            get
            {
                return _tiposDescansos;
            }
            set
            {
                SetProperty(ref _tiposDescansos, value);
            }
        }

        public BREAK_TYPE TipoDescansoSeleccionado
        {
            get
            {
                return _tipoDescansoSeleccionado;
            }
            set
            {
                SetProperty(ref _tipoDescansoSeleccionado, value);
            }
        }
        #endregion

        #region Metodos
        private async void ObtenerTiposDescansos()
        {
            TiposDescansos = await _servicio.ObtenerTiposDescansosAsync();
            TipoDescansoSeleccionado = TiposDescansos.FirstOrDefault();
        }

        public async void Aceptar(object obj)
        {
            if (BotonPresionado)
                return;
            BotonPresionado = true;
            IdDescanso = await _servicio.RegistrarDescansoAsync(new BREAK { BREAK_TYPE_ID = TipoDescansoSeleccionado.ID, USER_SESSION_ID = ((DatosLogin)App.Current.Resources["DatosLogin"]).IdSesion } );
            BotonPresionado = false;
        }
        #endregion
    }
}

using Console.ServicioConsole;
using RECEPTIO.CapaPresentacion.UWP.MVVM;
using System.Collections.ObjectModel;
using System.Linq;

namespace Console.ViewModels
{
    internal class VentanaMotivosAutoTicketViewModel : NotificationBase
    {
        #region Variables
        private readonly ServicioConsoleClient _servicio;
        private ObservableCollection<AUTO_TROUBLE_REASON> _motivos;
        private RelayCommand _comandoGenerar;
        private AUTO_TROUBLE_REASON _motivoSeleccionado;
        #endregion

        #region Constructor
        internal VentanaMotivosAutoTicketViewModel(ServicioConsoleClient servicio)
        {
            _servicio = servicio;
            _comandoGenerar = new RelayCommand(Generar);
            PropertyChanged += (s, e) => _comandoGenerar.RaiseCanExecuteChanged();
            ObtenerMotivos();
        }
        #endregion

        #region Propiedades
        public RelayCommand ComandoGenerar
        {
            get
            {
                return _comandoGenerar;
            }
            set
            {
                SetProperty(ref _comandoGenerar, value);
            }
        }

        public ObservableCollection<AUTO_TROUBLE_REASON> Motivos
        {
            get
            {
                return _motivos;
            }
            set
            {
                SetProperty(ref _motivos, value);
            }
        }

        public AUTO_TROUBLE_REASON MotivoSeleccionado
        {
            get
            {
                return _motivoSeleccionado;
            }
            set
            {
                SetProperty(ref _motivoSeleccionado, value);
            }
        }
        #endregion

        #region Metodos
        private async void ObtenerMotivos()
        {
            Motivos = await _servicio.ObtenerMotivosAutoTicketsAsync();
            MotivoSeleccionado = Motivos.FirstOrDefault();
        }

        public async void Generar(object obj)
        {
            if (BotonPresionado)
                return;
            BotonPresionado = true;
            await _servicio.CrearAutoTicketAsync(MotivoSeleccionado.ID, ((DatosLogin)App.Current.Resources["DatosLogin"]).IdSesion);
            BotonPresionado = false;
        }
        #endregion
    }
}

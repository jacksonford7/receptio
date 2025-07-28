using Console.ServicioConsole;
using RECEPTIO.CapaPresentacion.UWP.MVVM;
using System.Collections.ObjectModel;

namespace Console.ViewModels
{
    internal class VentanaAccionTicketViewModel : NotificationBase
    {
        #region Variables
        private readonly ServicioConsoleClient _servicio;
        private ObservableCollection<ACTION> _acciones;
        #endregion

        #region Constructor
        internal VentanaAccionTicketViewModel(ServicioConsoleClient servicio, long idTicket)
        {
            _servicio = servicio;
            ObtenerAcciones(idTicket);
        }
        #endregion

        #region Propiedades
        public ObservableCollection<ACTION> Acciones
        {
            get
            {
                return _acciones;
            }
            set
            {
                SetProperty(ref _acciones, value);
            }
        }
        #endregion

        #region Metodos
        private async void ObtenerAcciones(long idTicket)
        {
            Acciones = await _servicio.ObtenerAccionesTicketAsync(idTicket);
        }
        #endregion
    }
}

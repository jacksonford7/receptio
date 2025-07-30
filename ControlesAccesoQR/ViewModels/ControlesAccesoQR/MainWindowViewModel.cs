using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using ControlesAccesoQR.Views.ControlesAccesoQR;
using ControlesAccesoQR.Models;
using RECEPTIO.CapaPresentacion.UI.MVVM;

namespace ControlesAccesoQR.ViewModels.ControlesAccesoQR
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly Frame _frame;

        private EstadoProceso _estadoProceso = EstadoProceso.EnEspera;
        private EstadoProceso _ultimoEstadoVisible = EstadoProceso.EnEspera;
        private PaseProcesoModel _paseActual;

        public ICommand MostrarEntradaSalidaCommand { get; }
        public ICommand MostrarSalidaFinalCommand { get; }

        public EstadoProceso EstadoProceso
        {
            get => _estadoProceso;
            set
            {
                _estadoProceso = value;
                OnPropertyChanged(nameof(EstadoProceso));
                if (value != EstadoProceso.EnEspera)
                    UltimoEstadoVisible = value;
            }
        }

        public EstadoProceso UltimoEstadoVisible
        {
            get => _ultimoEstadoVisible;
            private set { _ultimoEstadoVisible = value; OnPropertyChanged(nameof(UltimoEstadoVisible)); }
        }

        public PaseProcesoModel PaseActual
        {
            get => _paseActual;
            set { _paseActual = value; OnPropertyChanged(nameof(PaseActual)); }
        }

        public MainWindowViewModel(Frame frame)
        {
            _frame = frame;
            MostrarEntradaSalidaCommand = new RelayCommand(MostrarEntradaSalida);
            MostrarSalidaFinalCommand = new RelayCommand(MostrarSalidaFinal);
        }

        private void MostrarEntradaSalida()
        {
            _frame.Navigate(new VistaEntradaSalida { DataContext = new VistaEntradaSalidaViewModel(this) });
        }

        private void MostrarSalidaFinal()
        {
            _frame.Navigate(new VistaSalidaFinal { DataContext = new VistaSalidaFinalViewModel(this) });
        }

        public async Task ReiniciarDespuesDeSalidaAsync()
        {
            await Task.Delay(5000);
            EstadoProceso = EstadoProceso.EnEspera;
        }
    }
}

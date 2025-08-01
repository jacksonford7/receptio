using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using ControlesAccesoQR.Views.ControlesAccesoQR;
using ControlesAccesoQR.Models;
using EstadoProcesoEnum = ControlesAccesoQR.Models.EstadoProceso;
using RECEPTIO.CapaPresentacion.UI.MVVM;

namespace ControlesAccesoQR.ViewModels.ControlesAccesoQR
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly Frame _frame;

        private EstadoProcesoEnum _estadoProceso = EstadoProcesoEnum.EnEspera;
        private EstadoProcesoEnum _ultimoEstadoVisible = EstadoProcesoEnum.EnEspera;
        private PaseProcesoModel _paseActual;
        public ObservableCollection<Proceso> Procesos { get; } = new ObservableCollection<Proceso>();

        public ICommand MostrarEntradaSalidaCommand { get; }
        public ICommand MostrarSalidaFinalCommand { get; }

        public EstadoProcesoEnum EstadoProceso
        {
            get => _estadoProceso;
            set
            {
                _estadoProceso = value;
                OnPropertyChanged(nameof(EstadoProceso));
                if (value != EstadoProcesoEnum.EnEspera)
                    UltimoEstadoVisible = value;
            }
        }

        public EstadoProcesoEnum UltimoEstadoVisible
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
            EstadoProceso = EstadoProcesoEnum.EnEspera;
        }
    }
}

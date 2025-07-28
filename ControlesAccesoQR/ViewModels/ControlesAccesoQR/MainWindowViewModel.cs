using System.Windows.Controls;
using System.Windows.Input;
using ControlesAccesoQR.Views.ControlesAccesoQR;
using RECEPTIO.CapaPresentacion.UI.MVVM;

namespace ControlesAccesoQR.ViewModels.ControlesAccesoQR
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly Frame _frame;

        public ICommand MostrarEntradaSalidaCommand { get; }
        public ICommand MostrarSalidaFinalCommand { get; }

        public MainWindowViewModel(Frame frame)
        {
            _frame = frame;
            MostrarEntradaSalidaCommand = new RelayCommand(MostrarEntradaSalida);
            MostrarSalidaFinalCommand = new RelayCommand(MostrarSalidaFinal);
        }

        private void MostrarEntradaSalida()
        {
            _frame.Navigate(new VistaEntradaSalida { DataContext = new VistaEntradaSalidaViewModel() });
        }

        private void MostrarSalidaFinal()
        {
            _frame.Navigate(new VistaSalidaFinal { DataContext = new VistaSalidaFinalViewModel() });
        }
    }
}

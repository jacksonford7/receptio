using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ControlesAccesoQR.Models;
using ControlesAccesoQR.Views.ControlesAccesoQR;

using EstadoProcesoEnum = ControlesAccesoQR.Models.EstadoProceso;

using RECEPTIO.CapaPresentacion.UI.MVVM;
using Transaction.ServicioTransaction;
using ServicioComunKioscoClient = Transaction.ServicioTransaction.ServicioTransactionClient;

namespace ControlesAccesoQR.ViewModels.ControlesAccesoQR
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly Frame _frame;

        private EstadoProcesoEnum _estadoProceso = EstadoProcesoEnum.EnEspera;
        private EstadoProcesoEnum _ultimoEstadoVisible = EstadoProcesoEnum.EnEspera;
        private PaseProcesoModel _paseActual;
        private string _numeroKiosco;

        public ObservableCollection<Proceso> Procesos { get; } = new ObservableCollection<Proceso>();

        public ICommand MostrarEntradaSalidaCommand { get; }
        public ICommand MostrarSalidaFinalCommand { get; }

        public string NumeroKiosco
        {
            get => _numeroKiosco;
            private set { _numeroKiosco = value; OnPropertyChanged(nameof(NumeroKiosco)); }
        }

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

            ObtenerQuiosco();
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

        private void ObtenerQuiosco()
        {
            var ipLocal = ConfigurationManager.AppSettings["IP_LOCAL"];

            if (string.IsNullOrWhiteSpace(ipLocal))
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                if (host.AddressList.Length > 1)
                    ipLocal = host.AddressList[1].ToString();
            }

            using (var cliente = new ServicioComunKioscoClient())
            {
                var kiosco = cliente.ObtenerQuiosco(ipLocal);
                if (kiosco == null || !kiosco.IS_ACTIVE)
                {
                    MessageBox.Show("El quiosco no está disponible", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Application.Current.Shutdown();
                    return;
                }

                NumeroKiosco = kiosco.NAME?.Split(' ').ElementAtOrDefault(1);
            }
        }
    }
}

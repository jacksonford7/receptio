using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using ControlesAccesoQR.Models;
using ControlesAccesoQR.Views.ControlesAccesoQR;
using ControlesAccesoQR.Servicios;

using EstadoPanel = ControlesAccesoQR.Estados.EstadoProceso;
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
        private EstadoPanel _estadoPanelActual = EstadoPanel.Pase;
        private string _fechaHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        private readonly DispatcherTimer _reloj = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };

        public ObservableCollection<Proceso> Procesos { get; } = new ObservableCollection<Proceso>();

        public IEnumerable<EstadoPanel> Estados { get; } = Enum.GetValues(typeof(EstadoPanel)).Cast<EstadoPanel>();

        public ICommand MostrarEntradaSalidaCommand { get; }
        public ICommand MostrarSalidaFinalCommand { get; }

        public string NumeroKiosco
        {
            get => _numeroKiosco;
            private set { _numeroKiosco = value; OnPropertyChanged(nameof(NumeroKiosco)); }
        }

        public EstadoPanel EstadoPanelActual
        {
            get => _estadoPanelActual;
            private set
            {
                if (_estadoPanelActual == value) return;
                _estadoPanelActual = value;
                OnPropertyChanged(nameof(EstadoPanelActual));
            }
        }

        public string FechaHora
        {
            get => _fechaHora;
            private set { _fechaHora = value; OnPropertyChanged(nameof(FechaHora)); }
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

            _reloj.Tick += (s, e) => FechaHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            _reloj.Start();

            EstadoPanelEvents.EstadoCodigoCambiado += OnEstadoCodigoCambiado;

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

        public EstadoPanel MapEstadoToProceso(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return EstadoPanel.Pase;

            var c = codigo.Trim().ToUpperInvariant();

            switch (c)
            {
                case "H": return EstadoPanel.Huella;
                case "R": return EstadoPanel.Tag;
                case "P": return EstadoPanel.Ticket;
                default: return EstadoPanel.Pase;
            }
        }

        public void SetEstadoDesdeCodigo(string codigo)
        {
            var nuevo = MapEstadoToProceso(codigo);
            if (nuevo == _estadoPanelActual) return;

            var disp = System.Windows.Application.Current?.Dispatcher;
            if (disp != null && !disp.CheckAccess())
            {
                disp.Invoke(new Action(() => EstadoPanelActual = nuevo));
            }
            else
            {
                EstadoPanelActual = nuevo;
            }
        }

        private void OnEstadoCodigoCambiado(string codigo)
        {
            SetEstadoDesdeCodigo(codigo);
        }

    }
}

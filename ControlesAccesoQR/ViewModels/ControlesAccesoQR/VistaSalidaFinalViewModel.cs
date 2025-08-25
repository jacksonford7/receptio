using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using System.ComponentModel;
using ControlesAccesoQR;
using ControlesAccesoQR.accesoDatos;
using ControlesAccesoQR.Models;
using ControlesAccesoQR.Servicios;
using ControlesAccesoQR.Impresion;
using EstadoProcesoEnum = ControlesAccesoQR.Models.EstadoProceso;

using RECEPTIO.CapaPresentacion.UI.MVVM;

namespace ControlesAccesoQR.ViewModels.ControlesAccesoQR
{
    public class VistaSalidaFinalViewModel : ViewModelBase
    {
        private string _nombre;
        private string _empresa;
        private string _patente;
        private string _horaSalida;
        private bool _salidaRegistrada;
        private string _numeroPaseSalida;
        private int _isProcessing;
        private string _ultimoCodigoProcesado;
        private DateTime? _fechaSalida;
        private string _mensajeError;
        private readonly PasePuertaDataAccess _dataAccess = new PasePuertaDataAccess();
        private readonly IEstadoService _estadoService = new EstadoService();
        private readonly MainWindowViewModel _mainViewModel;

        public string Nombre { get => _nombre; set { _nombre = value; OnPropertyChanged(nameof(Nombre)); } }
        public string Empresa { get => _empresa; set { _empresa = value; OnPropertyChanged(nameof(Empresa)); } }
        public string Patente { get => _patente; set { _patente = value; OnPropertyChanged(nameof(Patente)); } }
        public string HoraSalida { get => _horaSalida; set { _horaSalida = value; OnPropertyChanged(nameof(HoraSalida)); } }
        public bool SalidaRegistrada { get => _salidaRegistrada; set { _salidaRegistrada = value; OnPropertyChanged(nameof(SalidaRegistrada)); } }
        public string NumeroPaseSalida
        {
            get { return _numeroPaseSalida; }
            set
            {
                if (_numeroPaseSalida == value) return;
                _numeroPaseSalida = value;
                OnPropertyChanged(nameof(NumeroPaseSalida));
                CommandManager.InvalidateRequerySuggested();
            }
        }
        public DateTime? FechaSalida { get => _fechaSalida; set { _fechaSalida = value; OnPropertyChanged(nameof(FechaSalida)); } }
        public string MensajeError { get => _mensajeError; set { _mensajeError = value; OnPropertyChanged(nameof(MensajeError)); } }

        public ObservableCollection<string> Contenedores { get; } = new ObservableCollection<string>();

        public ICommand ProcesarNumeroCommand { get; }

        public VistaSalidaFinalViewModel(MainWindowViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            _mainViewModel.PropertyChanged += MainViewModelOnPropertyChanged;

            ProcesarNumeroCommand = new RelayCommand(
                async () => await ProcesarSalidaAsync(),
                () => !string.IsNullOrWhiteSpace(NumeroPaseSalida) && _isProcessing == 0);

            Contenedores.Add("CONT-001");
            Contenedores.Add("CONT-002");
        }

        private void MainViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainWindowViewModel.EstadoProceso) &&
                _mainViewModel.EstadoProceso == EstadoProcesoEnum.EnEspera)
            {
                LimpiarCampos();
            }
        }

        private void LimpiarCampos()
        {
            Nombre = string.Empty;
            Empresa = string.Empty;
            Patente = string.Empty;
            HoraSalida = string.Empty;
            NumeroPaseSalida = string.Empty;
            FechaSalida = null;
            SalidaRegistrada = false;
            MensajeError = string.Empty;
        }

        private async Task ProcesarSalidaAsync()
        {
            if (Interlocked.Exchange(ref _isProcessing, 1) == 1) return;
            try
            {
                MensajeError = string.Empty;
                SalidaRegistrada = false;

                var codigo = string.IsNullOrWhiteSpace(NumeroPaseSalida) ? null : NumeroPaseSalida.Trim();
                if (string.IsNullOrWhiteSpace(codigo))
                {
                    MensajeError = "Ingrese el número de pase.";
                    return;
                }

                if (string.Equals(_ultimoCodigoProcesado, codigo, StringComparison.Ordinal))
                    return;

                var info = _dataAccess.ObtenerChoferEmpresaPorPaseSalida(codigo);
                if (info == null)
                {
                    MensajeError = "Número de pase inválido.";
                    return;
                }
                Nombre = info.ChoferNombre;
                Empresa = info.EmpresaNombre;
                Patente = info.Patente;

                var resultado = _dataAccess.ActualizarFechaSalida(codigo);
                if (resultado == null)
                {
                    MensajeError = "No se pudo registrar la salida.";
                    return;
                }
                HoraSalida = resultado.FechaHoraSalida.ToString("HH:mm");
                FechaSalida = resultado.FechaHoraSalida;
                SalidaRegistrada = true;

                await ImprimirAsync(codigo);

                try
                {
                    await _estadoService.ActualizarAsync(codigo, "S", default(CancellationToken));
                }
                catch { }

                _ultimoCodigoProcesado = codigo;

                _mainViewModel.PaseActual = new PaseProcesoModel
                {
                    NombreChofer = Nombre,
                    Placa = Patente,
                    FechaHoraSalida = resultado.FechaHoraSalida,
                    NumeroPase = resultado.NumeroPase,
                    Estado = EstadoProcesoEnum.SalidaRegistrada,
                };
                _mainViewModel.EstadoProceso = EstadoProcesoEnum.SalidaRegistrada;
                EstadoPanelEvents.RaiseEstadoCodigoCambiado("P");
                _ = _mainViewModel.ReiniciarDespuesDeSalidaAsync();
            }
            finally
            {
                Interlocked.Exchange(ref _isProcessing, 0);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private async Task ImprimirAsync(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                MensajeError = "No hay código para imprimir.";
                return;
            }

            var datos = new DatosTicketQr { Cliente = Empresa, Chofer = Nombre };

            if (DevBypass.IsDevKiosk)
            {
                MessageBox.Show("Impresión simulada (CGDE041)");
                return;
            }

            IEstadoImpresora estadoImpresora = new EstadoImpresora();
            var mensajes = estadoImpresora.VerEstado();
            if (mensajes.Item1.Any())
            {
                MensajeError = string.Join(Environment.NewLine, mensajes.Item1);
                return;
            }

            using (var ticket = new ImprimirTicketSalidaQr(codigo, datos))
            {
                ticket.Imprimir();
            }
        }
    }
}


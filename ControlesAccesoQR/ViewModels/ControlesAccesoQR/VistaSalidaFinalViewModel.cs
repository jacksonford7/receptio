using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Text.RegularExpressions;
using ControlesAccesoQR;
using ControlesAccesoQR.accesoDatos;
using ControlesAccesoQR.Models;
using ControlesAccesoQR.Servicios;
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
        private DateTime? _fechaSalida;
        private string _mensajeError;
        private readonly PasePuertaDataAccess _dataAccess = new PasePuertaDataAccess();
        private readonly IEstadoService _estadoService = new EstadoService();
        private readonly PrintService _printService = new PrintService();
        private readonly MainWindowViewModel _mainViewModel;

        private int _isProcessing; // 0 = libre, 1 = procesando
        private DateTime _lastScanUtc;
        private CancellationTokenSource _debounceCts;

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
                if (string.Equals(_numeroPaseSalida, value)) return;
                _numeroPaseSalida = value;
                OnPropertyChanged(nameof(NumeroPaseSalida));

                if (_debounceCts != null) _debounceCts.Cancel();
                _debounceCts = new CancellationTokenSource();
                var ct = _debounceCts.Token;

                if (string.IsNullOrWhiteSpace(_numeroPaseSalida)) return;

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await DebounceAsync(200, ct);
                        if (!ct.IsCancellationRequested)
                        {
                            await ProcesarSalidaAsync();
                        }
                    }
                    catch (TaskCanceledException) { }
                });
            }
        }
        public DateTime? FechaSalida { get => _fechaSalida; set { _fechaSalida = value; OnPropertyChanged(nameof(FechaSalida)); } }
        public string MensajeError { get => _mensajeError; set { _mensajeError = value; OnPropertyChanged(nameof(MensajeError)); } }

        public ObservableCollection<string> Contenedores { get; } = new ObservableCollection<string>();

        public ICommand SubmitPassCommand { get; }
        public ICommand ProcesarSalidaCommand { get; }
        public ICommand ImprimirCommand { get; }

        public VistaSalidaFinalViewModel(MainWindowViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            SubmitPassCommand = new RelayCommand(() => SubmitPass(NumeroPaseSalida, "manual"));
            ProcesarSalidaCommand = new AsyncRelayCommand(ProcesarSalidaAsync);
            ImprimirCommand = new RelayCommand(Imprimir, PuedeImprimir);

            Contenedores.Add("CONT-001");
            Contenedores.Add("CONT-002");
        }

        private async Task DebounceAsync(int milliseconds, CancellationToken ct)
        {
            await Task.Delay(milliseconds, ct);
        }

        private void DetenerLectorQr()
        {
            // TODO: desuscribir eventos del lector o deshabilitar timer/polling
        }

        private void IniciarLectorQr()
        {
            // TODO: volver a suscribir eventos / reactivar lector
        }

        private async Task ProcesarSalidaAsync()
        {
            if (Interlocked.Exchange(ref _isProcessing, 1) == 1) return;

            try
            {
                DetenerLectorQr();

                var now = DateTime.UtcNow;
                if (_lastScanUtc != DateTime.MinValue && (now - _lastScanUtc).TotalMilliseconds < 800)
                    return;
                _lastScanUtc = now;

                MensajeError = string.Empty;
                SalidaRegistrada = false;

                if (string.IsNullOrWhiteSpace(NumeroPaseSalida))
                {
                    MensajeError = "Número de pase inválido";
                    return;
                }

                var info = _dataAccess.ObtenerChoferEmpresaPorPaseSalida(NumeroPaseSalida);
                if (info == null)
                {
                    MensajeError = "Número de pase inválido";
                    return;
                }

                Nombre = info.ChoferNombre;
                Empresa = info.EmpresaNombre;
                Patente = info.Patente;

                var resultado = _dataAccess.ActualizarFechaSalida(NumeroPaseSalida);
                if (resultado == null)
                {
                    MensajeError = "Número de pase inválido";
                    return;
                }

                HoraSalida = resultado.FechaHoraSalida.ToString("HH:mm");
                FechaSalida = resultado.FechaHoraSalida;
                SalidaRegistrada = true;

                try
                {
                    var estado = await _estadoService.ActualizarAsync(NumeroPaseSalida, "S");
                    if (estado == null)
                    {
                        if (DevBypass.IsDevKiosk)
                            MessageBox.Show("El pase no existe o el SP no devolvió filas");
                        else
                            Console.WriteLine("ActualizarEstadoAsync retornó null");
                        return;
                    }
                    NumeroPaseSalida = estado.NumeroPase;
                }
                catch (SqlException ex)
                {
                    if (DevBypass.IsDevKiosk)
                        MessageBox.Show(ex.Message);
                    else
                        Console.WriteLine(ex);
                    return;
                }
                catch (TimeoutException ex)
                {
                    if (DevBypass.IsDevKiosk)
                        MessageBox.Show(ex.Message);
                    else
                        Console.WriteLine(ex);
                    return;
                }

                _mainViewModel.PaseActual = new PaseProcesoModel
                {
                    NombreChofer = Nombre,
                    Placa = Patente,
                    FechaHoraSalida = resultado.FechaHoraSalida,
                    NumeroPase = resultado.NumeroPase,

                    Estado = EstadoProcesoEnum.SalidaRegistrada,

                };
                _mainViewModel.EstadoProceso = EstadoProcesoEnum.SalidaRegistrada;
                await ImprimirAsync();
                _ = _mainViewModel.ReiniciarDespuesDeSalidaAsync();
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
            finally
            {
                Interlocked.Exchange(ref _isProcessing, 0);
                IniciarLectorQr();
            }
        }

        public void SubmitPass(string input, string inputMethod)
        {
            var normalized = NormalizeInput(input);
            if (string.IsNullOrWhiteSpace(normalized))
            {
                MessageBox.Show("Número de pase inválido");
                return;
            }

            if (!Regex.IsMatch(normalized, "^[A-Za-z0-9]+$"))
            {
                MessageBox.Show("Formato de número de pase inválido");
                return;
            }

            NumeroPaseSalida = normalized;

            var datos = _dataAccess.ObtenerChoferEmpresaPorPaseSalida(NumeroPaseSalida);
            if (datos != null)
            {
                Nombre = datos.ChoferNombre;
                Empresa = datos.EmpresaNombre;
                Patente = datos.Patente;
            }
            else
            {
                MessageBox.Show("No se encontraron datos para el pase");
            }
        }

        private string NormalizeInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var cleaned = input.Trim().Replace("\r", string.Empty).Replace("\n", string.Empty);

            if (cleaned.StartsWith("URI:", StringComparison.OrdinalIgnoreCase))
                cleaned = cleaned.Substring(4);

            var match = Regex.Match(cleaned, @"passNumber[""':=]+([A-Za-z0-9-]+)");
            if (match.Success)
                return match.Groups[1].Value;

            return cleaned;
        }

        private bool PuedeImprimir() => FechaSalida.HasValue;

        private async Task ImprimirAsync()
        {
            try
            {
                await Task.Run(() => Imprimir());
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
        }

        private void Imprimir()
        {
            MensajeError = string.Empty;
            if (!FechaSalida.HasValue)
            {
                MensajeError = "No hay fecha de salida para imprimir.";
                return;
            }

            var contenido =
                "SALIDA" + Environment.NewLine +
                $"Fecha Salida: {FechaSalida.Value:yyyy-MM-dd HH:mm}";

            _printService.Print(contenido);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ControlesAccesoQR;
using System.Windows.Input;
using QRCoder;
using RECEPTIO.CapaPresentacion.UI.Interfaces.RFID;
using RECEPTIO.CapaPresentacion.UI.MVVM;
using RECEPTIO.CapaPresentacion.UI.Interfaces.Impresora;
using RECEPTIO.CapaPresentacion.UI.ImpresoraZebra;
using Spring.Context.Support;
using ControlesAccesoQR.accesoDatos;
using ControlesAccesoQR.Models;
using ControlesAccesoQR.Impresion;
using ControlesAccesoQR.Servicios;

using EstadoProcesoTipo = ControlesAccesoQR.Models.EstadoProceso;


namespace ControlesAccesoQR.ViewModels.ControlesAccesoQR
{
    public class VistaEntradaSalidaViewModel : ViewModelBase
    {
        private string _nombre;
        private string _empresa;
        private string _patente;
        private DateTime _horaLlegada;
        private bool _ingresoRealizado;
        private string _qrImagePath;
        private string _codigoQR;
        private bool _isBusy;
        private string _rfidMensaje;
        private string _estadoActual;
        private DateTime? _ultimaActualizacion;
        private DateTime? _fecha;
        private string _salida;
        private string _chofer;
        private string _mensajeError;
        private int _isProcessing;
        private string _ultimoCodigoProcesado;
        private CancellationTokenSource _debounceCts;
        private readonly TimeSpan _debounceDelay = TimeSpan.FromMilliseconds(300);
        private bool _lectorSuscrito;
        private string _numeroPase;
        public bool HabilitarAutoPorLectorQR { get; set; }

        private readonly PasePuertaDataAccess _dataAccess = new PasePuertaDataAccess();
        private readonly IEstadoService _estadoService = new EstadoService();
        private readonly MainWindowViewModel _mainViewModel;

        public MainWindowViewModel MainViewModel => _mainViewModel;
        public string ChoferID { get => _choferId; set { _choferId = value; OnPropertyChanged(nameof(ChoferID)); } }
        private string _choferId;

        public string Nombre { get => _nombre; set { _nombre = value; OnPropertyChanged(nameof(Nombre)); } }
        public string Empresa { get => _empresa; set { _empresa = value; OnPropertyChanged(nameof(Empresa)); CommandManager.InvalidateRequerySuggested(); } }
        public string Patente { get => _patente; set { _patente = value; OnPropertyChanged(nameof(Patente)); } }
        public DateTime HoraLlegada { get => _horaLlegada; set { _horaLlegada = value; OnPropertyChanged(nameof(HoraLlegada)); } }
        public DateTime? Fecha { get => _fecha; set { _fecha = value; OnPropertyChanged(nameof(Fecha)); CommandManager.InvalidateRequerySuggested(); } }
        public string Salida { get => _salida; set { _salida = value; OnPropertyChanged(nameof(Salida)); CommandManager.InvalidateRequerySuggested(); } }
        public string Chofer { get => _chofer; set { _chofer = value; OnPropertyChanged(nameof(Chofer)); CommandManager.InvalidateRequerySuggested(); } }
        public string MensajeError { get => _mensajeError; set { _mensajeError = value; OnPropertyChanged(nameof(MensajeError)); } }
        public bool IngresoRealizado { get => _ingresoRealizado; set { _ingresoRealizado = value; OnPropertyChanged(nameof(IngresoRealizado)); } }
        public string QrImagePath { get => _qrImagePath; set { _qrImagePath = value; OnPropertyChanged(nameof(QrImagePath)); } }
        public string CodigoQR
        {
            get => _codigoQR;
            set
            {
                var cleaned = (value ?? string.Empty)
                    .Replace("\r", string.Empty)
                    .Replace("\n", string.Empty)
                    .Trim();

                if (_codigoQR == cleaned) return;
                _codigoQR = cleaned;
                OnPropertyChanged(nameof(CodigoQR));

                if (!HabilitarAutoPorLectorQR)
                {
                    CommandManager.InvalidateRequerySuggested();
                    return;
                }

                _ = DebounceProcesarAsync();
            }
        }

        public string NumeroPase
        {
            get => _numeroPase;
            set
            {
                var cleaned = (value ?? string.Empty)
                    .Replace("\r", string.Empty)
                    .Replace("\n", string.Empty)
                    .Trim();
                if (_numeroPase == cleaned) return;
                _numeroPase = cleaned;
                OnPropertyChanged(nameof(NumeroPase));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private string _numeroPaseEscaneado;
        public string NumeroPaseEscaneado
        {
            get => _numeroPaseEscaneado;
            set
            {
                var cleaned = (value ?? string.Empty)
                    .Replace("\r", string.Empty)
                    .Replace("\n", string.Empty)
                    .Trim();
                if (_numeroPaseEscaneado == cleaned) return;
                _numeroPaseEscaneado = cleaned;
                OnPropertyChanged(nameof(NumeroPaseEscaneado));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public bool IsProcessing => _isProcessing != 0;

        public string IdentificadorDeTrabajo => CodigoQR;

        public string RfidMensaje
        {
            get => _rfidMensaje;
            private set { _rfidMensaje = value; OnPropertyChanged(nameof(RfidMensaje)); }
        }

        public string EstadoActual
        {
            get => _estadoActual;
            private set { _estadoActual = value; OnPropertyChanged(nameof(EstadoActual)); }
        }

        public DateTime? UltimaActualizacion
        {
            get => _ultimaActualizacion;
            private set { _ultimaActualizacion = value; OnPropertyChanged(nameof(UltimaActualizacion)); }
        }

        public ICommand ProcesarCommand { get; }

        public VistaEntradaSalidaViewModel(MainWindowViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            HabilitarAutoPorLectorQR = false;

            if (!_lectorSuscrito && HabilitarAutoPorLectorQR)
            {
                // lector.OnLeido -= OnQrLeido;
                // lector.OnLeido += OnQrLeido;
                _lectorSuscrito = true;
            }

            ProcesarCommand = new RelayCommand(
                async () => await ProcesarEntradaSalidaAsync(),
                () => !IsProcessing && (!string.IsNullOrWhiteSpace(NumeroPase) || !string.IsNullOrWhiteSpace(CodigoQR) || !string.IsNullOrWhiteSpace(NumeroPaseEscaneado)));
        }


        private async Task ProcesarEntradaSalidaAsync()
        {
            if (Interlocked.Exchange(ref _isProcessing, 1) == 1) return;
            OnPropertyChanged(nameof(IsProcessing));

            try
            {
                MensajeError = string.Empty;

                string codigo = null;
                if (!string.IsNullOrWhiteSpace(CodigoQR))
                    codigo = CodigoQR.Trim();
                else if (!string.IsNullOrWhiteSpace(NumeroPaseEscaneado))
                    codigo = NumeroPaseEscaneado.Trim();
                else if (!string.IsNullOrWhiteSpace(NumeroPase))
                    codigo = NumeroPase.Trim();

                if (string.IsNullOrWhiteSpace(codigo))
                {
                    MensajeError = "Ingrese el código o número de pase.";
                    return;
                }

                if (string.Equals(_ultimoCodigoProcesado, codigo, StringComparison.Ordinal))
                    return;

                var datos = _dataAccess.ObtenerChoferEmpresaPorPaseSalida(codigo);
                if (datos == null)
                {
                    MensajeError = "Código inválido.";
                    return;
                }

                Nombre = datos.ChoferNombre;
                Empresa = datos.EmpresaNombre;
                Patente = datos.Patente;
                ChoferID = datos.ChoferID;
                Chofer = datos.ChoferNombre;

                var resultado = _dataAccess.ActualizarFechaLlegada(codigo);
                if (resultado == null)
                {
                    MensajeError = "No se pudo registrar.";
                    return;
                }

                HoraLlegada = resultado.FechaHoraLlegada;
                Fecha = resultado.FechaHoraLlegada;

                NumeroPase = codigo;

                var idPase = resultado.PasePuertaID.ToString();
                if (!string.Equals(CodigoQR, idPase, StringComparison.Ordinal))
                    CodigoQR = idPase;

                if (!await ActualizarEstadoAsync("I", default))
                    return;

                IngresoRealizado = true;

                _mainViewModel.PaseActual = new PaseProcesoModel
                {
                    NombreChofer = Nombre,
                    Placa = Patente,
                    FechaHoraLlegada = HoraLlegada,
                    NumeroPase = NumeroPase,
                    Estado = EstadoProcesoTipo.EnEspera,
                };

                await ImprimirAsync(codigo);

                _ultimoCodigoProcesado = codigo;
            }
            finally
            {
                Interlocked.Exchange(ref _isProcessing, 0);
                OnPropertyChanged(nameof(IsProcessing));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private async Task DebounceProcesarAsync()
        {
            _debounceCts?.Cancel();
            _debounceCts = new CancellationTokenSource();
            var token = _debounceCts.Token;

            try
            {
                await Task.Delay(_debounceDelay, token);
                if (!string.IsNullOrWhiteSpace(CodigoQR))
                {
                    await ProcesarEntradaSalidaAsync();
                }
            }
            catch (TaskCanceledException) { }
        }

        public async Task<bool> ActualizarEstadoAsync(string estado, CancellationToken ct = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(CodigoQR))
                return false;

            try
            {
                var result = await _estadoService.ActualizarAsync(CodigoQR, estado, ct);
                if (result == null)
                {
                    if (DevBypass.IsDevKiosk)
                        MessageBox.Show("El pase no existe o el SP no devolvió filas");
                    else
                        Console.WriteLine("ActualizarEstadoAsync retornó null");
                    return false;
                }

                EstadoActual = result.Estado;
                CodigoQR = result.PasePuertaID.ToString();
                UltimaActualizacion = result.FechaActualizacion;
                EstadoPanelEvents.RaiseEstadoCodigoCambiado(result.Estado);
                return true;
            }
            catch (SqlException ex)
            {
                if (DevBypass.IsDevKiosk)
                    MessageBox.Show(ex.Message);
                else
                    Console.WriteLine(ex);
            }
            catch (TimeoutException ex)
            {
                if (DevBypass.IsDevKiosk)
                    MessageBox.Show(ex.Message);
                else
                    Console.WriteLine(ex);
            }
            return false;
        }

        private async Task ImprimirAsync(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                MensajeError = "No hay código para imprimir.";
                return;
            }

            var datos = new DatosTicketQr
            {
                Cliente = Empresa,
                Chofer = Nombre
            };

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

        /// <summary>
        /// Ejecuta la validación del tag RFID de manera asíncrona.
        /// </summary>
        /// <returns>true si el tag leído es válido.</returns>
        public async Task<bool> ValidarRfidAsync()
        {
            bool resultado = false;
            IAntena antena = null;

            if (DevBypass.IsDevKiosk)
            {
                MessageBox.Show("RFID detectado"); // BYPASS CGDE041
                RfidMensaje = "Tag leído válido";
                resultado = true;
            }
            else
            {
                try
                {
                    var tagEsperado = _dataAccess.ObtenerTagRfidPorPlaca(Patente);
                    if (string.IsNullOrWhiteSpace(tagEsperado))
                    {
                        RfidMensaje = "No existe tag en BD";
                        return false;
                    }

                    var ctx = new XmlApplicationContext("~/Springs/SpringAntena.xml");
                    antena = (IAntena)ctx["AdministradorAntena"];
                    if (!antena.ConectarAntena())
                    {
                        RfidMensaje = "No se pudo conectar a la antena RFID";
                        return false;
                    }

                    antena.IniciarLectura();
                    await Task.Delay(1000);
                    List<string> tags = antena.ObtenerTagsLeidos();

                    if (tags == null || !tags.Any())
                    {
                        RfidMensaje = "No se leyó ningún tag";
                    }
                    else if (tags.Contains(tagEsperado))
                    {
                        RfidMensaje = "Tag leído válido";
                        resultado = true;
                    }
                    else
                    {
                        RfidMensaje = "Tag leído no coincide";
                    }
                }
                catch (Exception ex)
                {
                    RfidMensaje = ex.Message;
                    System.Diagnostics.Debug.WriteLine(ex);
                    if (DevBypass.IsDevKiosk)
                        MessageBox.Show(ex.Message);
                }
                finally
                {
                    antena?.TerminarLectura();
                    antena?.DesconectarAntena();
                    antena?.Dispose();
                }
            }

            _mainViewModel.Procesos.Add(new Proceso
            {
                STEP = "RFID",
                RESPONSE = RfidMensaje,
                MESSAGE_ID = resultado ? 1 : 0
            });

            if (resultado)
                _mainViewModel.MostrarSalidaFinal();
            else
                _mainViewModel.EstadoProceso = EstadoProcesoTipo.EnEspera;

            return resultado;
        }
    }
}

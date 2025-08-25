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
using System.Windows.Threading;
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
        private bool _isUpdatingCodigoQR;
        private readonly DispatcherTimer _debouncePase;
        private CancellationTokenSource _ctsConsultaPase;
        private bool _isBusy;
        private string _numeroPaseEscaneado;
        private string _rfidMensaje;
        private string _estadoActual;
        private DateTime? _ultimaActualizacion;
        private DateTime? _fecha;
        private string _salida;
        private string _chofer;
        private string _mensajeError;

        private readonly PasePuertaDataAccess _dataAccess = new PasePuertaDataAccess();
        private readonly IEstadoService _estadoService = new EstadoService();
        private readonly PrintService _printService = new PrintService();
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
                if (_isUpdatingCodigoQR)
                    return;
                var nuevo = value ?? string.Empty;
                var soloDigitos = new string(nuevo.Where(char.IsDigit).ToArray());
                if (string.Equals(_codigoQR, soloDigitos, StringComparison.Ordinal))
                    return;

                _isUpdatingCodigoQR = true;
                try
                {
                    _codigoQR = soloDigitos;
                    OnPropertyChanged(nameof(CodigoQR));
                    OnPropertyChanged(nameof(QrValue));
                    IngresarCommand?.RaiseCanExecuteChanged();
                    _debouncePase?.Stop();
                    _debouncePase?.Start();
                }
                finally
                {
                    _isUpdatingCodigoQR = false;
                }
            }
        }

        public string QrValue
        {
            get => CodigoQR;
            set => CodigoQR = value;
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
                IngresarCommand?.RaiseCanExecuteChanged();
            }
        }

        public string NumeroPaseEscaneado
        {
            get => _numeroPaseEscaneado;
            private set
            {
                if (_numeroPaseEscaneado == value)
                    return;
                _numeroPaseEscaneado = value;
                OnPropertyChanged(nameof(NumeroPaseEscaneado));
            }
        }

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

        public ICommand SubmitPassCommand { get; }
        public AsyncRelayCommand IngresarCommand { get; }
        public ICommand ImprimirQrCommand { get; }
        public ICommand ImprimirCommand { get; }

        public VistaEntradaSalidaViewModel(MainWindowViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            _debouncePase = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(350) };
            _debouncePase.Tick += async (s, e) =>
            {
                _debouncePase.Stop();
                await ConsultarPaseAsync();
            };

            SubmitPassCommand = new AsyncRelayCommand(ConsultarPaseAsync);
            IngresarCommand = new AsyncRelayCommand(IngresarAsync, () => !IsBusy && !string.IsNullOrWhiteSpace(QrValue));
            ImprimirQrCommand = new RelayCommand(ImprimirQr);
            ImprimirCommand = new RelayCommand(Imprimir, PuedeImprimir);
        }

        private async Task ConsultarPaseAsync()
        {
            _debouncePase.Stop();
            if (string.IsNullOrWhiteSpace(CodigoQR) || CodigoQR.Length < 4)
                return;

            _ctsConsultaPase?.Cancel();
            _ctsConsultaPase = new CancellationTokenSource();
            var ct = _ctsConsultaPase.Token;

            try
            {
                IsBusy = true;

                var datos = await Task.Run(() => _dataAccess.ObtenerChoferEmpresaPorPaseSalida(CodigoQR), ct);
                if (ct.IsCancellationRequested)
                    return;

                if (datos != null)
                {
                    Nombre = datos.ChoferNombre;
                    Empresa = datos.EmpresaNombre;
                    Patente = datos.Patente;
                    ChoferID = datos.ChoferID;
                    Chofer = datos.ChoferNombre;
                }
                else
                {
                    MensajeError = "No se encontraron datos para el pase";
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
            finally
            {
                if (!ct.IsCancellationRequested)
                    IsBusy = false;
            }
        }

        private async Task IngresarAsync()
        {
            var pass = CodigoQR;
            if (string.IsNullOrWhiteSpace(pass))
                return;

            try
            {
                IsBusy = true;

                var resultado = _dataAccess.ActualizarFechaLlegada(pass);
                if (resultado == null)
                    return;

            HoraLlegada = resultado.FechaHoraLlegada;
            Fecha = resultado.FechaHoraLlegada;
            NumeroPaseEscaneado = CodigoQR;
            // Guardar el QR original por referencia
            if (string.IsNullOrWhiteSpace(NumeroPaseEscaneado))
                NumeroPaseEscaneado = CodigoQR;

            // A partir de aquí trabajamos con el ID devuelto por el SP
            var idPase = resultado.PasePuertaID.ToString();
            if (!string.Equals(CodigoQR, idPase, StringComparison.Ordinal))
            {
                CodigoQR = idPase;                 // <- ahora CodigoQR lleva el PasePuertaID
                OnPropertyChanged(nameof(CodigoQR));
            }

            if (!await ActualizarEstadoAsync("I"))
                return;

            var qrText = $"{CodigoQR}|{resultado.FechaHoraLlegada:yyyy-MM-dd HH:mm:ss}";
            using (var generator = new QRCodeGenerator())
            {
                var data = generator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
                using (var qrCode = new PngByteQRCode(data))
                {
                    var bytes = qrCode.GetGraphic(20);
                    var path = Path.Combine(Path.GetTempPath(), $"qr_{Guid.NewGuid()}.png");
                    File.WriteAllBytes(path, bytes);
                    QrImagePath = path;
                }
            }

            IngresoRealizado = true;

            _mainViewModel.PaseActual = new PaseProcesoModel
            {
                NombreChofer = Nombre,
                Placa = Patente,
                FechaHoraLlegada = HoraLlegada,
                NumeroPase = CodigoQR,

                Estado = EstadoProcesoTipo.EnEspera

            };
        }
        finally
        {
            IsBusy = false;
        }
    }

        public async Task<bool> ActualizarEstadoAsync(string estado, CancellationToken ct = default)
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

        private void ImprimirQr()
        {
            if (string.IsNullOrWhiteSpace(CodigoQR))
                return;

            var datos = new DatosTicketQr
            {
                Cliente = Empresa,
                Chofer = Nombre
            };

            if (DevBypass.IsDevKiosk)
            {
                MessageBox.Show("Impresión simulada (CGDE041)"); // BYPASS CGDE041
                return;
            }

            IEstadoImpresora estadoImpresora = new EstadoImpresora();
            var mensajes = estadoImpresora.VerEstado();
            if (mensajes.Item1.Any())
                return;

            using (var ticket = new ImprimirTicketSalidaQr(CodigoQR, datos))
            {
                ticket.Imprimir();
            }
        }

        private bool PuedeImprimir()
        {
            return Fecha.HasValue &&
                   !string.IsNullOrWhiteSpace(Salida) &&
                   !string.IsNullOrWhiteSpace(Empresa) &&
                   !string.IsNullOrWhiteSpace(Chofer);
        }

        private void Imprimir()
        {
            MensajeError = string.Empty;

            if (!PuedeImprimir())
            {
                MensajeError = "Faltan datos para imprimir (Fecha/Salida/Empresa/Chofer).";
                return;
            }

            var contenido =
                "CONTROL ENTRADA/SALIDA" + Environment.NewLine +
                $"Fecha: {Fecha.Value:yyyy-MM-dd HH:mm}" + Environment.NewLine +
                $"Salida: {Salida}" + Environment.NewLine +
                $"Empresa: {Empresa}" + Environment.NewLine +
                $"Chofer: {Chofer}";

            _printService.Print(contenido);
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

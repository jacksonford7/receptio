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
using System.Text.RegularExpressions;
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
        private string _numeroPaseEscaneado;
        private string _rfidMensaje;
        private string _estadoActual;
        private DateTime? _ultimaActualizacion;

        private readonly PasePuertaDataAccess _dataAccess = new PasePuertaDataAccess();
        private readonly IEstadoService _estadoService = new EstadoService();
        private readonly MainWindowViewModel _mainViewModel;

        public MainWindowViewModel MainViewModel => _mainViewModel;
        public string ChoferID { get => _choferId; set { _choferId = value; OnPropertyChanged(nameof(ChoferID)); } }
        private string _choferId;

        public string Nombre { get => _nombre; set { _nombre = value; OnPropertyChanged(nameof(Nombre)); } }
        public string Empresa { get => _empresa; set { _empresa = value; OnPropertyChanged(nameof(Empresa)); } }
        public string Patente { get => _patente; set { _patente = value; OnPropertyChanged(nameof(Patente)); } }
        public DateTime HoraLlegada { get => _horaLlegada; set { _horaLlegada = value; OnPropertyChanged(nameof(HoraLlegada)); } }
        public bool IngresoRealizado { get => _ingresoRealizado; set { _ingresoRealizado = value; OnPropertyChanged(nameof(IngresoRealizado)); } }
        public string QrImagePath { get => _qrImagePath; set { _qrImagePath = value; OnPropertyChanged(nameof(QrImagePath)); } }
        public string CodigoQR
        {
            get => _codigoQR;
            set
            {
                _codigoQR = value;
                OnPropertyChanged(nameof(CodigoQR));
                OnPropertyChanged(nameof(QrValue));
                IngresarCommand?.RaiseCanExecuteChanged();
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

        public VistaEntradaSalidaViewModel(MainWindowViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            SubmitPassCommand = new RelayCommand(() => SubmitPass(CodigoQR, "manual"));
            IngresarCommand = new AsyncRelayCommand(IngresarAsync, () => !IsBusy && !string.IsNullOrWhiteSpace(QrValue));
            ImprimirQrCommand = new RelayCommand(ImprimirQr);
        }

        private DateTime _lastSubmitTime = DateTime.MinValue;
        private string _lastInput = string.Empty;

        public void SubmitPass(string input, string inputMethod)
        {
            var now = DateTime.UtcNow;
            if (input == _lastInput && (now - _lastSubmitTime).TotalMilliseconds < 200)
                return;
            _lastInput = input;
            _lastSubmitTime = now;

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

            CodigoQR = normalized;

            var datos = _dataAccess.ObtenerChoferEmpresaPorPase(CodigoQR);
            if (datos != null)
            {
                Nombre = datos.ChoferNombre;
                Empresa = datos.EmpresaNombre;
                Patente = datos.Patente;
                ChoferID = datos.ChoferID;
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
                Contenedor = "CONT-001",
                Booking = "BOOK-001",
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

using System;
using System.IO;
using System.Windows.Input;
using QRCoder;
using RECEPTIO.CapaPresentacion.UI.MVVM;
using ControlesAccesoQR.accesoDatos;
using ControlesAccesoQR.Models;

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

        private readonly PasePuertaDataAccess _dataAccess = new PasePuertaDataAccess();
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
            }
        }

        public ICommand EscanearQrCommand { get; }
        public ICommand IngresarCommand { get; }
        public ICommand ImprimirQrCommand { get; }

        public VistaEntradaSalidaViewModel(MainWindowViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            EscanearQrCommand = new RelayCommand(EscanearQr);
            IngresarCommand = new RelayCommand(Ingresar);
            ImprimirQrCommand = new RelayCommand(ImprimirQr);
        }

        private void EscanearQr()
        {
            if (string.IsNullOrWhiteSpace(CodigoQR))
                return;

            var datos = _dataAccess.ObtenerChoferEmpresaPorPase(CodigoQR);
            if (datos != null)
            {
                Nombre = datos.ChoferNombre;
                Empresa = datos.EmpresaNombre;
                Patente = datos.Patente;
                ChoferID = datos.ChoferID;
            }
        }

        private void Ingresar()
        {
            if (string.IsNullOrWhiteSpace(CodigoQR))
                return;

            var resultado = _dataAccess.ActualizarFechaLlegada(CodigoQR);
            if (resultado == null)
                return;

            HoraLlegada = resultado.FechaHoraLlegada;

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

        private void ImprimirQr()
        {
            // Lógica de impresión pendiente
        }
    }
}

using System;
using System.Windows.Input;
using RECEPTIO.CapaPresentacion.UI.MVVM;
using ControlesAccesoQR.accesoDatos;

namespace ControlesAccesoQR.ViewModels.ControlesAccesoQR
{
    public class VistaEntradaSalidaViewModel : ViewModelBase
    {
        private string _nombre;
        private string _empresa;
        private string _patente;
        private string _horaLlegada;
        private bool _ingresoRealizado;
        private string _qrImagePath;
        private string _qrIngresado;

        private readonly PasePuertaDataAccess _dataAccess = new PasePuertaDataAccess();

        public string Nombre { get => _nombre; set { _nombre = value; OnPropertyChanged(nameof(Nombre)); } }
        public string Empresa { get => _empresa; set { _empresa = value; OnPropertyChanged(nameof(Empresa)); } }
        public string Patente { get => _patente; set { _patente = value; OnPropertyChanged(nameof(Patente)); } }
        public string HoraLlegada { get => _horaLlegada; set { _horaLlegada = value; OnPropertyChanged(nameof(HoraLlegada)); } }
        public bool IngresoRealizado { get => _ingresoRealizado; set { _ingresoRealizado = value; OnPropertyChanged(nameof(IngresoRealizado)); } }
        public string QrImagePath { get => _qrImagePath; set { _qrImagePath = value; OnPropertyChanged(nameof(QrImagePath)); } }
        public string QrIngresado { get => _qrIngresado; set { _qrIngresado = value; OnPropertyChanged(nameof(QrIngresado)); } }

        public ICommand EscanearQrCommand { get; }
        public ICommand IngresarCommand { get; }
        public ICommand ImprimirQrCommand { get; }

        public VistaEntradaSalidaViewModel()
        {
            EscanearQrCommand = new RelayCommand(EscanearQr);
            IngresarCommand = new RelayCommand(Ingresar);
            ImprimirQrCommand = new RelayCommand(ImprimirQr);
        }

        private void EscanearQr()
        {
            if (string.IsNullOrWhiteSpace(QrIngresado))
                return;

            var datos = _dataAccess.ObtenerChoferEmpresaPorPase(QrIngresado);
            if (datos != null)
            {
                Nombre = datos.Nombre;
                Empresa = datos.Empresa;
                Patente = datos.Patente;
            }
        }

        private void Ingresar()
        {
            HoraLlegada = DateTime.Now.ToString("HH:mm");
            QrImagePath = "Images/qr_placeholder.png";
            IngresoRealizado = true;
            // TODO: Cambiar estado a 'En Curso' en [vhs].[PasePuerta]
        }

        private void ImprimirQr()
        {
            // Lógica de impresión pendiente
        }
    }
}

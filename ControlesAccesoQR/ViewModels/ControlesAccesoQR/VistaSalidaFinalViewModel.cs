using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ControlesAccesoQR.accesoDatos;
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
        private string _qrLeido;
        private string _mensajeError;
        private readonly PasePuertaDataAccess _dataAccess = new PasePuertaDataAccess();

        public string Nombre { get => _nombre; set { _nombre = value; OnPropertyChanged(nameof(Nombre)); } }
        public string Empresa { get => _empresa; set { _empresa = value; OnPropertyChanged(nameof(Empresa)); } }
        public string Patente { get => _patente; set { _patente = value; OnPropertyChanged(nameof(Patente)); } }
        public string HoraSalida { get => _horaSalida; set { _horaSalida = value; OnPropertyChanged(nameof(HoraSalida)); } }
        public bool SalidaRegistrada { get => _salidaRegistrada; set { _salidaRegistrada = value; OnPropertyChanged(nameof(SalidaRegistrada)); } }
        public string QrLeido
        {
            get => _qrLeido;
            set
            {
                _qrLeido = value;
                OnPropertyChanged(nameof(QrLeido));
                ProcesarQr();
            }
        }
        public string MensajeError { get => _mensajeError; set { _mensajeError = value; OnPropertyChanged(nameof(MensajeError)); } }

        public ObservableCollection<string> Contenedores { get; } = new ObservableCollection<string>();

        public ICommand EscanearQrSalidaCommand { get; }
        public ICommand ImprimirSalidaCommand { get; }

        public VistaSalidaFinalViewModel()
        {
            EscanearQrSalidaCommand = new RelayCommand(EscanearQrSalida);
            ImprimirSalidaCommand = new RelayCommand(ImprimirSalida);

            Contenedores.Add("CONT-001");
            Contenedores.Add("CONT-002");
        }

        private void EscanearQrSalida()
        {
            // Simulación de escaneo
            Nombre = "Transportista Demo";
            Empresa = "Empresa Demo";
            Patente = "ABC123";
        }

        private void ProcesarQr()
        {
            MensajeError = string.Empty;
            SalidaRegistrada = false;

            if (string.IsNullOrWhiteSpace(QrLeido))
                return;

            if (!QrLeido.Contains("|"))
            {
                MensajeError = "Formato de QR inválido";
                return;
            }

            var partes = QrLeido.Split('|');
            var numeroPase = partes[0];

            var resultado = _dataAccess.ActualizarFechaSalida(numeroPase);
            if (resultado != null)
            {
                HoraSalida = resultado.FechaHoraSalida.ToString("HH:mm");
                SalidaRegistrada = true;
            }
        }

        private void ImprimirSalida()
        {
            // Lógica de impresión pendiente
        }
    }
}

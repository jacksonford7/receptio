using RECEPTIO.CapaPresentacion.UI.MVVM;
using System;
using System.Windows.Input;

namespace ControlesAccesoQR.ViewModels
{
    internal class VistaEntradaSalidaViewModel : ViewModelBase
    {
        private string _nombre;
        private string _empresa;
        private string _patente;
        private string _horaLlegada;
        private string _qrSalida;

        public VistaEntradaSalidaViewModel()
        {
            ComandoEscanearQr = new RelayCommand(EscanearQr);
            ComandoIngresar = new RelayCommand(Ingresar);
            ComandoImprimirQrSalida = new RelayCommand(ImprimirQrSalida);
        }

        public string Nombre
        {
            get => _nombre;
            set { if (_nombre == value) return; _nombre = value; OnPropertyChanged(nameof(Nombre)); }
        }

        public string Empresa
        {
            get => _empresa;
            set { if (_empresa == value) return; _empresa = value; OnPropertyChanged(nameof(Empresa)); }
        }

        public string Patente
        {
            get => _patente;
            set { if (_patente == value) return; _patente = value; OnPropertyChanged(nameof(Patente)); }
        }

        public string HoraLlegada
        {
            get => _horaLlegada;
            set { if (_horaLlegada == value) return; _horaLlegada = value; OnPropertyChanged(nameof(HoraLlegada)); }
        }

        public string QrSalida
        {
            get => _qrSalida;
            set { if (_qrSalida == value) return; _qrSalida = value; OnPropertyChanged(nameof(QrSalida)); }
        }

        public ICommand ComandoEscanearQr { get; }
        public ICommand ComandoIngresar { get; }
        public ICommand ComandoImprimirQrSalida { get; }

        private void EscanearQr()
        {
            // Lógica de lectura del QR (simulada)
        }

        private void Ingresar()
        {
            HoraLlegada = DateTime.Now.ToString("HH:mm:ss");
            QrSalida = "placeholder.png";
            // Aquí debe cambiarse el estado a En Curso en [vhs].[PasePuerta]
        }

        private void ImprimirQrSalida()
        {
            // Lógica de impresión (simulada)
        }
    }
}

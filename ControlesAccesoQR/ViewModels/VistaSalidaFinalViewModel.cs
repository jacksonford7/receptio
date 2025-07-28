using RECEPTIO.CapaPresentacion.UI.MVVM;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ControlesAccesoQR.ViewModels
{
    internal class VistaSalidaFinalViewModel : ViewModelBase
    {
        private string _nombre;
        private string _empresa;
        private string _patente;
        private string _horaSalida;
        private ObservableCollection<string> _contenedores;

        public VistaSalidaFinalViewModel()
        {
            Contenedores = new ObservableCollection<string>();
            ComandoEscanearQrSalida = new RelayCommand(EscanearQrSalida);
            ComandoFinalizar = new RelayCommand(Finalizar);
            ComandoImprimirSalida = new RelayCommand(ImprimirSalida);
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

        public ObservableCollection<string> Contenedores
        {
            get => _contenedores;
            set { if (_contenedores == value) return; _contenedores = value; OnPropertyChanged(nameof(Contenedores)); }
        }

        public string HoraSalida
        {
            get => _horaSalida;
            set { if (_horaSalida == value) return; _horaSalida = value; OnPropertyChanged(nameof(HoraSalida)); }
        }

        public ICommand ComandoEscanearQrSalida { get; }
        public ICommand ComandoFinalizar { get; }
        public ICommand ComandoImprimirSalida { get; }

        private void EscanearQrSalida()
        {
            // Lógica de lectura de QR de salida (simulada)
        }

        private void Finalizar()
        {
            HoraSalida = DateTime.Now.ToString("HH:mm:ss");
            // Aquí debe cambiarse el estado a Finalizado en [vhs].[PasePuerta]
        }

        private void ImprimirSalida()
        {
            // Lógica de impresión (simulada)
        }
    }
}

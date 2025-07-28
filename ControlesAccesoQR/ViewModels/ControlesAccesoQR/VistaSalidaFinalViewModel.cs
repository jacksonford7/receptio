using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using RECEPTIO.CapaPresentacion.UI.MVVM;

namespace ControlesAccesoQR.ViewModels.ControlesAccesoQR
{
    public class VistaSalidaFinalViewModel : ViewModelBase
    {
        private string _nombre;
        private string _empresa;
        private string _patente;
        private string _horaSalida;
        private bool _salidaFinalizada;

        public string Nombre { get => _nombre; set { _nombre = value; OnPropertyChanged(nameof(Nombre)); } }
        public string Empresa { get => _empresa; set { _empresa = value; OnPropertyChanged(nameof(Empresa)); } }
        public string Patente { get => _patente; set { _patente = value; OnPropertyChanged(nameof(Patente)); } }
        public string HoraSalida { get => _horaSalida; set { _horaSalida = value; OnPropertyChanged(nameof(HoraSalida)); } }
        public bool SalidaFinalizada { get => _salidaFinalizada; set { _salidaFinalizada = value; OnPropertyChanged(nameof(SalidaFinalizada)); } }

        public ObservableCollection<string> Contenedores { get; } = new ObservableCollection<string>();

        public ICommand EscanearQrSalidaCommand { get; }
        public ICommand FinalizarCommand { get; }
        public ICommand ImprimirSalidaCommand { get; }

        public VistaSalidaFinalViewModel()
        {
            EscanearQrSalidaCommand = new RelayCommand(EscanearQrSalida);
            FinalizarCommand = new RelayCommand(Finalizar);
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

        private void Finalizar()
        {
            HoraSalida = DateTime.Now.ToString("HH:mm");
            SalidaFinalizada = true;
            // TODO: Cambiar estado a 'Finalizado' en [vhs].[PasePuerta]
        }

        private void ImprimirSalida()
        {
            // Lógica de impresión pendiente
        }
    }
}

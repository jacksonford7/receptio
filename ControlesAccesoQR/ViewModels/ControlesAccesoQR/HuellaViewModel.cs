using System.Windows.Input;
using RECEPTIO.CapaPresentacion.UI.Biometrico;
using RECEPTIO.CapaPresentacion.UI.Interfaces.Biometrico;
using RECEPTIO.CapaPresentacion.UI.MVVM;

namespace ControlesAccesoQR.ViewModels.ControlesAccesoQR
{
    public class HuellaViewModel : ViewModelBase
    {
        private readonly IBiometrico _biometrico;
        private readonly string _choferId;
        private string _resultado;
        private bool _huellaValida;
        private bool _procesando;

        public string Resultado
        {
            get => _resultado;
            set { _resultado = value; OnPropertyChanged(nameof(Resultado)); }
        }

        public bool HuellaValida
        {
            get => _huellaValida;
            set { _huellaValida = value; OnPropertyChanged(nameof(HuellaValida)); }
        }

        public bool Procesando
        {
            get => _procesando;
            set { _procesando = value; OnPropertyChanged(nameof(Procesando)); }
        }

        public ICommand ValidarCommand { get; }

        public HuellaViewModel(string choferId)
        {
            _choferId = choferId;
            _biometrico = new Biometrico();
            ValidarCommand = new RelayCommand(ValidarHuella, () => !Procesando);
        }

        public void ValidarHuella()
        {
            Procesando = true;
            Resultado = _biometrico.ProcesoHuella(_choferId);
            HuellaValida = !string.IsNullOrEmpty(Resultado) && Resultado.Contains(_choferId);
            Procesando = false;
        }
    }
}

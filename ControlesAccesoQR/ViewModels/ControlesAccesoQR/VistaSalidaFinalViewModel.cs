using System.Collections.ObjectModel;
using System.Windows.Input;
using ControlesAccesoQR.accesoDatos;
using ControlesAccesoQR.Models;
using EstadoProcesoEnum = ControlesAccesoQR.Models.EstadoProceso;

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
        private string _numeroPaseSalida;
        private string _mensajeError;
        private readonly PasePuertaDataAccess _dataAccess = new PasePuertaDataAccess();
        private readonly MainWindowViewModel _mainViewModel;

        public string Nombre { get => _nombre; set { _nombre = value; OnPropertyChanged(nameof(Nombre)); } }
        public string Empresa { get => _empresa; set { _empresa = value; OnPropertyChanged(nameof(Empresa)); } }
        public string Patente { get => _patente; set { _patente = value; OnPropertyChanged(nameof(Patente)); } }
        public string HoraSalida { get => _horaSalida; set { _horaSalida = value; OnPropertyChanged(nameof(HoraSalida)); } }
        public bool SalidaRegistrada { get => _salidaRegistrada; set { _salidaRegistrada = value; OnPropertyChanged(nameof(SalidaRegistrada)); } }
        public string NumeroPaseSalida { get => _numeroPaseSalida; set { _numeroPaseSalida = value; OnPropertyChanged(nameof(NumeroPaseSalida)); } }
        public string MensajeError { get => _mensajeError; set { _mensajeError = value; OnPropertyChanged(nameof(MensajeError)); } }

        public ObservableCollection<string> Contenedores { get; } = new ObservableCollection<string>();

        public ICommand ProcesarSalidaCommand { get; }
        public ICommand ImprimirSalidaCommand { get; }

        public VistaSalidaFinalViewModel(MainWindowViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            ProcesarSalidaCommand = new RelayCommand(ProcesarSalida);
            ImprimirSalidaCommand = new RelayCommand(ImprimirSalida);

            Contenedores.Add("CONT-001");
            Contenedores.Add("CONT-002");
        }

        private void ProcesarSalida()
        {
            MensajeError = string.Empty;
            SalidaRegistrada = false;

            if (string.IsNullOrWhiteSpace(NumeroPaseSalida))
                return;

            var resultado = _dataAccess.ActualizarFechaSalida(NumeroPaseSalida);
            if (resultado != null)
            {
                HoraSalida = resultado.FechaHoraSalida.ToString("HH:mm");
                SalidaRegistrada = true;
                Nombre = resultado.ChoferNombre;
                Empresa = resultado.EmpresaNombre;
                Patente = resultado.Patente;

                _dataAccess.ActualizarEstado(NumeroPaseSalida, "S");

                _mainViewModel.PaseActual = new PaseProcesoModel
                {
                    NombreChofer = Nombre,
                    Placa = Patente,
                    FechaHoraSalida = resultado.FechaHoraSalida,
                    NumeroPase = resultado.NumeroPase,

                    Estado = EstadoProcesoEnum.SalidaRegistrada,

                };
                _mainViewModel.EstadoProceso = EstadoProcesoEnum.SalidaRegistrada;
                _ = _mainViewModel.ReiniciarDespuesDeSalidaAsync();
            }
            else
            {
                MensajeError = "Número de pase inválido";
            }
        }

        private void ImprimirSalida()
        {
            // Lógica de impresión pendiente
        }
    }
}


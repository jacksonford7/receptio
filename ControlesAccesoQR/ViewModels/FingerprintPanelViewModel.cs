using System;
using System.Threading;
using System.Windows.Input;
using ControlesAccesoQR.Models;
using ControlesAccesoQR.Services;
using RECEPTIO.CapaPresentacion.UI.MVVM;

namespace ControlesAccesoQR.ViewModels
{
    public class FingerprintPanelViewModel : ViewModelBase
    {
        private readonly IFingerprintWorkflow _workflow;
        private readonly CancellationTokenSource _cts;
        private string _status;

        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(nameof(Status)); }
        }

        public ICommand StartCaptureCommand { get; }
        public ICommand CancelCommand { get; }

        public FingerprintPanelViewModel(IFingerprintWorkflow workflow, CancellationTokenSource cts)
        {
            _workflow = workflow;
            _cts = cts;
            Status = "Preparando...";
            StartCaptureCommand = new RelayCommand(() => Status = "Coloque el dedo");
            CancelCommand = new RelayCommand(() => _cts.Cancel());
            _workflow.StatusChanged += s => Status = ToMessage(s);
        }

        private string ToMessage(FingerprintStatus status)
        {
            switch (status)
            {
                case FingerprintStatus.Preparing: return "Preparando...";
                case FingerprintStatus.PlaceFinger: return "Coloque el dedo";
                case FingerprintStatus.Capturing: return "Capturando...";
                case FingerprintStatus.Retrying: return "Reintentando...";
                case FingerprintStatus.Success: return "Éxito";
                case FingerprintStatus.Denied: return "Denegado";
                default: return string.Empty;
            }
        }
    }
}

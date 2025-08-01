using RECEPTIO.CapaPresentacion.UI.MVVM;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace ControlesAccesoQR.ViewModels.ControlesAccesoQR
{
    /// <summary>
    /// Base para los pasos del flujo de ingreso/salida.
    /// Replica la estructura de Transaction.EstadoProceso de manera simplificada.
    /// </summary>
    public abstract class EstadoProceso : ViewModelBase
    {
        private string _mensaje;
        private string _titulo;
        private Brush _colorTextoMensaje;
        private Visibility _esVisibleBoton;
        protected bool BotonPresionado;

        internal DispatcherTimer Dispatcher = new DispatcherTimer { Interval = new System.TimeSpan(0, 0, 2), IsEnabled = true };
        internal BackgroundWorker Worker = new BackgroundWorker { WorkerReportsProgress = true };

        public string Mensaje
        {
            get => _mensaje;
            set { _mensaje = value; OnPropertyChanged(nameof(Mensaje)); }
        }

        public string Titulo
        {
            get => _titulo;
            set { _titulo = value; OnPropertyChanged(nameof(Titulo)); }
        }

        public Visibility EsVisibleBoton
        {
            get => _esVisibleBoton;
            set { _esVisibleBoton = value; OnPropertyChanged(nameof(EsVisibleBoton)); }
        }

        public Brush ColorTextoMensaje
        {
            get => _colorTextoMensaje;
            set { _colorTextoMensaje = value; OnPropertyChanged(nameof(ColorTextoMensaje)); }
        }

        internal abstract void EstablecerControles(MainWindowViewModel viewModel);
        internal abstract void CambiarEstado();
    }
}

using RECEPTIO.CapaPresentacion.UI.MVVM;
using System;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Threading;

namespace TransactionEmpty.ViewModels
{
    internal abstract class EstadoProceso : ViewModelBase
    {
        private string _mensaje;
        private string _titulo;
        private Brush _colorTextoMensaje;
        protected bool BotonPresionado;
        internal DispatcherTimer Dispatcher = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 2), IsEnabled = true };
        internal BackgroundWorker Worker = new BackgroundWorker { WorkerReportsProgress = true };

        public string Mensaje
        {
            get
            {
                return _mensaje;
            }
            set
            {
                if (_mensaje == value) return;
                _mensaje = value;
                OnPropertyChanged("Mensaje");
            }
        }

        public string Titulo
        {
            get
            {
                return _titulo;
            }
            set
            {
                if (_titulo == value) return;
                _titulo = value;
                OnPropertyChanged("Titulo");
            }
        }

        public Brush ColorTextoMensaje
        {
            get
            {
                return _colorTextoMensaje;
            }
            set
            {
                if (Equals(_colorTextoMensaje, value)) return;
                _colorTextoMensaje = value;
                OnPropertyChanged("ColorTextoMensaje");
            }
        }

        internal abstract void EstablecerControles(VentanaPrincipalViewModel viewModel);

        internal abstract void CambiarEstado();
    }
}

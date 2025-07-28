using Console.ServicioConsole;
using RECEPTIO.CapaPresentacion.UWP.MVVM;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Popups;

namespace Console.ViewModels
{
    internal class VentanaAutorizacionViewModel : NotificationBase
    {
        #region Variables
        private readonly ServicioConsoleClient _servicio;
        private RelayCommand _comandoAceptar;
        private long _idPreGate;
        private string _usuario;
        private string _contrasena;
        private string _motivo;
        private bool _estaHabilitadoBoton;
        internal Tuple<bool, string> Resultado;
        private ObservableCollection<MOTIVE> _motivos;
        private MOTIVE _motivoSeleccionado;
        #endregion

        #region Constructor
        internal VentanaAutorizacionViewModel(ServicioConsoleClient servicio, string titulo)
        {
            _servicio = servicio;
            Titulo = titulo;
            _comandoAceptar = new RelayCommand(Aceptar, PuedoAceptar);
            PropertyChanged += VentanaAutorizacionViewModelPropertyChanged;
            ObtenerMotivos();
        }
        #endregion
       
        #region Propiedades
        public string Titulo { get; }

        public RelayCommand ComandoAceptar
        {
            get
            {
                return _comandoAceptar;
            }
            set
            {
                SetProperty(ref _comandoAceptar, value);
            }
        }

        public string Usuario
        {
            get
            {
                return _usuario;
            }
            set
            {
                if (_usuario == value)
                    return;
                _usuario = value;
                RaisePropertyChanged("Usuario");
            }
        }

        public string Contrasena
        {
            get
            {
                return _contrasena;
            }
            set
            {
                if (_contrasena == value)
                    return;
                _contrasena = value;
                RaisePropertyChanged("Contrasena");
            }
        }

        public long IdPreGate
        {
            get
            {
                return _idPreGate;
            }
            set
            {
                if (_idPreGate == value)
                    return;
                _idPreGate = value;
                RaisePropertyChanged("IdPreGate");
            }
        }

        public string Motivo
        {
            get
            {
                return _motivo;
            }
            set
            {
                if (_motivo == value)
                    return;
                _motivo = value;
                RaisePropertyChanged("Motivo");
            }
        }

        public bool EstaHabilitadoBoton
        {
            get
            {
                return _estaHabilitadoBoton;
            }
            set
            {
                if (_estaHabilitadoBoton == value)
                    return;
                _estaHabilitadoBoton = value;
                RaisePropertyChanged("EstaHabilitadoBoton");
            }
        }

        public ObservableCollection<MOTIVE> Motivos
        {
            get
            {
                return _motivos;
            }
            set
            {
                SetProperty(ref _motivos, value);
            }
        }

        public MOTIVE MotivoSeleccionado
        {
            get
            {
                return _motivoSeleccionado;
            }
            set
            {
                SetProperty(ref _motivoSeleccionado, value);
            }
        }
        #endregion

        #region Metodos
        private void VentanaAutorizacionViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _comandoAceptar.RaiseCanExecuteChanged();
            EstaHabilitadoBoton = PuedoAceptar(null);
        }

        public bool PuedoAceptar(object obj)
        {
            return !string.IsNullOrWhiteSpace(Usuario) && !string.IsNullOrWhiteSpace(Contrasena) && !string.IsNullOrWhiteSpace(Motivo) &&  _idPreGate > 0;
        }

        public async void Aceptar(object obj)
        {
            if (BotonPresionado)
                return;
            BotonPresionado = true;
            Resultado = await _servicio.AutenticarAccionAsync(Usuario, Contrasena);
            string v_result = await _servicio.ObtenerValidacionesGeneralesAsync("PREGATE", string.Empty, 0, IdPreGate);

            
 
            if (v_result != "")
            {
                Tuple<bool, string> Resultado1 =  new Tuple<bool, string>(false,"");
                Resultado = Resultado1;
                var mensajeDialogo = new MessageDialog(v_result, "PreGate Validación");
                await mensajeDialogo.ShowAsync();
            }
           
            BotonPresionado = false;
        }

        private async void ObtenerMotivos()
        {
            Motivos = await _servicio.ObtenerMotivosAsync(2);
            MotivoSeleccionado = Motivos.FirstOrDefault();
        }
        #endregion
    }
}

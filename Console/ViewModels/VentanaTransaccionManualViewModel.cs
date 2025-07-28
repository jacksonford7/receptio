using Console.ServicioConsole;
using RECEPTIO.CapaPresentacion.UWP.MVVM;
using System;
using Windows.UI.Popups;

namespace Console.ViewModels
{
    internal class VentanaTransaccionManualViewModel : NotificationBase
    {
        #region Variables
        private readonly ServicioConsoleClient _servicio;
        private DatosTransaccionManual _datosTransaccionManual;
        private string _mrn;
        private string _msn;
        private string _hsn;
        private string _numeroEntrega;
        private string _comentarios;
        private bool _estaHabilitadoBoton;
        private RelayCommand _comandoGrabar;
        #endregion

        #region Constructor
        internal VentanaTransaccionManualViewModel(ServicioConsoleClient servicio, DatosTransaccionManual datosTransaccionManual)
        {
            _servicio = servicio;
            _datosTransaccionManual = datosTransaccionManual;
            _comandoGrabar = new RelayCommand(Grabar, PuedoGrabar);
            PropertyChanged += VentanaTransaccionManualViewModelPropertyChanged;
        }
        #endregion

        #region Propiedades
        public RelayCommand ComandoGrabar
        {
            get
            {
                return _comandoGrabar;
            }
            set
            {
                SetProperty(ref _comandoGrabar, value);
            }
        }

        public string Mrn
        {
            get
            {
                return _mrn;
            }
            set
            {
                if (_mrn == value)
                    return;
                _mrn = value;
                RaisePropertyChanged("Mrn");
                _comandoGrabar.RaiseCanExecuteChanged();
            }
        }

        public string Msn
        {
            get
            {
                return _msn;
            }
            set
            {
                if (_msn == value)
                    return;
                _msn = value;
                RaisePropertyChanged("Msn");
                _comandoGrabar.RaiseCanExecuteChanged();
            }
        }

        public string Hsn
        {
            get
            {
                return _hsn;
            }
            set
            {
                if (_hsn == value)
                    return;
                _hsn = value;
                RaisePropertyChanged("Hsn");
                _comandoGrabar.RaiseCanExecuteChanged();
            }
        }

        public string NumeroEntrega
        {
            get
            {
                return _numeroEntrega;
            }
            set
            {
                if (_numeroEntrega == value)
                    return;
                _numeroEntrega = value;
                RaisePropertyChanged("NumeroEntrega");
                _comandoGrabar.RaiseCanExecuteChanged();
            }
        }

        public string Comentarios
        {
            get
            {
                return _comentarios;
            }
            set
            {
                if (_comentarios == value)
                    return;
                _comentarios = value;
                RaisePropertyChanged("Comentarios");
                _comandoGrabar.RaiseCanExecuteChanged();
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
        #endregion

        #region Metodos
        private void VentanaTransaccionManualViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _comandoGrabar.RaiseCanExecuteChanged();
            EstaHabilitadoBoton = PuedoGrabar(null);
        }

        private bool PuedoGrabar(object obj)
        {
            return !string.IsNullOrWhiteSpace(Mrn) && !string.IsNullOrWhiteSpace(Msn) && !string.IsNullOrWhiteSpace(Hsn) && !string.IsNullOrWhiteSpace(NumeroEntrega) && !string.IsNullOrWhiteSpace(Comentarios);
        }

        public async void Grabar(object obj)
        {
            if (BotonPresionado)
                return;
            BotonPresionado = true;
            _datosTransaccionManual.Comentarios = Comentarios;
            _datosTransaccionManual.Hsn = Hsn;
            _datosTransaccionManual.Mrn = Mrn;
            _datosTransaccionManual.Msn = Msn;
            _datosTransaccionManual.NumeroEntrega = NumeroEntrega;
            var resultado = await _servicio.AgregarTransaccionManualAsync(_datosTransaccionManual);
            MessageDialog mensajeDialogo;
            if (resultado.message == "Ok")
                mensajeDialogo = new MessageDialog("Proceso Ok.", "Transacción Manual SMDT");
            else
                mensajeDialogo = new MessageDialog($"{resultado.message}", "Transacción Manual SMDT");
            await mensajeDialogo.ShowAsync();
            BotonPresionado = false;
        }
        #endregion
    }
}

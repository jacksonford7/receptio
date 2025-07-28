using Console.ServicioConsole;
using Console.Vistas;
using RECEPTIO.CapaPresentacion.UWP.MVVM;
using System;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace Console.ViewModels
{
    internal class VentanaByPassViewModel : Base
    {
        #region Variables
        private ServicioConsoleClient _servicio;
        private RelayCommand _comandoConsultar;
        private RelayCommand _comandoGrabar;
        private RelayCommand _comandoLimpiar;
        private long _idPreGate;
        private string _motivo;
        private bool _estaHabilitadoByPass;
        private bool _esIngreso;
        private BY_PASS _byPass;
        #endregion

        #region Constructor
        internal VentanaByPassViewModel()
        {
            PropertyChanged += (s, e) => DelegarEventosCambiosAEventoComando();
            InstanciarComandos();
            InicializarServicioConsole();
            EsIngreso = true;
            EstaHabilitadoByPass = true;
        }

        private void DelegarEventosCambiosAEventoComando()
        {
            _comandoConsultar.RaiseCanExecuteChanged();
            _comandoGrabar.RaiseCanExecuteChanged();
            _comandoLimpiar.RaiseCanExecuteChanged();
        }

        private void InstanciarComandos()
        {
            _comandoConsultar = new RelayCommand(ConsultarAsync, PuedoConsultar);
            _comandoGrabar = new RelayCommand(GrabarAsync, PuedoGrabar);
            _comandoLimpiar = new RelayCommand(Limpiar);
        }
        #endregion

        #region Propiedades
        public RelayCommand ComandoConsultar
        {
            get
            {
                return _comandoConsultar;
            }
            set
            {
                SetProperty(ref _comandoConsultar, value);
            }
        }

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

        public RelayCommand ComandoLimpiar
        {
            get
            {
                return _comandoLimpiar;
            }
            set
            {
                SetProperty(ref _comandoLimpiar, value);
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

        public bool EstaHabilitadoByPass
        {
            get
            {
                return _estaHabilitadoByPass;
            }
            set
            {
                _estaHabilitadoByPass = value;
                RaisePropertyChanged("EstaHabilitadoByPass");
            }
        }

        public bool EsIngreso
        {
            get
            {
                return _esIngreso;
            }
            set
            {
                _esIngreso = value;
                RaisePropertyChanged("EsIngreso");
            }
        }
        #endregion

        #region Metodos
        private void InicializarServicioConsole()
        {
            _servicio = (ServicioConsoleClient)App.Current.Resources["ServicioConsole"];
        }

        private bool PuedoConsultar(object obj)
        {
            return _idPreGate > 0;
        }

        private async void ConsultarAsync(object obj)
        {
            var resultado = await _servicio.ObtenerByPassAsync(IdPreGate);
            if (resultado == null)
            {
                var mensajeDialogo = new MessageDialog("No existe un bypass para el código ingresado.", "ByPass");
                await mensajeDialogo.ShowAsync();
            }
            else
            {
                EsIngreso = false;
                _byPass = resultado;
                Motivo = _byPass.REASON;
                EstaHabilitadoByPass = _byPass.IS_ENABLED;
                var mensajeDialogo = new MessageDialog("By Pass es correcto. Ahora ingrese el motivo y luego presione grabar.", "ByPass");
                await mensajeDialogo.ShowAsync();
            }
        }

        private bool PuedoGrabar(object obj)
        {
            return IdPreGate > 0 && !string.IsNullOrWhiteSpace(Motivo);
        }

        private async void GrabarAsync(object obj)
        {
            if (EsIngreso)
            {
                var resultadoValidacion = await _servicio.ValidarIdPreGateAsync(IdPreGate);
                if (resultadoValidacion.FueOk)
                {
                    await _servicio.CrearByPassAsync(new BY_PASS
                    {
                        IS_ENABLED = EstaHabilitadoByPass,
                        REASON = Motivo,
                        PRE_GATE = new PRE_GATE { PRE_GATE_ID = IdPreGate }
                    }, ((DatosLogin)App.Current.Resources["DatosLogin"]).IdUsuario);
                    var mensajeDialogo = new MessageDialog("Proceso Ok.", "ByPass");
                    await mensajeDialogo.ShowAsync();
                }
                else
                {
                    var mensajeDialogo = new MessageDialog(resultadoValidacion.Mensaje, "ByPass");
                    await mensajeDialogo.ShowAsync();
                }
            }
            else
            {
                _byPass.REASON = Motivo;
                _byPass.IS_ENABLED = EstaHabilitadoByPass;
                await _servicio.ActualizarrByPassAsync(_byPass, ((DatosLogin)App.Current.Resources["DatosLogin"]).IdUsuario);
                var mensajeDialogo = new MessageDialog("Proceso Ok.", "ByPass");
                await mensajeDialogo.ShowAsync();
            }
            Limpiar(null);
        }

        private void Limpiar(object obj)
        {
            EsIngreso = true;
            _byPass = null;
            IdPreGate = 0;
            Motivo = null;
            EstaHabilitadoByPass = true;
        }
        #endregion
    }
}

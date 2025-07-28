using Console.ServicioConsole;
using Console.Vistas;
using RECEPTIO.CapaPresentacion.UWP.MVVM;
using System;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace Console.ViewModels
{
    internal class VentanaPreGateCancelViewModel : Base
    {
        #region Variables
        private ServicioConsoleClient _servicio;
        private RelayCommand _comandoGrabar;
        private RelayCommand _comandoLimpiar;
        private long _idPreGate;
        private string _motivo;
        private BY_PASS _byPass;
        #endregion

        #region Constructor
        internal VentanaPreGateCancelViewModel()
        {
            PropertyChanged += (s, e) => DelegarEventosCambiosAEventoComando();
            InstanciarComandos();
            InicializarServicioConsole();
        }

        private void DelegarEventosCambiosAEventoComando()
        {
            _comandoGrabar.RaiseCanExecuteChanged();
            _comandoLimpiar.RaiseCanExecuteChanged();
        }

        private void InstanciarComandos()
        {
            _comandoGrabar = new RelayCommand(GrabarAsync, PuedoGrabar);
            _comandoLimpiar = new RelayCommand(Limpiar);
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
        #endregion

        #region Metodos
        private void InicializarServicioConsole()
        {
            _servicio = (ServicioConsoleClient)App.Current.Resources["ServicioConsole"];
        }

         
        private bool PuedoGrabar(object obj)
        {
            return IdPreGate > 0 && !string.IsNullOrWhiteSpace(Motivo);
        }

        private async void GrabarAsync(object obj)
        {
            var resultadoValidacion = await _servicio.ValidarIdPreGateParaCancelarAsync(IdPreGate);
            if (resultadoValidacion.FueOk)
            {
                await _servicio.CrearByPassCancelPregateAsync(new BY_PASS
                {
                    IS_ENABLED = false,
                    REASON = "PREGATE CANCEL : " + Motivo,
                    PRE_GATE = new PRE_GATE { PRE_GATE_ID = IdPreGate }
                }, ((DatosLogin)App.Current.Resources["DatosLogin"]).IdUsuario);

                await _servicio.ActualizarStatusPregateAsync(IdPreGate, "C");
                
                var mensajeDialogo = new MessageDialog("Proceso Ok.", "PreGate Cancel");
                await mensajeDialogo.ShowAsync();
            }
            else
            {
                var mensajeDialogo = new MessageDialog(resultadoValidacion.Mensaje, "PreGate Cancel");
                await mensajeDialogo.ShowAsync();
            }
            Limpiar(null);
        }

        private void Limpiar(object obj)
        {
            _byPass = null;
            IdPreGate = 0;
            Motivo = null;
        }
        #endregion
    }
}

using RECEPTIO.CapaPresentacion.UI.MVVM;
using System.Windows;
using System.Windows.Input;
using Transaction.ServicioTransaction;

namespace Transaction.ViewModels
{
    internal class VentanaAutorizacionViewModel : ViewModelBase
    {
        #region Variables
        private readonly Window _ventana;
        private readonly ServicioTransactionClient _servicio;
        private string _usuario;
        private string _contrasena;
        #endregion

        #region Constructor
        internal VentanaAutorizacionViewModel(Window ventana, ServicioTransactionClient servicio, string tag)
        {
            _ventana = ventana;
            _servicio = servicio;
            Tag = tag.Split(':')[1];
        }
        #endregion

        #region Propiedades
        public ICommand ComandoContinuar
        {
            get
            {
                return new RelayCommand(Continuar, PuedoContinuar);
            }
        }

        public string Tag { get; }

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
                OnPropertyChanged("Usuario");
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
                OnPropertyChanged("Contrasena");
            }
        }
        #endregion

        #region Metodos
        private bool PuedoContinuar()
        {
            return !string.IsNullOrWhiteSpace(Usuario) && !string.IsNullOrWhiteSpace(Contrasena);
        }

        private void Continuar()
        {
            var resultado = _servicio.AutenticarAccion(Usuario, Contrasena);
            if (resultado.Item1)
                _ventana.DialogResult = true;
            else
                MessageBox.Show(resultado.Item2, "TRANSACTION", MessageBoxButton.OK, MessageBoxImage.Exclamation);                
        }
        #endregion
    }
}

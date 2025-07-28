using Console.ServicioConsole;
using Console.Vistas;
using RECEPTIO.CapaPresentacion.UWP.MVVM;
using System;
using System.Net;
using System.ServiceModel;
using Windows.UI.Popups;

namespace Console.ViewModels
{
    internal class VentanaAutenticacionViewModel : NotificationBase
    {
        #region Variables
        private readonly VentanaAutenticacion _ventanaAutenticacion;
        private string _usuario;
        private string _contrasena;
        private string _ip;
        private RelayCommand _comandoLogin;
        #endregion

        #region Constructor
        internal VentanaAutenticacionViewModel(VentanaAutenticacion ventanaAutenticacion)
        {
            _ventanaAutenticacion = ventanaAutenticacion;
            _comandoLogin = new RelayCommand(Ingresar, PuedoIngresar);
            PropertyChanged += (s, e) => _comandoLogin.RaiseCanExecuteChanged();
            ObtenerIp();
            InicializarServicioConsole();
        }
        #endregion

        #region Propiedades
        public RelayCommand ComandoLogin
        {
            get
            {
                return _comandoLogin;
            }
            set
            {
                SetProperty(ref _comandoLogin, value);
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
        #endregion

        #region Metodos
        private async void ObtenerIp()
        {
            var ip = await Dns.GetHostEntryAsync(Dns.GetHostName());
            _ip = ip.AddressList[1].ToString();
        }

        private void InicializarServicioConsole()
        {
            if (!App.Current.Resources.Keys.Contains("ServicioConsole"))
            {
                var binding = new BasicHttpBinding
                {
                    MaxBufferPoolSize = 2147483647,
                    MaxBufferSize = 2147483647,
                    MaxReceivedMessageSize = 2147483647
                };
                binding.ReaderQuotas.MaxArrayLength = 2147483647;
                binding.ReaderQuotas.MaxDepth = 2147483647;
                binding.ReaderQuotas.MaxStringContentLength = 2147483647;
                Windows.ApplicationModel.Resources.Core.ResourceContext ctx = new Windows.ApplicationModel.Resources.Core.ResourceContext();
                Windows.ApplicationModel.Resources.Core.ResourceMap rmap = Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap.GetSubtree("Recursos");
                var endpoint = rmap.GetValue("DireccionServicio", ctx).ValueAsString;
                App.Current.Resources.Add("ServicioConsole", new ServicioConsoleClient(binding, new EndpointAddress(endpoint)));
            }
        }

        public bool PuedoIngresar(object obj)
        {
            return !string.IsNullOrWhiteSpace(Usuario) && !string.IsNullOrWhiteSpace(Contrasena);
        }

        public async void Ingresar(object obj)
        {
            if (BotonPresionado)
                return;
            BotonPresionado = true;
            EstaOcupado = true;
            var servicio = (ServicioConsoleClient)App.Current.Resources["ServicioConsole"];
            var resultado = await servicio.AutenticarAsync(Usuario, Contrasena, _ip);
            if (resultado.EstaAutenticado)
            {
                GuardarRecursosAplicacion(resultado);
                IrVentanaPrincipal(resultado.EsLider);
            }
            else
            {
                var mensajeDialogo = new MessageDialog(resultado.Mensaje, "Autenticación Fallida.");
                await mensajeDialogo.ShowAsync();
                EstaOcupado = false;
                BotonPresionado = false;
            }
        }

        private void GuardarRecursosAplicacion(DatosLogin resultado)
        {
            resultado.Usuario = Usuario;
            resultado.Ip = _ip;
            App.Current.Resources.Add("DatosLogin", resultado);
        }

        private void IrVentanaPrincipal(bool esLider)
        {
            if(esLider)
                _ventanaAutenticacion.Frame.Navigate(typeof(VentanaSupervisor));
            else
                _ventanaAutenticacion.Frame.Navigate(typeof(VentanaPrincipal));
        }
        #endregion
    }
}

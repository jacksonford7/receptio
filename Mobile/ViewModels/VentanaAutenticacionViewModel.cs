using Mobile.ServicioAutenticacion;
using Mobile.Vistas;
using RECEPTIO.CapaPresentacion.UWP.MVVM;
using System;
using System.Net;
using System.ServiceModel;
using Windows.UI.Popups;
using Mobile.ServicioMobile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI;

namespace Mobile.ViewModels
{
    internal class VentanaAutenticacionViewModel : NotificationBase
    {
        #region Variables
        private readonly VentanaAutenticacion _ventanaAutenticacion;
        private string _usuario;
        private string _contrasena;
        private string _type;
        private string _ip;
        private string _zona;
        private string _host;
        SolidColorBrush _gcolor;
        public bool bvalida = false;
        #endregion

        #region Constructor
        internal VentanaAutenticacionViewModel(VentanaAutenticacion ventanaAutenticacion)
        {
            _ventanaAutenticacion = ventanaAutenticacion;
            //Contrasena = "CGS@it2018!7";
            ObtenerIp();
            GColor = new SolidColorBrush(Colors.Transparent);
        }
        #endregion

        #region Propiedades
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

        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                if (_type == value)
                    return;
                _type = value;
                RaisePropertyChanged("Tipo");
            }
        }

        public string Ip
        {
            get
            {
                return _ip;
            }
            set
            {
                if (_ip == value)
                    return;
                _ip = value;
                RaisePropertyChanged("Ip");
            }
        }

        public string Host
        {
            get
            {
                return _host;
            }
            set
            {
                if (_host == value)
                    return;
                _host = value;
                RaisePropertyChanged("Host");
            }
        }

        public string Zona
        {
            get
            {
                return _zona;
            }
            set
            {
                if (_zona == value)
                    return;
                _zona = value;
                RaisePropertyChanged("Zona");
            }
        }

        public SolidColorBrush GColor
        {
            get
            {
                return _gcolor;
            }
            set
            {
                if (_gcolor == value)
                    return;
                _gcolor = value;
                RaisePropertyChanged("GColor");
            }
        }
        #endregion

        #region Metodos
        private async void ObtenerIp()
        {
            var ip = await Dns.GetHostEntryAsync(Dns.GetHostName());
            Ip = ip.AddressList[1].ToString();
            Host = Dns.GetHostName();
        }

        private bool PuedoIngresar()
        {
            return !string.IsNullOrWhiteSpace(Usuario) && !string.IsNullOrWhiteSpace(Contrasena) && !string.IsNullOrWhiteSpace(Type);
        }

        public async void Ingresar()
        {
            bvalida = false;
            if (!PuedoIngresar())
            {
                bvalida = true;
                return;
            }  
            // Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Wait, 10);
            //GColor = new SolidColorBrush(Colors.DarkOrange);
                
            var binding = new BasicHttpBinding();
            Windows.ApplicationModel.Resources.Core.ResourceContext ctx = new Windows.ApplicationModel.Resources.Core.ResourceContext();
            Windows.ApplicationModel.Resources.Core.ResourceMap rmap = Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap.GetSubtree("Recursos");
            var endpoint = rmap.GetValue("DireccionServicio", ctx).ValueAsString;
            var servicio = new ServicioLoginMobileClient(binding, new EndpointAddress(endpoint));
            var resultado = await servicio.AutenticarAsync(Usuario, Contrasena, _ip);

            if (resultado.EstaAutenticado)
            {
                GuardarRecursosAplicacion(resultado);
                Zona = resultado.Zona;

                if (Type == "INGRESO")
                    IrVentanaPrincipal(resultado.EsLider);
                else
                    IrVentanaTransac(true);
            }
            else
            {
                var mensajeDialogo = new MessageDialog(resultado.Mensaje, "Autenticación Fallida.");
                await mensajeDialogo.ShowAsync();
            }
            bvalida = true;

        }

        private void GuardarRecursosAplicacion(DatosLogin resultado)
        {
            App.Current.Resources.Remove("LoginData");
            App.Current.Resources.Add("LoginData", resultado);
            App.Current.Resources.Remove("UserData");
            App.Current.Resources.Add("UserData", this);
            //((DatosLogin)(App.Current.Resources["LoginData"]))
            //object y;
            //App.Current.Resources.TryGetValue("LoginData", out y);
        }

        public void IrVentanaPrincipal(bool esLider)
        {
            _ventanaAutenticacion.Frame.Navigate(typeof(ValidatePage));
        }

        public void IrVentanaTransac(bool esLider)
        {
           _ventanaAutenticacion.Frame.Navigate(typeof(OutPage));
        }
        #endregion
    }
}

using Mobile.ServicioAutenticacion;
using Mobile.Vistas;
using Mobile.ViewModels;
using RECEPTIO.CapaPresentacion.UWP.MVVM;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.ServiceModel;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Mobile.ServiceTransact;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Xaml;
using System.Threading.Tasks;

namespace Mobile.ViewModels
{
    internal class ValidatePageViewModel : NotificationBase
    {
        #region Variables
        private readonly ValidatePage _validaPage;
        private ObservableCollection<DatoGridViewModel> _tb;
        private string _usuario;
        private string _chofer;
        private string _contrasena;
        private string _nombre;
        private string _empresa;
        private string _ip;
        private string _zona;
        private string _host;
        private bool _bchf = true;
        private bool _bplc = true;
        private bool bchof = false;
        private bool bplac = false;
        public bool bvalida = true;
        private Visibility _vs;
        private SolidColorBrush _gcolor;
        private ServiceClient s;
        #endregion

        #region Constructor
        internal ValidatePageViewModel(ValidatePage ventanaAutenticacion)
        {
            _validaPage = ventanaAutenticacion;
            ObtenerIp();
            Vs = new Visibility();
            Vs = Visibility.Visible;
            GColor = new SolidColorBrush(Colors.Green);
            //s = new ServiceClient();

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
            var endpoint = rmap.GetValue("ServiceTransact", ctx).ValueAsString;
            s = new ServiceClient(binding, new EndpointAddress(endpoint));

            Usuario = ((VentanaAutenticacionViewModel)(App.Current.Resources["UserData"])).Usuario;
            Host = ((VentanaAutenticacionViewModel)(App.Current.Resources["UserData"])).Host;
            Zona = ((VentanaAutenticacionViewModel)(App.Current.Resources["UserData"])).Zona;
            //tb = new ObservableCollection<DatoGridViewModel>();
        }
        #endregion

        #region Propiedades
        public string Chofer
        {
            get
            {
                return _chofer;
            }
            set
            {
                if (_chofer == value)
                    return;
                _chofer = value;
                RaisePropertyChanged("Chofer");
            }
        }

        public string Nombre
        {
            get
            {
                return _nombre;
            }
            set
            {
                if (_nombre == value)
                    return;
                _nombre = value;
                RaisePropertyChanged("Nombre");
            }
        }

        public string Placa
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
                RaisePropertyChanged("Placa");
            }
        }

        public string Empresa
        {
            get
            {
                return _empresa;
            }
            set
            {
                if (_empresa == value)
                    return;
                _empresa = value;
                RaisePropertyChanged("Empresa");
            }
        }

        public bool bPlaca
        {
            get
            {
                return _bplc;
            }
            set
            {
                if (_bplc == value)
                    return;
                _bplc = value;
                RaisePropertyChanged("bPlaca");
            }
        }

        public bool bChofer
        {
            get
            {
                return _bchf;
            }
            set
            {
                if (_bchf == value)
                    return;
                _bchf = value;
                RaisePropertyChanged("bChofer");
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

        public Visibility Vs
        {
            get
            {
                return _vs;
            }
            set
            {
                if (_vs == value)
                    return;
                _vs = value;
                RaisePropertyChanged("Vs");
            }
        }

        public ObservableCollection<DatoGridViewModel> tb
        {
            get
            {
                return _tb;
            }
            set
            {
                SetProperty(ref _tb, value);
            }
        }

        #endregion

        #region Metodos
        private async void ObtenerIp()
        {
            var ip = await Dns.GetHostEntryAsync(Dns.GetHostName());
            Ip = ip.AddressList[1].ToString();
        }

        private bool PuedoIngresar()
        {
            //return !string.IsNullOrWhiteSpace(Chofer) && !string.IsNullOrWhiteSpace(Placa);
            if (string.IsNullOrWhiteSpace(Chofer))
            {
                GColor = new SolidColorBrush(Colors.Red);
                LlenarGrid("CHOFER", "FALTA INGRESAR DATO", new SolidColorBrush(Colors.Red));
            }
            //else
            //    bchof = true;

            if (string.IsNullOrWhiteSpace(Placa))
            {
                GColor = new SolidColorBrush(Colors.Red);
                LlenarGrid("CAMION", "FALTA INGRESAR DATO", new SolidColorBrush(Colors.Red));
            }
            //else
            //    bplac = true;

            return !string.IsNullOrWhiteSpace(Chofer) && !string.IsNullOrWhiteSpace(Placa);
        }

        public async void Ingresar()
        {
            if (!bvalida)
                return;
            bvalida = false;
            if (!(bchof && bplac))
            {
                var mensajeDialogo = new MessageDialog("No puede continuar", "Verificar mensajes presentados o Realizar la validación correspondiente.");
                await mensajeDialogo.ShowAsync();
                bvalida = true;
                return;
            }
            //var binding = new BasicHttpBinding();
            //Windows.ApplicationModel.Resources.Core.ResourceContext ctx = new Windows.ApplicationModel.Resources.Core.ResourceContext();
            //Windows.ApplicationModel.Resources.Core.ResourceMap rmap = Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap.GetSubtree("Recursos");
            //var endpoint = rmap.GetValue("DireccionServicio", ctx).ValueAsString;
            //var servicio = new ServicioConsoleClient(binding, new EndpointAddress(endpoint));
            //var resultado = await servicio.AutenticarAsync(Chofer, Placa, _ip);
            //if (resultado.EstaAutenticado)
            //{
            GuardarRecursosAplicacion(this);
            bvalida = true;
            IrVentanaPrincipal(true);
            //}
            //else
            //{
            //    var mensajeDialogo = new MessageDialog(resultado.Mensaje, "Autenticación Fallida.");
            //    await mensajeDialogo.ShowAsync();
            //}
        }

        //public async Task<bool> Validar()
        public async void Validar()
        {
            if (!bvalida)
                return;
            tb = new ObservableCollection<DatoGridViewModel>();
            bchof = false;
            bplac = false;
            bvalida = false;

            if (!PuedoIngresar())
            {
                var mensajeDialogo = new MessageDialog("No puede continuar", "Llenar los campos faltantes.");
                await mensajeDialogo.ShowAsync();
                bvalida = true;
                return;
            }

            try
            { 

            Vs = Visibility.Collapsed;
            //var mensajeDialogo1 = new MessageDialog("Prueba Carga", "Prueba");
            //await mensajeDialogo1.ShowAsync();

            var r = await s.get_driver_listAsync(Chofer.Trim());

            if (r.Count > 0)
                LlenarChofer(r);
            else
            {
                LlenarGrid("CHOFER", "NO HA SIDO ENCONTRADO", new SolidColorBrush(Colors.Red));
                bvalida = true;
                return;
            }

            var t = new check_driver_expoRequest();
            t.driver = Chofer;

            var z = new check_driver_expoResponse();
            z = await s.check_driver_expoAsync(t);

            if (!z.check_driver_expoResult)
            {
                LlenarGrid("CHOFER", z.msg, new SolidColorBrush(Colors.Red));
            }
            else
            {
                //GColor = new SolidColorBrush(Colors.Green);
                LlenarGrid("CHOFER", "AUTORIZADO", new SolidColorBrush(Colors.Green));
                bchof = true;
            }

            /*

            Nombre = "RICARDO RODRIGUEZ";

            binding = new BasicHttpBinding();
            ctx = new Windows.ApplicationModel.Resources.Core.ResourceContext();
            rmap = Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap.GetSubtree("Recursos");
            var endpoint = rmap.GetValue("ServiceTransact", ctx).ValueAsString;
            var servicio = new ServiceClient(binding, new EndpointAddress(endpoint));
            var resultado = await servicio.check_driver_expoAsync(t);
            */

            var x = new check_truck_expoRequest();
            x.driver = Placa;

            var y = new check_truck_expoResponse();
            y = await s.check_truck_expoAsync(x);

            if (!y.check_truck_expoResult)
            {
                LlenarGrid("CAMION", y.msg, new SolidColorBrush(Colors.Red));
            }
            else
            {
                //GColor = new SolidColorBrush(Colors.Green);
                LlenarGrid("CAMION", "AUTORIZADO", new SolidColorBrush(Colors.Green));
                bplac = true;
            }

            bPlaca = false;
            bChofer = false;
            Vs = Visibility.Visible;
            //await mensajeDialogo1.ShowAsync();
            //return true;
            }catch(Exception e)
            {
                LlenarGrid("Error Validar", e.Message, new SolidColorBrush(Colors.Red));
            }
            bvalida = true;
        }

        public async void Vacios()
        {
            if (!bvalida)
                return;
            bvalida = false;
            if (!(bchof && bplac))
            {
                var mensajeDialogo = new MessageDialog("No puede continuar", "Verificar mensajes presentados o Realizar la validación correspondiente.");
                await mensajeDialogo.ShowAsync();
                bvalida = true;
                return;
            }
            //var binding = new BasicHttpBinding();
            //Windows.ApplicationModel.Resources.Core.ResourceContext ctx = new Windows.ApplicationModel.Resources.Core.ResourceContext();
            //Windows.ApplicationModel.Resources.Core.ResourceMap rmap = Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap.GetSubtree("Recursos");
            //var endpoint = rmap.GetValue("DireccionServicio", ctx).ValueAsString;
            //var servicio = new ServicioConsoleClient(binding, new EndpointAddress(endpoint));
            //var resultado = await servicio.AutenticarAsync(Chofer, Placa, _ip);
            //if (resultado.EstaAutenticado)
            //{
            GuardarRecursosAplicacion(this);
            bvalida = true;
            IrVentanaVacios(true);
            //}
            //else
            //{
            //    var mensajeDialogo = new MessageDialog(resultado.Mensaje, "Autenticación Fallida.");
            //    await mensajeDialogo.ShowAsync();
            //}
        }

        public async void Servicios()
        {
            if (!bvalida)
                return;
            bvalida = false;
            if (!(bchof && bplac))
            {
                var mensajeDialogo = new MessageDialog("No puede continuar", "Verificar mensajes presentados o Realizar la validación correspondiente.");
                await mensajeDialogo.ShowAsync();
                bvalida = true;
                return;
            }
            //var binding = new BasicHttpBinding();
            //Windows.ApplicationModel.Resources.Core.ResourceContext ctx = new Windows.ApplicationModel.Resources.Core.ResourceContext();
            //Windows.ApplicationModel.Resources.Core.ResourceMap rmap = Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap.GetSubtree("Recursos");
            //var endpoint = rmap.GetValue("DireccionServicio", ctx).ValueAsString;
            //var servicio = new ServicioConsoleClient(binding, new EndpointAddress(endpoint));
            //var resultado = await servicio.AutenticarAsync(Chofer, Placa, _ip);
            //if (resultado.EstaAutenticado)
            //{
            GuardarRecursosAplicacion(this);
            bvalida = true;
            IrVentanaServicios(true);
            //IrVentanaSAV(true);
            //}
            //else
            //{
            //    var mensajeDialogo = new MessageDialog(resultado.Mensaje, "Autenticación Fallida.");
            //    await mensajeDialogo.ShowAsync();
            //}
        }

        public async void ServicioSav()
        {
            if (!bvalida)
                return;
            bvalida = false;
            if (!(bchof && bplac))
            {
                var mensajeDialogo = new MessageDialog("No puede continuar", "Verificar mensajes presentados o Realizar la validación correspondiente.");
                await mensajeDialogo.ShowAsync();
                bvalida = true;
                return;
            }
            //var binding = new BasicHttpBinding();
            //Windows.ApplicationModel.Resources.Core.ResourceContext ctx = new Windows.ApplicationModel.Resources.Core.ResourceContext();
            //Windows.ApplicationModel.Resources.Core.ResourceMap rmap = Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap.GetSubtree("Recursos");
            //var endpoint = rmap.GetValue("DireccionServicio", ctx).ValueAsString;
            //var servicio = new ServicioConsoleClient(binding, new EndpointAddress(endpoint));
            //var resultado = await servicio.AutenticarAsync(Chofer, Placa, _ip);
            //if (resultado.EstaAutenticado)
            //{
            GuardarRecursosAplicacion(this);
            bvalida = true;
            //IrVentanaServicios(true);
            IrVentanaSAV(true);
            //}
            //else
            //{
            //    var mensajeDialogo = new MessageDialog(resultado.Mensaje, "Autenticación Fallida.");
            //    await mensajeDialogo.ShowAsync();
            //}
        }

        public void Regresar()
        {
            //var mensajeDialogo = new MessageDialog("No puede continuar", tb[0].GColor.ToString());
            //await mensajeDialogo.ShowAsync();
            //return;
            if (!bvalida)
                return;
            IrVentanaLogin(true);
        }

        public async void CargaNombre()     
        {
            var r = await s.get_driver_listAsync(Chofer);

            if (r!=null)
                LlenarChofer(r);
        }

        public void Limpiar()
        {
            if (!bvalida)
                return;
            LimpiarCampos();
        }

        private void LimpiarCampos()
        {
            Chofer = "";
            Nombre = "";
            Placa = "";
            bPlaca = true;
            bChofer = true;
            bchof = false;
            bplac = false;
            tb = new ObservableCollection<DatoGridViewModel>();
        }

        private void GuardarRecursosAplicacion(ValidatePageViewModel resultado)
        {
            App.Current.Resources.Remove("ValidaData");
            App.Current.Resources.Add("ValidaData", resultado);
        }

        private void IrVentanaPrincipal(bool esLider)
        {
            _validaPage.Frame.Navigate(typeof(MainPage));
        }

        private void IrVentanaVacios(bool esLider)
        {
            _validaPage.Frame.Navigate(typeof(EmptyPage));
        }

        private void IrVentanaServicios(bool esLider)
        {
            _validaPage.Frame.Navigate(typeof(ProveedorPage));
        }

        private void IrVentanaSAV(bool esLider)
        {
            _validaPage.Frame.Navigate(typeof(SAVPage));
        }

        private void IrVentanaLogin(bool esLider)
        {
            _validaPage.Frame.Navigate(typeof(VentanaAutenticacion));
        }

        private void LlenarGrid(string dat, string inf, SolidColorBrush col)
        {
            
            var obj = new DatoGridViewModel();
            obj.Dato = dat;
            obj.Info = inf;
            //obj.GColor = "Red"; //new SolidColorBrush(Colors.Red);
            obj.GColor2 = col;
            tb.Add(obj);

        }

        private void LlenarChofer(ObservableCollection<combo_item> r)
        {
            Nombre = r[0].value;
            Empresa = r[0].name;
            //Nombre = "STALIN ALVARADO";
        }
        #endregion
    }
}

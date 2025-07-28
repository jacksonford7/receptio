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
using Mobile.ServiceSMDT;
using Mobile.ServiceTD;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mobile.ServicioMobile;
using System.Text;
using Windows.UI.Xaml.Media;
using Windows.UI;
using System.Xml.Linq;
using System.Xml;


namespace Mobile.ViewModels
{
    internal class OutPageViewModel : NotificationBase
    {
        #region Variables
        private readonly OutPage _validaPage;
        private ObservableCollection<DatoGridViewModel> _tb;
        private ObservableCollection<combo_item> _lsColor;
        private List<string> _lsCol;
        private string usuario;
        private string _usuario;
        private string _contenedor;
        private string _host;
        private string _zona;
        private string _contrasena;
        private string _nombre;
        private string _satelite;
        private string _ip;
        private bool _bcontain = true;
        private bool _bseal = true;
        private bool _bcolor = true;
        private bool _bsatel = false;
        private bool bcont=false;
        private bool bsel=false;
        private string gkey;
        private string valida;
        private string peso;
        private string mns;
        private bool val;
        public bool bvalida = true;
        SolidColorBrush _gcolor;
        private ServiceClient s;
        private n4ServiceSoapClient sm;
        private ServicioMobileClient w;
        #endregion

        #region Constructor
        internal OutPageViewModel(OutPage ventanaAutenticacion)
        {
            _validaPage = ventanaAutenticacion;
            ObtenerIp();
            usuario = ((VentanaAutenticacionViewModel)(App.Current.Resources["UserData"])).Usuario;
            //s = new ServiceClient();
            //td = new ServicioAnuncianteProblemaClient();
            sm = new n4ServiceSoapClient();
            lsColor = new ObservableCollection<combo_item>();
            lsCol = new List<string>();
            GColor = new SolidColorBrush(Colors.Green);
            //w = new ServicioMobileClient();

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

            var endpoint1 = rmap.GetValue("DireccionServicioMobile", ctx).ValueAsString;
            w = new ServicioMobileClient(binding, new EndpointAddress(endpoint1));


            Usuario = usuario;
            Host = ((VentanaAutenticacionViewModel)(App.Current.Resources["UserData"])).Host;
            Zona = ((VentanaAutenticacionViewModel)(App.Current.Resources["UserData"])).Zona;
            ListColor();
        }
        #endregion

        #region Propiedades
        public string Contenedor
        {
            get
            {
                return _contenedor;
            }
            set
            {
                if (_contenedor == value)
                    return;
                _contenedor = value;
                RaisePropertyChanged("Contenedor");
            }
        }

        public string Sello
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
                RaisePropertyChanged("Sello");
            }
        }

        public string Color
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
                RaisePropertyChanged("Color");
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

        public string Satelite
        {
            get
            {
                return _satelite;
            }
            set
            {
                if (_satelite == value)
                    return;
                _satelite = value;
                RaisePropertyChanged("Satelite");
            }
        }

        public bool bContain
        {
            get
            {
                return _bcontain;
            }
            set
            {
                if (_bcontain == value)
                    return;
                _bcontain = value;
                RaisePropertyChanged("bContain");
            }
        }

        public bool bSeal
        {
            get
            {
                return _bseal;
            }
            set
            {
                if (_bseal == value)
                    return;
                _bseal = value;
                RaisePropertyChanged("bSeal");
            }
        }

        public bool bColor
        {
            get
            {
                return _bcolor;
            }
            set
            {
                if (_bcolor == value)
                    return;
                _bcolor = value;
                RaisePropertyChanged("bColor");
            }
        }

        public bool bSatel
        {
            get
            {
                return _bsatel;
            }
            set
            {
                if (_bsatel == value)
                    return;
                _bsatel = value;
                RaisePropertyChanged("bSatel");
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

        public ObservableCollection<combo_item> lsColor
        {
            get
            {
                return _lsColor;
            }
            set
            {
                SetProperty(ref _lsColor, value);
            }
        }

        public List<string> lsCol
        {
            get
            {
                return _lsCol;
            }
            set
            {
                SetProperty(ref _lsCol, value);
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
            if (string.IsNullOrWhiteSpace(Contenedor))
                LlenarGrid("CONTENEDOR", "FALTA INGRESAR DATO", new SolidColorBrush(Colors.Red));

            if (string.IsNullOrWhiteSpace(Sello))
                LlenarGrid("SELLO", "FALTA INGRESAR DATO", new SolidColorBrush(Colors.Red));

            if (string.IsNullOrWhiteSpace(Color))
                LlenarGrid("COLOR", "FALTA INGRESAR DATO", new SolidColorBrush(Colors.Red));

            return !string.IsNullOrWhiteSpace(Contenedor) && !string.IsNullOrWhiteSpace(Sello) && !string.IsNullOrWhiteSpace(Color);
        }

        private async void ListColor()
        {
            lsColor = await s.get_seals_colorsAsync();

            for (int i = 1; i< lsColor.Count; i ++)
            {
                lsCol.Add(lsColor[i].value);//, lsColor[i].value);
            }

            if (lsCol.Count == 0)
            {
                lsCol.Add("BLANCO");
                Color = "BLANCO";
            }
            //Color = lsColor[0].value;
        }

        public async void Ingresar()
        {
            if(!bvalida)
                return;
            bvalida = false;

            if (!(bcont && bsel))
            {
                var mensajeDialogo = new MessageDialog("No puede continuar", "Verificar mensajes presentados o validar la información.");
                await mensajeDialogo.ShowAsync();
                //LlenarGrid("No puede continuar", "Verificar mensajes presentados o validar la información", new SolidColorBrush(Colors.Red));
                bvalida = true;
                return;
            }

            if (bSatel == true && (Satelite == null || Satelite.Trim().Length == 0))
            {
                LlenarGrid("Problema", "Proceda a llenar el sello satelital", new SolidColorBrush(Colors.Red));
                bvalida = true;
                return;
            }

            try
            { 
                var smdt = ArmarXml(usuario, valida, gkey.ToString(), Satelite, peso);
                var smr = await sm.basicInvokeAsync("", smdt);
                //var reader = new XDocument();
                //reader = XDocument.Parse(smr.Body.basicInvokeResult);

                if (smr.Body.basicInvokeResult.Contains("<Code>0</Code>"))
                {
                    //GuardarRecursosAplicacion(this);
                    //IrVentanaPrincipal(true);
                    //var t = reader.Descendants("ticket");

                    await LiberaHold(Contenedor);

                    if (val)
                    {
                        var a = smr.Body.basicInvokeResult.IndexOf("<ticket>");
                        var b = smr.Body.basicInvokeResult.IndexOf("</ticket>");
                        var mensajeDialogo = new MessageDialog("Transacción", "SMDT Generado "+ smr.Body.basicInvokeResult.Substring(a + 8, b - a-8));
                        await mensajeDialogo.ShowAsync();
                        //LlenarGrid("Transacción", "Contenedor Liberado Generado", new SolidColorBrush(Colors.Green));
                    }
                    //else
                    //    LlenarGrid("Hold", "No se pudo liberar Contenedor", new SolidColorBrush(Colors.Red));
                }
                else
                {
                    //var mensajeDialogo = new MessageDialog("Transacción", "Proceso Satisfactorio.");
                    //await mensajeDialogo.ShowAsync();
                    //var  t= reader.Descendants("error");
                    //XmlElement el;
                    //el = t.Elements();
                    var a = smr.Body.basicInvokeResult.IndexOf("<error>");
                    var b = smr.Body.basicInvokeResult.IndexOf("</error>");
                    LlenarGrid("Error", smr.Body.basicInvokeResult.Substring(a+7,b-a-7), new SolidColorBrush(Colors.Red));
                }

            //var binding = new BasicHttpBinding();
            //    Windows.ApplicationModel.Resources.Core.ResourceContext ctx = new Windows.ApplicationModel.Resources.Core.ResourceContext();
            //    Windows.ApplicationModel.Resources.Core.ResourceMap rmap = Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap.GetSubtree("Recursos");
            //    var endpoint = rmap.GetValue("ServicioTD", ctx).ValueAsString;
            //    var servicio = new ServicioConsoleClient(binding, new EndpointAddress(endpoint));
            //await servicio.CrearAutoTicketAsync(1, 1024458);
            }
            catch (Exception e)
            {
                LlenarGrid("Error Ingreso", e.Message, new SolidColorBrush(Colors.Red));
            }
            bvalida = true;
        }

        public async void Validar()
        {
            if (!bvalida)
                return;

            tb = new ObservableCollection<DatoGridViewModel>();
            bcont = false;
            bsel = false;
            bvalida = false;
            valida = "0";

            if (!PuedoIngresar())
            {
                var mensajeDialogo = new MessageDialog("No puede continuar", "Llenar los campos faltantes.");
                await mensajeDialogo.ShowAsync();
                //LlenarGrid("Problema", "Llenar los campos faltantes", new SolidColorBrush(Colors.Red));
                bvalida = true;
                return;
            }

            var cnt = new find_impo_cntr_pre_outRequest(Contenedor.Trim().ToUpper());
            var cnta = await s.find_impo_cntr_pre_outAsync(cnt);
            gkey = "0";


            if (cnta.find_impo_cntr_pre_outResult != null)
            {
                var gk = long.Parse(cnta.find_impo_cntr_pre_outResult.gkey.ToString().Substring(0, cnta.find_impo_cntr_pre_outResult.gkey.ToString().Length-3));
                gkey = gk.ToString(); 
                peso = cnta.find_impo_cntr_pre_outResult.actual_weight.ToString();
                valida = "0";

                if (cnta.find_impo_cntr_pre_outResult.region == "T")
                {
                    bSatel = true;
                }
                bcont = true;
            }
            else
            {
                //var mensajeDialogo = new MessageDialog("Problema", cnta.msg);
                //await mensajeDialogo.ShowAsync();
                LlenarGrid("Error Obtener Contenedor", cnta.msg!=""? cnta.msg:"No se encontró el contenedor", new SolidColorBrush(Colors.Red));
                //bcont = false;
                bvalida = true;
                return;
            }


            var vseal = new set_impo_fcl_sealsRequest();
            vseal.cntr = cnta.find_impo_cntr_pre_outResult;

            var seals = new seal_setter();
            seals.seal_cgsa = Sello.Trim().ToUpper();
            seals.seal_color = Color;
            seals.seal_ip_address = _ip;
            seals.seal_user = usuario;
            vseal.se = seals;

            var vseala = await s.set_impo_fcl_sealsAsync(vseal);

            if (!vseala.set_impo_fcl_sealsResult)
            {
                //var mensajeDialogo = new MessageDialog("Problema", vseala.msg);
                //await mensajeDialogo.ShowAsync();
                LlenarGrid("Valida Sello", vseala.msg, new SolidColorBrush(Colors.Red));
                //bsel = false;
                bvalida = true;
                return;
            }
            else
            {
                LlenarGrid("Error Valida Sello", vseala.msg, new SolidColorBrush(Colors.Green));
                bsel = true;
            }

            bContain = false;
            bSeal = false;
            bColor = false;
            bvalida = true;
        }

        public void Regresar()
        {
            if (!bvalida)
                return;
            IrVentanaLogin(true);
        }

        public void Limpiar()
        {
            if (!bvalida)
                return;
            LimpiarCampos();
        }

        private async Task LiberaHold(string unit)
        {
            val = true;
            //DesbloqueaUnit
            var dbuni = new DatosN4();
            dbuni.IdCompania = unit;
            dbuni.CedulaChofer = "CGSA_IMPO_SEAL";
            var lbh = await w.LiberarHoldAsync(dbuni);

            if (lbh.FueOk == true)
            {
                LlenarGrid("Hold", $@"Contenedor ""{unit}"" Liberado", new SolidColorBrush(Colors.Green));
            }
            else
            {
                mns = lbh.Mensaje;
                val = false;
                LlenarGrid("Problema Hold", mns, new SolidColorBrush(Colors.Red));
            }
        }

        private void LimpiarCampos()
        {
            Contenedor = "";
            Sello = "";
            Color = "";
            bContain = true;
            bSeal = true;
            bColor = true;
            Satelite = "";
            bSatel = false;
            tb = new ObservableCollection<DatoGridViewModel>();
        }

        private void GuardarRecursosAplicacion(OutPageViewModel resultado)
        {
            App.Current.Resources.Remove("OutData");
            App.Current.Resources.Add("OutData", resultado);
        }

        private void IrVentanaPrincipal(bool esLider)
        {
            _validaPage.Frame.Navigate(typeof(ValidatePage));
        }

        private void IrVentanaLogin(bool esLider)
        {
            _validaPage.Frame.Navigate(typeof(VentanaAutenticacion));
        }

        private void LlenarGrid(string dat, string inf, SolidColorBrush col)
        {
            var obj = new DatoGridViewModel();
            obj.Dato = dat.Trim().ToUpper();
            obj.Info = inf.Trim().ToUpper();
            obj.GColor2 = col;
            tb.Add(obj);
        }

        private static string ArmarXml(string user,string valid, string cnt, string seal, string peso)
        {
            StringBuilder xml = new StringBuilder();
            xml.AppendLine("<smdt><tipo>C</tipo>");
            xml.AppendLine("<usuario>" + user + "</usuario>");
            xml.AppendLine("<validar>" + valid + "</validar>");
            xml.AppendLine("<parametros>" + "<gkey>" + cnt + "</gkey>");

            if (seal != null && seal.Trim().Length>0)
                xml.AppendLine("<seals>" + seal + "</seals>");

            xml.AppendLine("<peso>" + peso + "</peso></parametros></smdt>");
            return xml.ToString();
        }
        #endregion
    }
}

using Mobile.ServiceTransact;
using Mobile.ServicioMobile;
using Mobile.ServiceTD;
using Mobile.ServicioAutenticacion;
using Mobile.Vistas;
using RECEPTIO.CapaPresentacion.UWP.MVVM;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.ServiceModel;
using Windows.UI.Popups;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI;

namespace Mobile.ViewModels
{
    internal class SAVPageViewModel : NotificationBase
    {
        #region Variables
        private readonly SAVPage _ventanaAutenticacion;
        private ObservableCollection<InfoViewModel> _tb;
        private ServiceClient s;
        private ServicioMobileClient w;
        private ServicioAnuncianteProblemaClient td;
        private pre_gate_detail_container_item dcnt;
        private pre_gate_detail_damage_item dam;
        private pre_gate_container_seal_item seal;
        private seal_setter sel;
        private container_item contain;
        private ServicioMobile.SEAL sealc;
        private ServicioMobile.CONTAINER cont;
        private ServicioMobile.DAMAGE damc;
        private pre_gate_item cabt;
        private ObservableCollection<pre_gate_details_item>  ldet;
        private ObservableCollection<DatosContenedor> cdc;
        private ObservableCollection<string> lpas;
        private pre_gate_details_item dett;
        private save_tablet_transactionResponse trMa;
        private get_door_passResponse pp;
        private aisv_item aisv;
        private booking_item book;
        private ObservableCollection<ListItem> _lsTip;
        private ObservableCollection<ListItem> _lsCom;
        private string Iso;
        private string Linea;
        private string Tamano;
        private string TipoCarga;
        private string Book;
        private string chofer;
        private string placa;
        private string empresa;
        private string usuario;
        private string carga1;
        private string carga2;
        private string turno;
        private string ap;
        private string tip;
        private string tipexp;
        private int tipTran;
        private string mns;
        private bool val;
        public bool bvalida = true;
        public bool binicia = false;
        public int num;
        public int farb1;
        public int farb2;
        private string _tranexp;
        private string _tranimp;
        private string LineaEdo;
        private bool flat = false;
        private bool habilita = false;
        private string tam = "";

        private string _cargaimp1;
        private string _cargaimp2;
        private string cargaimp3;
        private string cargaimp4;

        private string _cargaexp1;
        private string _sealcgsa1;
        private string _seal1;
        private string _seal2;
        private string _seal3;
        private string _seal4;
        private ListItem _danio1;
        private ListItem _danio2;
        private string _danio3;
        private string _danio4;
        private string _cargaexp2;
        private string _sealcgsa2;
        private string _seal5;
        private string _seal6;
        private string _seal7;
        private string _seal8;
        private ListItem _danio5;
        private ListItem _danio6;
        private string _danio7;
        private string _danio8;
        private string _sealcgsa3;
        private string _seal9;
        private string _seal10;
        private string _seal11;
        private string _seal12;
        private ListItem _danio9;
        private ListItem _danio10;
        private string _danio11;
        private string _danio12;
        private ListItem _danio13;
        private ListItem _danio14;
        private string _danio15;
        private string _danio16;
        private ListItem _danio17;
        private ListItem _danio18;
        private string _danio19;
        private string _danio20;
        private string _ip;
        private string _host;
        private string _zona;
        private string _usuario;
        private SolidColorBrush _gcolor;
        private ObservableCollection<combo_item> _lsTipo;
        private ObservableCollection<combo_item> _lsComp;
        private ObservableCollection<edo_control_item> _lsEDO;
        private RespuestaProceso tr;
        private ServicioMobile.ZONE ZonaObj;
        int id_device;

        #endregion

        #region Constructor
        internal SAVPageViewModel(SAVPage ventanaAutenticacion)
        {
            _ventanaAutenticacion = ventanaAutenticacion;
            //ObtenerIp();
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
            //w = new ServicioMobileClient();

            //var binding1 = new BasicHttpBinding
            //{
            //    MaxBufferPoolSize = 2147483647,
            //    MaxBufferSize = 2147483647,
            //    MaxReceivedMessageSize = 2147483647
            //};
            //binding1.ReaderQuotas.MaxArrayLength = 2147483647;
            //binding1.ReaderQuotas.MaxDepth = 2147483647;
            //binding1.ReaderQuotas.MaxStringContentLength = 2147483647;
            //Windows.ApplicationModel.Resources.Core.ResourceContext ctx = new Windows.ApplicationModel.Resources.Core.ResourceContext();
            //Windows.ApplicationModel.Resources.Core.ResourceMap rmap = Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap.GetSubtree("Recursos");
            var endpoint1 = rmap.GetValue("DireccionServicioMobile", ctx).ValueAsString;
            w = new ServicioMobileClient(binding, new EndpointAddress(endpoint1));


            Ip = ((VentanaAutenticacionViewModel)(App.Current.Resources["UserData"])).Ip.Trim();

            ObtenerDevice();
            ObtenerZona();
            td = new ServicioAnuncianteProblemaClient();
            chofer = ((ValidatePageViewModel)(App.Current.Resources["ValidaData"])).Chofer.Trim().ToUpper();
            placa = ((ValidatePageViewModel)(App.Current.Resources["ValidaData"])).Placa.Trim().ToUpper();
            empresa = ((ValidatePageViewModel)(App.Current.Resources["ValidaData"])).Empresa.Trim();
            usuario = ((VentanaAutenticacionViewModel)(App.Current.Resources["UserData"])).Usuario;
            Usuario = ((VentanaAutenticacionViewModel)(App.Current.Resources["UserData"])).Usuario;
            Host = ((VentanaAutenticacionViewModel)(App.Current.Resources["UserData"])).Host;
            Zona = ((VentanaAutenticacionViewModel)(App.Current.Resources["UserData"])).Zona;
            mns = "";
            lsTipo = new ObservableCollection<combo_item>();
            lsComp = new ObservableCollection<combo_item>();
            lsTip = new ObservableCollection<ListItem>();
            lsCom = new ObservableCollection<ListItem>();
            val = true;
            ListTipo();
            ListComponent();
        }
        
        #endregion

        #region Propiedades
        public string TransExp
        {
            get
            {
                return _tranexp;
            }
            set
            {
                if (_tranexp == value)
                    return;
                _tranexp = value;
                RaisePropertyChanged("TransExp");
            }
        }

        public string TransImp
        {
            get
            {
                return _tranimp;
            }
            set
            {
                if (_tranimp == value)
                    return;
                _tranimp = value;
                RaisePropertyChanged("TransImp");
            }
        }

        public string CargaExp1
        {
            get
            {
                return _cargaexp1;
            }
            set
            {
                if (_cargaexp1 == value)
                    return;
                _cargaexp1 = value;
                RaisePropertyChanged("CargaExp1");
            }
        }

        public string CargaExp2
        {
            get
            {
                return _cargaexp2;
            }
            set
            {
                if (_cargaexp2 == value)
                    return;
                _cargaexp2 = value;
                RaisePropertyChanged("CargaExp2");
            }
        }

        public string CargaImp1
        {
            get
            {
                return _cargaimp1;
            }
            set
            {
                if (_cargaimp1 == value)
                    return;
                _cargaimp1 = value;
                RaisePropertyChanged("CargaImp1");
            }
        }

        public string CargaImp2
        {
            get
            {
                return _cargaimp2;
            }
            set
            {
                if (_cargaimp2 == value)
                    return;
                _cargaimp2 = value;
                RaisePropertyChanged("CargaImp2");
            }
        }

        public string SealCgsa1
        {
            get
            {
                return _sealcgsa1;
            }
            set
            {
                if (_sealcgsa1 == value)
                    return;
                _sealcgsa1 = value;
                RaisePropertyChanged("SealCgsa1");
            }
        }

        public string SealCgsa2
        {
            get
            {
                return _sealcgsa2;
            }
            set
            {
                if (_sealcgsa2 == value)
                    return;
                _sealcgsa2 = value;
                RaisePropertyChanged("SealCgsa2");
            }
        }

        public string SealCgsa3
        {
            get
            {
                return _sealcgsa3;
            }
            set
            {
                if (_sealcgsa3 == value)
                    return;
                _sealcgsa3 = value;
                RaisePropertyChanged("SealCgsa3");
            }
        }

        public string Seal1
        {
            get
            {
                return _seal1;
            }
            set
            {
                if (_seal1 == value)
                    return;
                _seal1 = value;
                RaisePropertyChanged("Seal1");
            }
        }

        public string Seal2
        {
            get
            {
                return _seal2;
            }
            set
            {
                if (_seal2 == value)
                    return;
                _seal2 = value;
                RaisePropertyChanged("Seal2");
            }
        }

        public string Seal3
        {
            get
            {
                return _seal3;
            }
            set
            {
                if (_seal3 == value)
                    return;
                _seal3 = value;
                RaisePropertyChanged("Seal3");
            }
        }

        public string Seal4
        {
            get
            {
                return _seal4;
            }
            set
            {
                if (_seal4 == value)
                    return;
                _seal4 = value;
                RaisePropertyChanged("Seal4");
            }
        }

        public string Seal5
        {
            get
            {
                return _seal5;
            }
            set
            {
                if (_seal2 == value)
                    return;
                _seal5 = value;
                RaisePropertyChanged("Seal5");
            }
        }

        public string Seal6
        {
            get
            {
                return _seal6;
            }
            set
            {
                if (_seal6 == value)
                    return;
                _seal6 = value;
                RaisePropertyChanged("Seal6");
            }
        }

        public string Seal7
        {
            get
            {
                return _seal7;
            }
            set
            {
                if (_seal7 == value)
                    return;
                _seal7 = value;
                RaisePropertyChanged("Seal7");
            }
        }

        public string Seal8
        {
            get
            {
                return _seal8;
            }
            set
            {
                if (_seal8 == value)
                    return;
                _seal8 = value;
                RaisePropertyChanged("Seal8");
            }
        }

        public string Seal9
        {
            get
            {
                return _seal9;
            }
            set
            {
                if (_seal9 == value)
                    return;
                _seal9 = value;
                RaisePropertyChanged("Seal9");
            }
        }

        public string Seal10
        {
            get
            {
                return _seal10;
            }
            set
            {
                if (_seal10 == value)
                    return;
                _seal10 = value;
                RaisePropertyChanged("Seal10");
            }
        }

        public string Seal11
        {
            get
            {
                return _seal11;
            }
            set
            {
                if (_seal11 == value)
                    return;
                _seal11 = value;
                RaisePropertyChanged("Seal11");
            }
        }

        public string Seal12
        {
            get
            {
                return _seal12;
            }
            set
            {
                if (_seal12 == value)
                    return;
                _seal12 = value;
                RaisePropertyChanged("Seal12");
            }
        }

        public ListItem Danio1
        {
            get
            {
                return _danio1;
            }
            set
            {
                if (_danio1 == value)
                    return;
                _danio1 = value;
                RaisePropertyChanged("Danio1");
            }
        }

        public ListItem Danio2
        {
            get
            {
                return _danio2;
            }
            set
            {
                if (_danio2 == value)
                    return;
                _danio2 = value;
                RaisePropertyChanged("Danio2");
            }
        }

        public string Danio3
        {
            get
            {
                return _danio3;
            }
            set
            {
                if (_danio3 == value)
                    return;
                _danio3 = value;
                RaisePropertyChanged("Danio3");
            }
        }

        public string Danio4
        {
            get
            {
                return _danio4;
            }
            set
            {
                if (_danio4 == value)
                    return;
                _danio4 = value;
                RaisePropertyChanged("Danio4");
            }
        }

        public ListItem Danio5
        {
            get
            {
                return _danio5;
            }
            set
            {
                if (_danio5 == value)
                    return;
                _danio5 = value;
                RaisePropertyChanged("Danio5");
            }
        }

        public ListItem Danio6
        {
            get
            {
                return _danio6;
            }
            set
            {
                if (_danio6 == value)
                    return;
                _danio6 = value;
                RaisePropertyChanged("Danio6");
            }
        }

        public string Danio7
        {
            get
            {
                return _danio7;
            }
            set
            {
                if (_danio7 == value)
                    return;
                _danio7 = value;
                RaisePropertyChanged("Danio7");
            }
        }

        public string Danio8
        {
            get
            {
                return _danio8;
            }
            set
            {
                if (_danio8 == value)
                    return;
                _danio8 = value;
                RaisePropertyChanged("Danio8");
            }
        }

        public ListItem Danio9
        {
            get
            {
                return _danio9;
            }
            set
            {
                if (_danio9 == value)
                    return;
                _danio9 = value;
                RaisePropertyChanged("Danio9");
            }
        }

        public ListItem Danio10
        {
            get
            {
                return _danio10;
            }
            set
            {
                if (_danio10 == value)
                    return;
                _danio10 = value;
                RaisePropertyChanged("Danio10");
            }
        }

        public string Danio11
        {
            get
            {
                return _danio11;
            }
            set
            {
                if (_danio11 == value)
                    return;
                _danio11 = value;
                RaisePropertyChanged("Danio11");
            }
        }

        public string Danio12
        {
            get
            {
                return _danio12;
            }
            set
            {
                if (_danio12 == value)
                    return;
                _danio12 = value;
                RaisePropertyChanged("Danio12");
            }
        }

        public ListItem Danio13
        {
            get
            {
                return _danio13;
            }
            set
            {
                if (_danio13 == value)
                    return;
                _danio13 = value;
                RaisePropertyChanged("Danio13");
            }
        }

        public ListItem Danio14
        {
            get
            {
                return _danio14;
            }
            set
            {
                if (_danio14 == value)
                    return;
                _danio14 = value;
                RaisePropertyChanged("Danio14");
            }
        }

        public string Danio15
        {
            get
            {
                return _danio15;
            }
            set
            {
                if (_danio15 == value)
                    return;
                _danio15 = value;
                RaisePropertyChanged("Danio15");
            }
        }

        public string Danio16
        {
            get
            {
                return _danio16;
            }
            set
            {
                if (_danio16 == value)
                    return;
                _danio16 = value;
                RaisePropertyChanged("Danio16");
            }
        }

        public ListItem Danio17
        {
            get
            {
                return _danio17;
            }
            set
            {
                if (_danio17 == value)
                    return;
                _danio17 = value;
                RaisePropertyChanged("Danio17");
            }
        }

        public ListItem Danio18
        {
            get
            {
                return _danio18;
            }
            set
            {
                if (_danio18 == value)
                    return;
                _danio18 = value;
                RaisePropertyChanged("Danio18");
            }
        }

        public string Danio19
        {
            get
            {
                return _danio19;
            }
            set
            {
                if (_danio19 == value)
                    return;
                _danio19 = value;
                RaisePropertyChanged("Danio19");
            }
        }

        public string Danio20
        {
            get
            {
                return _danio20;
            }
            set
            {
                if (_danio20 == value)
                    return;
                _danio20 = value;
                RaisePropertyChanged("Danio20");
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

        public ObservableCollection<InfoViewModel> tb
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

        public ObservableCollection<combo_item> lsTipo
        {
            get
            {
                return _lsTipo;
            }
            set
            {
                SetProperty(ref _lsTipo, value);
            }
        }

        public ObservableCollection<combo_item> lsComp
        {
            get
            {
                return _lsComp;
            }
            set
            {
                SetProperty(ref _lsComp, value);
            }
        }

        public ObservableCollection<ListItem> lsTip
        {
            get
            {
                return _lsTip;
            }
            set
            {
                SetProperty(ref _lsTip, value);
            }
        }

        public ObservableCollection<ListItem> lsCom
        {
            get
            {
                return _lsCom;
            }
            set
            {
                SetProperty(ref _lsCom, value);
            }
        }

        #endregion

        #region Metodos
        private async void ObtenerIp()
        {
            var ip = await Dns.GetHostEntryAsync(Dns.GetHostName());
            Ip = ip.AddressList[1].ToString();

            //var dv = await w.ObtenerDeviceAsync(Ip);

            //if (dv != null)
            //    id_device = dv.DEVICE_ID;
            

            //var dz = await w.ObtenerZonaConTiposTransaccionesAsync(Ip);

            //if (dz != null)
            //    ZonaObj = dz;
        }

        private async void ObtenerDevice()
        {
            var dv = new ServicioMobile.DEVICE();
            dv = await w.ObtenerDeviceAsync(Ip);

            if (dv != null)
                id_device = dv.DEVICE_ID;
        }

        private async void ObtenerZona()
        {
            var dv = new ServicioMobile.ZONE();
            dv = await w.ObtenerZonaConTiposTransaccionesAsync(Ip);

            if (dv != null)
                ZonaObj = dv;
        }

        private bool PuedoIngresar()
        {
            return !string.IsNullOrWhiteSpace(CargaImp1) || !string.IsNullOrWhiteSpace(CargaExp1);
        }

        public async void Ingresar()
        {
            tb = new ObservableCollection<InfoViewModel>();
            bvalida = false;
            if (!PuedoIngresar())
            {
                //var mensajeDialogo = new MessageDialog("Error", "Verificar si ha ingresado datos.");
                //await mensajeDialogo.ShowAsync();
                LlenarGrid("Error", "Datos", "Verificar si ha ingresado datos", new SolidColorBrush(Colors.Red));
                bvalida = true;
                return;
            }

            if (!val && farb1==0 && farb2==0)
            {
                LimpiarGrid();
                val = true;
            }
            else
            {
                if (!val && (farb1 == 1 || farb2 == 1))
                {
                    var mensajeDialogo = new MessageDialog("Problema", "Existe un error no puede continuar (Verificar los mensajes presentados)");
                    await mensajeDialogo.ShowAsync();
                    bvalida = true;
                    return;
                }
            }

            if (CargaExp1 != null && CargaExp1.Trim().Length > 0)
            {
                try
                {
                    int cn = 0;
                    if (CargaExp1 != null && CargaExp1.Trim().Length > 0)
                    {
                        dcnt = new pre_gate_detail_container_item();
                        cdc = new ObservableCollection<DatosContenedor>();
                        await ValidarAisv(CargaExp1.Trim(), 1);

                        if (!val)
                        {
                            //var mensajeDialogo = new MessageDialog("Problema", mns);
                            //await mensajeDialogo.ShowAsync();
                            //LlenarGrid("Problema", "Aisv", mns, new SolidColorBrush(Colors.Red));
                            bvalida = true;
                            return;
                        }
                        else
                        {
                            cabt = new pre_gate_item();
                            ldet = new ObservableCollection<pre_gate_details_item>();
                            if (tipexp == "CNTR")
                                if (CargaImp1 != null && CargaImp1.Trim().Length > 0)
                                    tipTran = 10;
                                else
                                    tipTran = 7;

                            LlenaDetalleM(CargaExp1.Trim(), tipTran, empresa, "E");

                            //lpas = new ObservableCollection<string>();
                            //lpas.Add(ap);
                            ldet.Add(dett);
                        }
                    }
                    if (tipTran!=8 || tipTran!=9)
                    if (CargaExp2 != null && CargaExp2.Trim().Length > 0)
                    {
                        dcnt = new pre_gate_detail_container_item();
                        await ValidarAisv(CargaExp2.Trim(), 2);

                        if (!val)
                        {
                            //var mensajeDialogo = new MessageDialog("Problema", mns);
                            //await mensajeDialogo.ShowAsync();
                            //LlenarGrid("Problema", "Aisv", mns, new SolidColorBrush(Colors.Red));
                            bvalida = true;
                            return;
                        }
                        else
                        {
                            if (tipTran == 8)
                                cn = (int)aisv.packs;
                            LlenaDetalleM(CargaExp2.Trim(), tipTran, empresa, "E");
                            //lpas.Add(ap);
                            ldet.Add(dett);
                        }
                    }

                    LlenaCabecera();
                    await CreaTransacTablet();

                    if (!val)
                    {
                        //var mensajeDialogo = new MessageDialog("Problema", mns);
                        //await mensajeDialogo.ShowAsync();

                        LlenarGrid("Problema", "Transacción Tablet", mns, new SolidColorBrush(Colors.Red));
                        //await ActualizaTransacTablet();
                        var binding = new BasicHttpBinding();
                        Windows.ApplicationModel.Resources.Core.ResourceContext ctx = new Windows.ApplicationModel.Resources.Core.ResourceContext();
                        Windows.ApplicationModel.Resources.Core.ResourceMap rmap = Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap.GetSubtree("Recursos");
                        var endpoint = rmap.GetValue("ServicioTD", ctx).ValueAsString;
                        var servicio = new ServicioAnuncianteProblemaClient(binding, new EndpointAddress(endpoint));
                        await servicio.AnunciarProblemaGenericoMobileAsync("[Transacción TABLET] " + mns, ZonaObj.ZONE_ID);

                        bvalida = true;
                        return;
                    }

                    //Desbloquear Contenedores
                    if (carga1 != null && carga1.Length > 0)
                    {
                        await LiberaHold(carga1, "CGSA_EXPO_SEAL");
                        if (!val)
                        {
                            //var mensajeDialogo = new MessageDialog("Problema Hold", mns);
                            //await mensajeDialogo.ShowAsync();
                            //LlenarGrid("Problema", "Hold", mns, new SolidColorBrush(Colors.Red));
                            bvalida = true;
                            return;
                        }

                    }

                    if (carga2 != null && carga2.Length > 0)
                    {
                        await LiberaHold(carga2, "CGSA_EXPO_SEAL");
                        if (val == false)
                        {
                            //var mensajeDialogo = new MessageDialog("Problema Hold", mns);
                            //await mensajeDialogo.ShowAsync();
                            //LlenarGrid("Problema", "Hold", mns, new SolidColorBrush(Colors.Red));
                            bvalida = true;
                            return;
                        }

                    }

                    await CreaTransacN4();

                    if (!val)
                    {
                        if (mns.Contains("TROUBLE"))
                        {
                            if (mns.Contains("message-text="))
                            {
                                var a = mns.IndexOf("message-text=");
                                var b = mns.IndexOf("message-severity=");
                                LlenarGrid(TransExp, "Transacción N4", mns.Substring(a + 14, b - a - 17), new SolidColorBrush(Colors.Red));
                            }
                            else
                                LlenarGrid(TransExp, "Transacción N4", mns, new SolidColorBrush(Colors.Red));
                            //await ActualizaTransacTablet();
                        }
                        else
                        {
                            await ActualizaTransacTablet();
                            LlenarGrid(TransExp, "Transacción N4", mns, new SolidColorBrush(Colors.Red));
                        }
                    }
                    else
                    {
                        LlenarGrid(TransExp, "Transacción N4", mns, new SolidColorBrush(Colors.Green));
                        var mensajeDialogo = new MessageDialog("Transacción N4", mns);
                        await mensajeDialogo.ShowAsync();
                    }
                }
                catch (Exception e)
                {
                    LlenarGrid("Error", "Ingresar Expo", e.Message, new SolidColorBrush(Colors.Red));
                    //await ActualizaTransacTablet();
                }
            }
            else
            {
                if (CargaImp1 != null && CargaImp1.Trim().Length > 0)
                {
                    try
                    {
                        //LineaEdo = cargaimp3.Substring(4, 3);
                        //tam = cargaimp3.Substring(8, 1);

                        if (CargaImp1 != null && CargaImp1.Trim().Length > 0)
                        {
                            //dcnt = new pre_gate_detail_container_item();
                            await ValidarEdo(CargaImp1.Trim(), 1);

                            if (!val)
                            {
                                //var mensajeDialogo = new MessageDialog("Problema", mns);
                                //await mensajeDialogo.ShowAsync();
                                //LlenarGrid("Problema", "Pase", mns, new SolidColorBrush(Colors.Red));
                                bvalida = true;
                                return;
                            }
                            else
                            {
                                cabt = new pre_gate_item();
                                ldet = new ObservableCollection<pre_gate_details_item>();
                                tipTran = 17;

                                LlenaDetalleM(cargaimp3.Trim(), tipTran, empresa, "I");
                                lpas = new ObservableCollection<string>();
                                lpas.Add(CargaImp1);
                                ldet.Add(dett);
                            }
                        }

                        if (CargaImp2 != null && CargaImp2.Trim().Length > 0)
                        {
                            //if (LineaEdo != cargaimp4.Substring(4, 3))
                            //{
                            //    val = false;
                            //    mns = "No puede solicitar EDOs de líneas diferentes";
                            //    LlenarGrid("Problema", "EDO", mns, new SolidColorBrush(Colors.Red));
                            //    bvalida = true;
                            //    return;
                            //}

                            //if ((tam != "2" || cargaimp4.Substring(8, 1)!="2"))
                            //{
                            //    val = false;
                            //    mns = "No puede solicitar más de 1 unidad de 40";
                            //    LlenarGrid("Problema", "ERROR", mns, new SolidColorBrush(Colors.Red));
                            //    bvalida = true;
                            //    return;
                            //}

                            await ValidarEdo(CargaImp2.Trim(), 2);

                            if (!val)
                            {
                                //var mensajeDialogo = new MessageDialog("Problema", mns);
                                //await mensajeDialogo.ShowAsync();
                                //LlenarGrid("Problema", "Pase", mns, new SolidColorBrush(Colors.Red));
                                bvalida = true;
                                return;
                            }
                            else
                            {
                                LlenaDetalleM(cargaimp4.Trim(), tipTran, empresa, "I");
                                lpas.Add(ap);
                                ldet.Add(dett);
                            }
                        }

                        LlenaCabecera();
                        await CreaTransacTablet();

                        if (!val)
                        {
                            //var mensajeDialogo = new MessageDialog("Problema", mns);
                            //await mensajeDialogo.ShowAsync();
                            LlenarGrid("Problema", "Transacción Tablet", mns, new SolidColorBrush(Colors.Red));
                            //await ActualizaTransacTablet();

                            var binding = new BasicHttpBinding();
                            Windows.ApplicationModel.Resources.Core.ResourceContext ctx = new Windows.ApplicationModel.Resources.Core.ResourceContext();
                            Windows.ApplicationModel.Resources.Core.ResourceMap rmap = Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap.GetSubtree("Recursos");
                            var endpoint = rmap.GetValue("ServicioTD", ctx).ValueAsString;
                            var servicio = new ServicioAnuncianteProblemaClient(binding, new EndpointAddress(endpoint));
                            await servicio.AnunciarProblemaGenericoMobileAsync("[Transacción TABLET] " + mns, ZonaObj.ZONE_ID);

                            bvalida = true;
                            return;
                        }

                        await CreaTransacN4();

                        if (!val)
                        {

                            if (mns.Contains("TROUBLE"))
                            {
                                if (mns.Contains("message-text="))
                                {
                                    var a = mns.IndexOf("message-text=");
                                    var b = mns.IndexOf("message-severity=");
                                    LlenarGrid(TransImp, "Transacción N4", mns.Substring(a + 14, b - a - 17), new SolidColorBrush(Colors.Red));
                                }
                                else
                                    LlenarGrid(TransImp, "Transacción N4", mns, new SolidColorBrush(Colors.Red));
                                //await ActualizaTransacTablet();
                            }
                            else
                            {
                                await ActualizaTransacTablet();
                                LlenarGrid(TransImp, "Transacción N4", mns, new SolidColorBrush(Colors.Red));
                            }
                        }
                        else
                        {
                            LlenarGrid(TransImp, "Transacción N4", mns, new SolidColorBrush(Colors.Green));
                            var mensajeDialogo = new MessageDialog("Transacción N4", mns);
                            await mensajeDialogo.ShowAsync();
                        }
                    }
                    catch (Exception e)
                    {
                        LlenarGrid("Error", "Ingresar Impo", e.Message, new SolidColorBrush(Colors.Red));
                        //await ActualizaTransacTablet();
                    }
                }
            }

            bvalida = true;
        }

        public void Limpiar()
        {
            if (!bvalida)
                return;
            LimpiarCampos();
        }

        public void VentanaInicio()
        {
            if (!bvalida)
                return;
            IrVentanaPrincipal(true);
        }

        public void Regresar()
        {
            if (!bvalida)
                return;
            IrVentanaLogin(true);
        }

        public void Sellos()
        {
            binicia = false;
            //IrVentanaSello(true);
            CargarSellos();
            binicia = true;
        }

        public async void SellosUp()
        {
            /*binicia = false;
            val = true;
            TransExp = "Entrega Expo";
            
            DescargarSellos();
            binicia = true;*/
            tb = new ObservableCollection<InfoViewModel>();
            await SelloFarb();
        }

        public void LimpiaSello()
        {
            binicia = false;
            //IrVentanaSello(true);
            LimpiarSellos();
            binicia = true;
        }

        public void Danios()
        {
            binicia = false;
            //IrVentanaDanio(true);
            CargarDanios();
            binicia = true;
        }

        public void DaniosUp()
        {
            binicia = false;
            //IrVentanaDanio(true);
            DescargarDanios();
            binicia = true;
        }

        public void LimpiaDanio()
        {
            binicia = false;
            //IrVentanaDanio(true);
            LimpiarDanios();
            binicia = true;
        }

        private async Task ValidarAisv(string pase, int num)
        {
            val = true;
            TransExp = "Entrega Expo";
            contain = null;
            
            var fce = new find_expo_cntr_mtyRequest(pase);
            var fcea = await s.find_expo_cntr_mtyAsync(fce);
            if (fcea.find_expo_cntr_mtyResult != null)
                contain = fcea.find_expo_cntr_mtyResult;
            else
            {
                if (fcea.msg != null)
                    mns = fcea.msg;
                else
                    mns = "No existe Contenedor";

                val = false;
                LlenarGrid(TransExp, "Cont", mns, new SolidColorBrush(Colors.Red));
            }

            if (contain != null)
            {
                tipexp = "CNTR";
                turno = contain.id;

                TransExp = "Entrega Expo " + tipexp;

                if (turno != null && turno.Trim().Length > 0)
                    LlenarGrid(TransExp, "Contenedor", turno, new SolidColorBrush(Colors.Green));

                var cit = new check_impedimentsRequest();

                if (tipexp == "CNTR")
                {

                    int vac=0;
                    if (contain != null)
                    {
                        //if (contain.freight_kind == "MTY" || contain.freight_kind == "LCL")
                        //{
                            LlenarGrid(TransExp, "Booking", contain.bl_nbr, new SolidColorBrush(Colors.Green));
                            LlenarGrid(TransExp, "Iso", contain.type_cntr, new SolidColorBrush(Colors.Green));
                            var mensajeDialogo = new MessageDialog("Notificación", string.Format("El Contenedor {3} presenta el Booking {0} Iso {1} Height {2} confirmar si desea continuar.", contain.bl_nbr, contain.type_cntr, contain.tare_weight, contain.id));
                            mensajeDialogo.Commands.Add(new Windows.UI.Popups.UICommand("Continuar") { Id = 0 });
                            mensajeDialogo.Commands.Add(new Windows.UI.Popups.UICommand("Cancelar") { Id = 1 });
                            mensajeDialogo.DefaultCommandIndex = 0;
                            mensajeDialogo.CancelCommandIndex = 1;
                            var men = await mensajeDialogo.ShowAsync();
                            vac = (int)men.Id;
                        //}

                        if(vac==0)
                        { 
                            ObservableCollection<ServicioMobile.SEAL> csealC;
                            ObservableCollection<ServicioMobile.DAMAGE> cdamC;

                            if (num == 1)
                            {
                                carga1 = contain.id;
                                var dc = new DatosContenedor();
                            
                                cont = new ServicioMobile.CONTAINER();
                            
                                //LlenarGrid(TransExp, "Contenedor", carga1, new SolidColorBrush(Colors.Green));
                                // llenas detalle de contenedor
                                dcnt.number = contain.id;
                                Iso = contain.type_cntr;
                                Linea = contain.line;
                                Tamano = contain.size;
                                TipoCarga = contain.freight_kind;
                                Book = contain.bl_nbr;

                                cont.NUMBER = contain.id;
                                dc.Aisv = Book;
                                dc.Iso = Iso;
                                dc.Linea = Linea;
                                dc.Tamano = Tamano;
                                dc.TipoCarga = TipoCarga;

                                if (SealCgsa1 != null && SealCgsa1.Trim().Length > 0)
                                {
                                    var cseal = new ObservableCollection<pre_gate_container_seal_item>();
                                    csealC = new ObservableCollection<ServicioMobile.SEAL>();

                                    sel = new seal_setter();
                                    sel.seal_cgsa = SealCgsa1;
                                    sel.seal_user = Usuario;
                                    sel.seal_ip_address = Ip;

                                
                                    if (Seal1 != null && Seal1.Trim().Length > 0)
                                    {
                                        LlenaSello("seal1", Seal1);
                                        LlenaSelloC("SEAL-1", Seal1);
                                        cseal.Add(seal);
                                        csealC.Add(sealc);
                                        if (TipoCarga != "MTY" && TipoCarga != "LCL")
                                            aisv.seal1 = Seal1;
                                    }
                                    else
                                    {
                                        val = false;
                                        LlenarGrid(TransExp, "Sello", "Primera carga sin sello 1", new SolidColorBrush(Colors.Red));
                                        return;
                                    }

                                    if (Seal2 != null && Seal2.Trim().Length > 0)
                                    {
                                        LlenaSello("seal2", Seal2);
                                        LlenaSelloC("SEAL-2", Seal2);
                                        cseal.Add(seal);
                                        csealC.Add(sealc);
                                        if (TipoCarga != "MTY" && TipoCarga != "LCL")
                                            aisv.seal2 = Seal2;
                                    }

                                    if (Seal3 != null && Seal3.Trim().Length > 0)
                                    {
                                        LlenaSello("seal3", Seal3);
                                        LlenaSelloC("SEAL-3", Seal3);
                                        cseal.Add(seal);
                                        csealC.Add(sealc);
                                        if (TipoCarga != "MTY" && TipoCarga != "LCL")
                                            aisv.seal3 = Seal3;
                                    }

                                    if (Seal4 != null && Seal4.Trim().Length > 0)
                                    {
                                        LlenaSello("seal4", Seal4);
                                        LlenaSelloC("SEAL-4", Seal4);
                                        cseal.Add(seal);
                                        csealC.Add(sealc);
                                        if (TipoCarga != "MTY" && TipoCarga != "LCL")
                                            aisv.seal4 = Seal4;
                                    }
                                    LlenaSello("cgsa_seal", SealCgsa1);
                                    cseal.Add(seal);
                                    dcnt.seals = cseal;
                                    cont.SEALS = csealC;
                                }
                                else
                                {
                                    val = false;
                                    LlenarGrid(TransExp, "Sello", "Primera carga sin sello CGSA", new SolidColorBrush(Colors.Red));
                                    return;
                                }

                                var cdam = new ObservableCollection<pre_gate_detail_damage_item>(); 
                                cdamC = new ObservableCollection<ServicioMobile.DAMAGE>();

                                if (Danio1 != null)
                                {
                                    LlenaDanio(Danio1.Dato, Danio2.Dato, Danio3, Danio4);
                                    LlenaDanioC(Danio1.Dato, Danio2.Dato, Danio3, Danio4);
                                    cdam.Add(dam);
                                    cdamC.Add(damc);
                                }

                                if (Danio13 != null)
                                {
                                    LlenaDanio(Danio13.Dato, Danio14.Dato, Danio15, Danio16);
                                    LlenaDanioC(Danio13.Dato, Danio14.Dato, Danio15, Danio16);
                                    cdam.Add(dam);
                                    cdamC.Add(damc);
                                }

                                if (cdam.Count>0)
                                    dcnt.damages = cdam;

                                if (cdamC.Count > 0)
                                    cont.DAMAGES = cdamC;

                                dc.Contenedor = cont;
                                cdc.Add(dc);
                            }

                            if (num == 2)
                            {
                                carga2 = contain.id;
                                var dc = new DatosContenedor();
                                cont = new ServicioMobile.CONTAINER();
                                //LlenarGrid(TransImp, "Contenedor", carga2, new SolidColorBrush(Colors.Green));
                                // llenas detalle de contenedor
                                dcnt.number = contain.id;
                                Iso = contain.type_cntr;
                                Linea = contain.line;
                                Tamano = contain.size;
                                TipoCarga = contain.freight_kind;
                                Book = contain.bl_nbr;

                                cont.NUMBER = contain.id;
                                dc.Aisv = Book;
                                //dc.Contenedor = cont;
                                dc.Iso = Iso;
                                dc.Linea = Linea;
                                dc.Tamano = Tamano;
                                dc.TipoCarga = TipoCarga;

                                //cdc.Add(dc);

                                if (SealCgsa2 != null && SealCgsa2.Trim().Length > 0)
                                {
                                    var cseal = new ObservableCollection<pre_gate_container_seal_item>();
                                    csealC = new ObservableCollection<ServicioMobile.SEAL>();

                                    sel = new seal_setter();
                                    sel.seal_cgsa = SealCgsa2;
                                    sel.seal_user = Usuario;
                                    sel.seal_ip_address = Ip;

                                
                                    if (Seal5 != null && Seal5.Trim().Length > 0)
                                    {
                                        LlenaSello("seal1", Seal5);
                                        LlenaSelloC("SEAL-1", Seal5);
                                        cseal.Add(seal);
                                        csealC.Add(sealc);
                                        if (TipoCarga != "MTY" && TipoCarga != "LCL")
                                            aisv.seal1 = Seal5;
                                    }
                                    else
                                    {
                                        val = false;
                                        LlenarGrid(TransExp, "Sello", "Segunda carga sin sello 1", new SolidColorBrush(Colors.Red));
                                        return;
                                    }

                                    if (Seal6 != null && Seal6.Trim().Length > 0)
                                    {
                                        LlenaSello("seal2", Seal6);
                                        LlenaSelloC("SEAL-2", Seal6);
                                        cseal.Add(seal);
                                        csealC.Add(sealc);
                                    }

                                    if (Seal7 != null && Seal7.Trim().Length > 0)
                                    {
                                        LlenaSello("seal3", Seal7);
                                        LlenaSelloC("SEAL-3", Seal7);
                                        cseal.Add(seal);
                                        csealC.Add(sealc);
                                    }

                                    if (Seal8 != null && Seal8.Trim().Length > 0)
                                    {
                                        LlenaSello("seal4", Seal8);
                                        LlenaSelloC("SEAL-4", Seal8);
                                        cseal.Add(seal);
                                        csealC.Add(sealc);
                                    }

                                    LlenaSello("cgsa_seal", SealCgsa2);
                                    cseal.Add(seal);
                                    dcnt.seals = cseal;
                                    cont.SEALS = csealC;
                                }
                                else
                                {
                                    val = false;
                                    LlenarGrid(TransExp, "Sello", "Segunda carga sin sello CGSA", new SolidColorBrush(Colors.Red));
                                    return;
                                }

                                var cdam = new ObservableCollection<pre_gate_detail_damage_item>();
                                cdamC = new ObservableCollection<ServicioMobile.DAMAGE>();
                                if (Danio5 != null)
                                {
                                    LlenaDanio(Danio5.Dato, Danio6.Dato, Danio7, Danio8);
                                    LlenaDanioC(Danio5.Dato, Danio6.Dato, Danio7, Danio8);
                                    cdam.Add(dam);
                                    cdamC.Add(damc);
                                }

                                if (Danio17 != null)
                                {
                                    LlenaDanio(Danio17.Dato, Danio18.Dato, Danio19, Danio20);
                                    LlenaDanioC(Danio17.Dato, Danio18.Dato, Danio19, Danio20);
                                    cdam.Add(dam);
                                    cdamC.Add(damc);
                                }

                                if (cdam.Count>0)
                                    dcnt.damages = cdam;

                                if (cdamC.Count > 0)
                                    cont.DAMAGES = cdamC;

                                dc.Contenedor = cont;
                                cdc.Add(dc);
                                //dcnt.container_id = int.Parse(contain.gkey.ToString().Substring(0, contain.gkey.ToString().Length - 3));
                            }

                            cit.cntr = contain;
                            cit.stage = "PRE_IN";

                            var cia = await s.check_impedimentsAsync(cit);

                            if (!cia.check_impedimentsResult)
                            {
                                if (val)
                                    mns = cia.msg;
                                val = false;
                                LlenarGrid(TransExp, "Impedimento", cia.msg, new SolidColorBrush(Colors.Red));
                            }

                            if(val)
                            {
                                var afs = new set_aisv_mty_sealsRequest();
                            
                                afs.cntr = contain.id;
                                afs.se = sel;

                                if (num == 1)
                                {
                                    afs.seal1 = Seal1;
                                    if (Seal2 != null && Seal2.Trim().Length > 0)
                                        afs.label1 = Seal2;
                                    else
                                        afs.label1 = "SS";
                                    if (Seal3 != null && Seal3.Trim().Length > 0)
                                        afs.label2 = Seal3;
                                    else
                                        afs.label2 = "SS";
                                }
                                else if (num == 2)
                                {
                                    afs.seal1 = Seal5;
                                    if (Seal6 != null && Seal6.Trim().Length > 0)
                                        afs.label1 = Seal2;
                                    else
                                        afs.label1 = "SS";
                                    if (Seal7 != null && Seal7.Trim().Length > 0)
                                        afs.label2 = Seal7;
                                    else
                                        afs.label2 = "SS";
                                }
                                else
                                {
                                    afs.seal1 = "SS";
                                    afs.label1 = "SS";
                                    afs.label2 = "SS";
                                }

                                var afsa = await s.set_aisv_mty_sealsAsync(afs);

                                if (!afsa.set_aisv_mty_sealsResult)
                                {
                                    mns = afsa.msg;
                                    LlenarGrid(TransExp, "Sellos", mns, new SolidColorBrush(Colors.Red));
                                    val = false;
                                }
                            }
                        }
                        else
                        {
                            mns = "Se detuvo Proceso";
                            LlenarGrid(TransExp, "Booking", mns, new SolidColorBrush(Colors.Red));
                            val = false;
                        }
                    }
                    else
                    {
                        mns = "No se obtuvo información del CONTENEDOR";
                        LlenarGrid(TransExp, "Contenedor", mns, new SolidColorBrush(Colors.Red));
                        val = false;
                    }
                }
            }
        }

        private async Task ValidarEdo(string pase, int num)
        {
            val = true;
            book = null;
            ap = pase;
            TransImp = "Retiro Impo SAV";
            contain = null;
            tip = "CNTR";
            TransImp += " " + tip;

            book = await s.find_bookingAsync(pase);

            if (book==null)
            {

                //valida chofer

                val = false;
                LlenarGrid(TransImp, "Error BOOKING", "BOOKING NO AUTORIZADO", new SolidColorBrush(Colors.Red));
                
            }
            else
            {
                if (num==1)
                    cargaimp3 = pase + book.line + book.iso + book.pod1 + book.pod + book.status;
                else
                    cargaimp4 = pase + book.line + book.iso + book.pod1 + book.pod + book.status;

                LlenarGrid(TransImp, "BOOKING", "Validación OK", new SolidColorBrush(Colors.Green));
                val = true;
            }

            ////valida transact N4
            //var cpt = new check_pass_transactionRequest();
            //cpt.pass = pp.get_door_passResult;
            //var cta = await s.check_pass_transactionAsync(cpt);

            //if (!cta.check_pass_transactionResult)
            //{
            //    //var mensajeDialogo = new MessageDialog("Problema", cta.msg);
            //    //await mensajeDialogo.ShowAsync();
            //    if (val == true)
            //        mns = cta.msg;
            //    val = false;
            //    LlenarGrid(TransImp, "Transacción", cta.msg, new SolidColorBrush(Colors.Red));

            //    var binding = new BasicHttpBinding();
            //    Windows.ApplicationModel.Resources.Core.ResourceContext ctx = new Windows.ApplicationModel.Resources.Core.ResourceContext();
            //    Windows.ApplicationModel.Resources.Core.ResourceMap rmap = Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap.GetSubtree("Recursos");
            //    var endpoint = rmap.GetValue("ServicioTD", ctx).ValueAsString;
            //    var servicio = new ServicioAnuncianteProblemaClient(binding, new EndpointAddress(endpoint));
            //    await servicio.AnunciarProblemaGenericoMobileAsync("[Transacción N4] " + cta.msg, ZonaObj.ZONE_ID);
            //}
        }

        private async Task SelloFarb()
        {
            binicia = false;
            //IrVentanaSello(true);
            val = true;
            TransExp = "Entrega Expo";
            contain = null;

            var fce = new sav_validationRequest();
            fce.fecha = DateTime.Now;
            if (num == 1)
            {
                farb1 = 0;
                fce.cntr_id = CargaExp1;
            }
            else
            {
                farb2 = 0;
                fce.cntr_id = CargaExp2;
            }

            sav_validationResponse fcea = await s.sav_validationAsync(fce);
            if (fcea.sav_validationResult == -1)
                DescargarSellos();
            else
            {
                if (fcea.sav_validationResult == 0)
                {
                    val = false;
                    mns = fcea.msg;
                    LlenarGrid(TransExp, "Farbem", mns, new SolidColorBrush(Colors.Red));

                    if (num == 1)
                        farb1 = 1;
                    else
                        farb2 = 1;
                }
                else
                {
                    CargarSellosF();
                    mns = "Contenedor " + fce.cntr_id + "pertenece al Servicio SAV";
                    LlenarGrid(TransExp, "Farbem", mns, new SolidColorBrush(Colors.Green));
                }
                var mensajeDialogo = new MessageDialog("Notificación", string.Format("Contenedor {0} pertenece al Servicio SAV.", fce.cntr_id));
                var men = await mensajeDialogo.ShowAsync();
            }
            
            binicia = true;
        }

        private void LlenaDetalle(string carga, int tip)
        {
            dett = new pre_gate_details_item();
            //llenas detalle de transact
            dett.status = "N";
            dett.transaction_number = carga;
            dett.transaction_type_id = tip;

            if (tip == 1 || tip == 3 || tip == 6 || tip == 7)
                dett.container = dcnt;
        }

        private void LlenaDetalleE(string carga, int tip, string dae, int cant)
        {
            dett = new pre_gate_details_item();
            //llenas detalle de transact
            dett.status = "N";
            dett.transaction_number = carga;
            dett.transaction_type_id = tip;
            if(dae!=null && dae.Trim().Length>0)
                dett.document_id = dae.Trim();
            if (cant > 0)
                dett.reference_id = cant.ToString();
            if (tip == 1 || tip == 3 || tip == 6 || tip == 7)
                dett.container = dcnt;
        }

        private void LlenaDetalleM(string carga, int tip, string dae, string refe)
        {
            dett = new pre_gate_details_item();
            //llenas detalle de transact
            dett.status = "N";
            dett.transaction_number = carga;
            dett.transaction_type_id = tip;
            if (dae != null && dae.Trim().Length > 0)
                dett.document_id = dae.Trim();
            //if (refe != null && refe.Trim().Length > 0)
            //    dett.reference_id = refe;
            if ((tip == 7 || tip == 10 || tip == 11) && refe=="E")
                dett.container = dcnt;
        }

        private void LlenaSello(string name, string sello)
        {
            seal = new pre_gate_container_seal_item();
            seal.caption = name;
            seal.value = sello;
        }

        private void LlenaSelloC(string name, string sello)
        {
            sealc = new ServicioMobile.SEAL();
            sealc.CAPTION = name;
            sealc.VALUE = sello;
        }

        private void LlenaDanio(string tip, string comp, string sev, string not)
        {
            dam = new pre_gate_detail_damage_item();
            dam.damage_type = tip;
            dam.component = comp;
            dam.severity = sev;
            dam.notes = not;
            dam.location = "RECEPT";
            dam.quantity = 1;
        }

        private void LlenaDanioC(string tip, string comp, string sev, string not)
        {
            damc = new ServicioMobile.DAMAGE();
            damc.DAMAGE_TYPE = tip;
            damc.COMPONENT = comp;
            damc.SEVERITY = sev;
            damc.NOTES = not;
            damc.LOCATION = "RECEPT";
            damc.QUANTITY = 1;
        }

        private void LlenaCabecera()
        {
            //llena cabecera transact
            cabt.device_id = id_device;
            cabt.driver_id = chofer;
            cabt.truck_licence = placa;
            cabt.user = usuario;
            cabt.pre_gate_details = ldet;
        }

        private async Task CreaTransacTablet()
        {
            val = true;
            // crear transacción interna
            var trM = new save_tablet_transactionRequest();
            trM.transaction = cabt;
            
            trMa = await s.save_tablet_transactionAsync(trM);

            if (!trMa.save_tablet_transactionResult)
            {
                mns = trMa.OnError + "- Tablet " + Host + " User " + Usuario + " Placa " + placa + " Id Chofer " + chofer;
                val = false;
                //LlenarGrid(TransExp, "Transaccion Tablet", mns, new SolidColorBrush(Colors.Red));
            }
        }

        private async Task ActualizaTransacTablet()
        {
            val = true;
            // crear transacción interna
            var trU = new update_pregate_transactionRequest();
            trU.driver = chofer;
            trU.truck = placa;

            var trUa = await s.update_pregate_transactionAsync(trU);
            if (!trUa.update_pregate_transactionResult)
            {
                mns = trUa.OnError;
                val = false;
                LlenarGrid(TransImp, "Transaccion Tablet Act", mns, new SolidColorBrush(Colors.Red));
            }
        }

        private async Task LiberaHold(string unit, string hold)
        {
            val = true;
            //DesbloqueaUnit
            var dbuni = new DatosN4();
            dbuni.IdCompania = unit;
            dbuni.CedulaChofer = hold;//"CGSA_APPT_VBS"
            var lbh = await w.LiberarHoldAsync(dbuni);

            if (lbh.FueOk == true)
            {
                if(TransImp!=null && TransImp.Trim().Length>0)
                    LlenarGrid(TransImp, "Hold", $@"Contenedor ""{unit}"" Liberado", new SolidColorBrush(Colors.Green));
                else
                    LlenarGrid(TransExp, "Hold", $@"Contenedor ""{unit}"" Liberado", new SolidColorBrush(Colors.Green));
            }
            else
            {
                mns = lbh.Mensaje;
                val = false;
                if (TransImp != null && TransImp.Trim().Length > 0)
                    LlenarGrid(TransImp, "Problema Hold", mns, new SolidColorBrush(Colors.Red));
                else
                    LlenarGrid(TransExp, "Problema Hold", mns, new SolidColorBrush(Colors.Red));
            }
        }

        private async Task CambiaHold(string appk)
        {
            val = true;
            //DesbloqueaUnit
            var dbuni = new DatosN4();
            dbuni.IdCompania = appk;
            var lbh = await w.CambiarHoldAsync(dbuni);

            if (lbh.FueOk == true)
            {
                LlenarGrid(TransImp, "Cambio Appoitment", $@"Appoitment ""{appk}"" Cambiado a Receptio", new SolidColorBrush(Colors.Green));
            }
            else
            {
                mns = lbh.Mensaje;
                val = false;
                LlenarGrid(TransImp, "Problema Cambio Appoitment", mns, new SolidColorBrush(Colors.Red));
            }
        }

        private async Task CreaTransacN4()
        {
            val = true;
            bool r = false;
            TransImp = "Despacho SAV";

            if (TransExp != null && TransExp.Trim().Length > 0)
            {
                if (tipexp == "CNTR")
                {
                    var de = new DatosReceiveExport();
                    de.CedulaChofer = chofer;
                    de.PlacaVehiculo = placa;
                    de.IdCompania = empresa;// aisv.truck_company_id;

                    if (trMa != null)
                        de.IdPreGate = trMa.OnError;
                    else
                        de.IdPreGate = "1";

                    if (cdc != null && cdc.Count > 0)
                    {
                        de.DataContenedores = cdc;

                        tr = new RespuestaProceso();
                        tr = await w.EjecutarProcesosReceiveExportAsync(de);
                        r = tr.FueOk;
                        mns = tr.Mensaje;
                    }
                    else
                    {
                        r = false;
                        mns = "ERROR EN CARGA DE UNIDADES PARA N4";
                    }
                }
            }
            else
                if (TransImp != null && TransImp.Trim().Length > 0)
                {
                tip = "CNTR";
                    if (tip == "CNTR")
                    {
                        var de = new DatosN4();
                        de.CedulaChofer = chofer;
                        de.PlacaVehiculo = placa;
                        de.IdCompania = empresa;// aisv.truck_company_id;

                        if (trMa != null)
                            de.IdPreGate = trMa.OnError;
                        else
                            de.IdPreGate = "1";

                        de.NumerosTransacciones = lpas;

                        tr = new RespuestaProceso();
                        tr = await w.EjecutarProcesosDeliveryImportMTYBookingAsync(de);
                        r = tr.FueOk;
                        mns = tr.Mensaje;

                    }
                }

            if (r)
            {
                //var mensajeDialogo = new MessageDialog("Transacción N4", "Proceso Satisfactorio.");
                //await mensajeDialogo.ShowAsync();
                mns = "Proceso Satisfactorio";
            }
            else
            {
                var binding = new BasicHttpBinding();
                Windows.ApplicationModel.Resources.Core.ResourceContext ctx = new Windows.ApplicationModel.Resources.Core.ResourceContext();
                Windows.ApplicationModel.Resources.Core.ResourceMap rmap = Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap.GetSubtree("Recursos");
                var endpoint = rmap.GetValue("ServicioTD", ctx).ValueAsString;
                var servicio = new ServicioAnuncianteProblemaClient(binding, new EndpointAddress(endpoint));
                await servicio.AnunciarProblemaMobileAsync(tr.IdTosProcess, ZonaObj.ZONE_ID);
                val = false;
                //LlenarGrid(TransImp, "Transaccion N4", mns);
            }
        }

        private async void ListTipo()
        {
            lsTipo = await s.get_damage_listAsync();
            ListItem c;

            for (int i = 0; i < lsTipo.Count; i++)
            {
                c = new ListItem();
                c.Dato = lsTipo[i].value;
                c.Info = lsTipo[i].name;
                lsTip.Add(c);
            }

            if (lsTip.Count == 0)
            {
                c = new ListItem();
                c.Dato = "DAMAG";
                c.Info = "DAMAG";
                lsTip.Add(c);
                //Danio9.Dato = "BLANCO";
            }
            //Color = lsColor[0].value;
        }

        private async void ListComponent()
        {
            lsComp = await s.get_component_listAsync();
            ListItem c;

            for (int i =0; i < lsComp.Count; i++)
            {
                c = new ListItem();
                c.Dato = lsComp[i].value;
                c.Info = lsComp[i].name;
                lsCom.Add(c);//, lsColor[i].value);
            }

            if (lsCom.Count == 0)
            {
                c = new ListItem();
                c.Dato = "COMP";
                c.Info = "COMP";
                lsCom.Add(c);
                //Danio10.Dato = "BLANCO";
            }
            //Color = lsColor[0].value;
        }

        private void LimpiarCampos()
        {
            CargaExp1 = "";
            CargaExp2 = "";
            CargaImp1 = null;
            CargaImp2 = null;
            TransExp = "";
            TransImp = "";
            SealCgsa1 = "";
            SealCgsa2 = "";
            Seal1 = "";
            Seal2 = "";
            Seal3 = "";
            Seal4 = "";
            Seal5 = "";
            Seal6 = "";
            Seal7 = "";
            Seal8 = "";
            Seal9 = "";
            Seal10 = "";
            Seal11 = "";
            Seal12 = "";
            Danio1 = null;
            Danio2 = null;
            Danio3 = "";
            Danio4 = "";
            Danio5 = null;
            Danio6 = null;
            Danio7 = "";
            Danio8 = "";
            Danio9 = null;
            Danio10 = null;
            Danio11 = "";
            Danio12 = "";
            Danio13 = null;
            Danio14 = null;
            Danio15 = "";
            Danio16 = "";
            Danio17 = null;
            Danio18 = null;
            Danio19 = "";
            Danio20 = "";
            val = true;
            tb = new ObservableCollection<InfoViewModel>();
        }

        private void LimpiarSellos()
        {
            SealCgsa3 = "";
            Seal9 = "";
            Seal10 = "";
            Seal11 = "";
            Seal12 = "";
        }

        private void LimpiarDanios()
        {
            Danio9 = null;
            Danio10 = null;
            Danio11 = "";
            Danio12 = "";
        }

        private void CargarSellos()
        {
            if (num == 1)
            {
                if (SealCgsa3 != null && SealCgsa3.Trim().Length > 0)
                {
                    SealCgsa1 = SealCgsa3.Trim().ToUpper();
                    Seal1 = Seal9!=null?Seal9.Trim().ToUpper():"";
                    Seal2 = Seal10!=null?Seal10.Trim().ToUpper():"";
                    Seal3 = Seal11!=null?Seal11.Trim().ToUpper():"";
                    Seal4 = Seal12!=null?Seal12.Trim().ToUpper():"";
                }
            }
            else
            {
                if (SealCgsa3 != null && SealCgsa3.Trim().Length > 0)
                {
                    SealCgsa2 = SealCgsa3.Trim().ToUpper();
                    Seal5 = Seal9 != null ? Seal9.Trim().ToUpper() : "";
                    Seal6 = Seal10 != null ? Seal10.Trim().ToUpper() : "";
                    Seal7 = Seal11 != null ? Seal11.Trim().ToUpper() : "";
                    Seal8 = Seal12 != null ? Seal12.Trim().ToUpper() : "";
                }
            }
        }

        private void CargarSellosF()
        {
            if (num == 1)
            {
                SealCgsa1 = "FARB001";
                Seal1 = "SS";
                Seal2 = "";
                Seal3 = "";
                Seal4 = "";
                farb1 = 1;
            }
            else
            {
                SealCgsa2 = "FARB001";
                Seal5 = "SS";
                Seal6 = "";
                Seal7 = "";
                Seal8 = "";
                farb2 = 1;
            }
        }

        private void DescargarSellos()
        {
            if (num == 1)
            {
                if (SealCgsa1 != null && SealCgsa1.Trim().Length > 0)
                {
                    SealCgsa3 = SealCgsa1.Trim().ToUpper();
                    Seal9 = Seal1;
                    Seal10 = Seal2;
                    Seal11 = Seal3;
                    Seal12 = Seal4;
                }
                farb1 = 0;
            }
            else
            {
                if (SealCgsa2 != null && SealCgsa2.Trim().Length > 0)
                {
                    SealCgsa3 = SealCgsa2.Trim().ToUpper();
                    Seal9 = Seal5;
                    Seal10 = Seal6;
                    Seal11 = Seal7;
                    Seal12 = Seal8;
                }
                farb2 = 0;
            }
        }

        private void CargarDanios()
        {
            if (num == 1)
            {
                if (Danio9 != null && Danio10 != null)
                {
                    Danio1 = Danio9;
                    Danio2 = Danio10;
                    Danio3 = Danio11!=null?Danio11.Trim().ToUpper():"MINOR";
                    Danio4 = Danio12!=null?Danio12.Trim().ToUpper():"RECEPTIO";
                }
            }
            else if (num == 2)
            {
                if (Danio9 != null && Danio10 != null)
                {
                    Danio5 = Danio9;
                    Danio6 = Danio10;
                    Danio7 = Danio11 != null ? Danio11.Trim().ToUpper() : "MINOR";
                    Danio8 = Danio12 != null ? Danio12.Trim().ToUpper() : "RECEPTIO";
                }
            }
            else if (num == 3)
            {
                if (Danio9 != null && Danio10 != null)
                {
                    Danio13 = Danio9;
                    Danio14 = Danio10;
                    Danio15 = Danio11 != null ? Danio11.Trim().ToUpper() : "MINOR";
                    Danio16 = Danio12 != null ? Danio12.Trim().ToUpper() : "RECEPTIO";
                }
            }
            else
            {
                if (Danio9 != null && Danio10 != null)
                {
                    Danio17 = Danio9;
                    Danio18 = Danio10;
                    Danio19 = Danio11 != null ? Danio11.Trim().ToUpper() : "MINOR";
                    Danio20 = Danio12 != null ? Danio12.Trim().ToUpper() : "RECEPTIO";
                }
            }
        }

        private void DescargarDanios()
        {
            if (num == 1)
            {
                if (Danio1 != null)
                {
                    Danio9 = Danio1;
                    Danio10 = Danio2;
                    Danio11 = Danio3;
                    Danio12 = Danio4;
                }
            }
            else if (num == 2)
            {
                if (Danio5 != null)
                {
                    Danio9 = Danio5;
                    Danio10 = Danio6;
                    Danio11 = Danio7;
                    Danio12 = Danio8;
                }
            }
            else if (num == 3)
            {
                if (Danio13 != null)
                {
                    Danio9 = Danio13;
                    Danio10 = Danio14;
                    Danio11 = Danio15;
                    Danio12 = Danio16;
                }
            }
            else
            {
                if (Danio14 != null)
                {
                    Danio9 = Danio17;
                    Danio10 = Danio18;
                    Danio11 = Danio19;
                    Danio12 = Danio20;
                }
            }
        }

        private void LimpiarGrid()
        {
            tb = new ObservableCollection<InfoViewModel>();
        }

        private void GuardarRecursosAplicacion(DatosLogin resultado)
        {
            App.Current.Resources.Add("LoginData", resultado);
        }

        public void IrVentanaPrincipal(bool esLider)
        {
            _ventanaAutenticacion.Frame.Navigate(typeof(ValidatePage));
        }

        public void IrVentanaSello(bool esLider)
        {
            _ventanaAutenticacion.Frame.Navigate(typeof(SealsPage));
        }

        public void IrVentanaDanio(bool esLider)
        {
            _ventanaAutenticacion.Frame.Navigate(typeof(DanioPage));
        }

        public void LlenarGrid(string tran, string dat, string inf, SolidColorBrush col)
        {
            var obj = new InfoViewModel();

            obj.Trans = tran.ToUpper();
            obj.Dato = dat.ToUpper();
            obj.Info = inf.ToUpper();
            obj.GColor = col;
            tb.Add(obj);
        }

        private void IrVentanaLogin(bool esLider)
        {
            _ventanaAutenticacion.Frame.Navigate(typeof(VentanaAutenticacion));
        }

        #endregion
    }
}

using Mobile.ServicioAutenticacion;
using Mobile.Vistas;
using Mobile.ViewModels;
using Mobile.ServiceTD;
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
using Mobile.ServicioMobile;

namespace Mobile.ViewModels
{
    internal class ProveedorPageViewModel : NotificationBase
    {
        #region Variables
        private readonly ProveedorPage _validaPage;
        private ObservableCollection<DatoGridViewModel> _tb;
        private ObservableCollection<combo_item> _lsTipo;
        private ObservableCollection<ListItem> _lsTip;
        private vip_find_passResponse pp;
        private container_item contain;
        private pre_gate_detail_container_item dcnt;
        private ServicioMobileClient w;
        private pre_gate_item cabt;
        private pre_gate_details_item dett;
        private save_tablet_transactionResponse trMa;
        private ObservableCollection<pre_gate_details_item> ldet;
        private ObservableCollection<string> lpas;
        private string _usuario;
        private string _chofer;
        private string _contrasena;
        private string _empresa;
        private string _ip;
        private string _zona;
        private string _host;
        private string _tranimp;
        private bool _bchf = true;
        private bool _bplc = true;
        private bool bchof = false;
        private bool bplac = false;
        private bool val;
        private int ap;
        private string turno;
        private string tip;
        private string empresa;
        private string chofer;
        private string placa;
        public bool bvalida = true;
        private int tipTran;
        private string mns;
        private Visibility _vs;
        private SolidColorBrush _gcolor;
        private ServiceClient s;
        private ServicioMobile.ZONE ZonaObj;
        int id_device;
        #endregion

        #region Constructor
        internal ProveedorPageViewModel(ProveedorPage ventanaAutenticacion)
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

            var endpoint1 = rmap.GetValue("DireccionServicioMobile", ctx).ValueAsString;
            w = new ServicioMobileClient(binding, new EndpointAddress(endpoint1));


            lsTipo = new ObservableCollection<combo_item>();
            chofer = ((ValidatePageViewModel)(App.Current.Resources["ValidaData"])).Chofer.Trim().ToUpper();
            placa = ((ValidatePageViewModel)(App.Current.Resources["ValidaData"])).Placa.Trim().ToUpper();
            Usuario = ((VentanaAutenticacionViewModel)(App.Current.Resources["UserData"])).Usuario;
            Host = ((VentanaAutenticacionViewModel)(App.Current.Resources["UserData"])).Host;
            Zona = ((VentanaAutenticacionViewModel)(App.Current.Resources["UserData"])).Zona;
            ListTipo();
            //tb = new ObservableCollection<DatoGridViewModel>();
        }
        #endregion

        #region Propiedades
        public string Transact
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

        public string Pase
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
            //return !string.IsNullOrWhiteSpace(Chofer) && !string.IsNullOrWhiteSpace(Placa);
            if (string.IsNullOrWhiteSpace(Transact))
            {
                GColor = new SolidColorBrush(Colors.Red);
                LlenarGrid("TRANSACCIÓN", "FALTA INGRESAR DATO", new SolidColorBrush(Colors.Red));
            }
            //else
            //    bchof = true;

            if (string.IsNullOrWhiteSpace(Pase))
            {
                GColor = new SolidColorBrush(Colors.Red);
                LlenarGrid("PASE", "FALTA INGRESAR DATO", new SolidColorBrush(Colors.Red));
            }
            //else
            //    bplac = true;

            return !string.IsNullOrWhiteSpace(Transact) && !string.IsNullOrWhiteSpace(Pase);
        }

       /* public async void Ingresar()
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

            if (!val)
            {
                LimpiarGrid();
                val = true;
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
                                if (CargaImp1 != null && CargaImp1.Dato.Trim().Length > 0)
                                    tipTran = 10;
                                else
                                    tipTran = 7;

                            LlenaDetalleM(CargaExp1.Trim(), tipTran, empresa, "E");

                            //lpas = new ObservableCollection<string>();
                            //lpas.Add(ap);
                            ldet.Add(dett);
                        }
                    }
                    if (tipTran != 8 || tipTran != 9)
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

                    if (tipTran != 8 || tipTran != 9)
                        if (CargaExp3 != null && CargaExp3.Trim().Length > 0)
                        {
                            dcnt = new pre_gate_detail_container_item();
                            await ValidarAisv(CargaExp3.Trim(), 3);

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
                                LlenaDetalleM(CargaExp3.Trim(), tipTran, empresa, "E");
                                //lpas.Add(ap);
                                ldet.Add(dett);
                            }
                        }

                    if (tipTran != 8 || tipTran != 9)
                        if (CargaExp4 != null && CargaExp4.Trim().Length > 0)
                        {
                            dcnt = new pre_gate_detail_container_item();
                            await ValidarAisv(CargaExp4.Trim(), 4);

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
                                LlenaDetalleM(CargaExp4.Trim(), tipTran, empresa, "E");
                                //lpas.Add(ap);
                                ldet.Add(dett);
                            }
                        }

                    if (CargaImp1 != null && CargaImp1.Dato.Trim().Length > 0)
                    {
                        try
                        {
                            LineaEdo = CargaImp1.Dato.Substring(4, 3);
                            if (CargaImp1 != null && CargaImp1.Dato.Trim().Length > 0)
                            {
                                //dcnt = new pre_gate_detail_container_item();
                                await ValidarEdo(CargaImp1.Dato.Trim(), 1);

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
                                    //cabt = new pre_gate_item();
                                    //ldet = new ObservableCollection<pre_gate_details_item>();

                                    LlenaDetalleM(CargaImp1.Dato.Trim(), tipTran, empresa, "I");
                                    lpas = new ObservableCollection<string>();
                                    lpas.Add(ap);
                                    ldet.Add(dett);
                                }
                            }

                            if (CargaImp2 != null && CargaImp2.Dato.Trim().Length > 0)
                            {
                                //dcnt = new pre_gate_detail_container_item();
                                if (LineaEdo != CargaImp2.Dato.Substring(4, 3))
                                {
                                    val = false;
                                    mns = "No puede solicitar EDOs de líneas diferentes";
                                    LlenarGrid("Problema", "EDO", mns, new SolidColorBrush(Colors.Red));
                                    bvalida = true;
                                    return;

                                }

                                await ValidarEdo(CargaImp2.Dato.Trim(), 2);

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
                                    LlenaDetalleM(CargaImp2.Dato.Trim(), tipTran, empresa, "I");
                                    lpas.Add(ap);
                                    ldet.Add(dett);
                                }
                            }

                            if (CargaImp3 != null && CargaImp3.Dato.Trim().Length > 0)
                            {
                                //dcnt = new pre_gate_detail_container_item();
                                if (LineaEdo != CargaImp3.Dato.Substring(4, 3))
                                {
                                    val = false;
                                    mns = "No puede solicitar EDOs de líneas diferentes";
                                    LlenarGrid("Problema", "EDO", mns, new SolidColorBrush(Colors.Red));
                                    bvalida = true;
                                    return;

                                }

                                await ValidarEdo(CargaImp3.Dato.Trim(), 3);

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
                                    LlenaDetalleM(CargaImp3.Dato.Trim(), tipTran, empresa, "I");
                                    lpas.Add(ap);
                                    ldet.Add(dett);
                                }
                            }

                            if (CargaImp4 != null && CargaImp4.Dato.Trim().Length > 0)
                            {
                                //dcnt = new pre_gate_detail_container_item();

                                if (LineaEdo != CargaImp4.Dato.Substring(4, 3))
                                {
                                    val = false;
                                    mns = "No puede solicitar EDOs de líneas diferentes";
                                    LlenarGrid("Problema", "EDO", mns, new SolidColorBrush(Colors.Red));
                                    bvalida = true;
                                    return;

                                }

                                await ValidarEdo(CargaImp4.Dato.Trim(), 4);

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
                                    LlenaDetalleM(CargaImp4.Dato.Trim(), tipTran, empresa, "I");
                                    lpas.Add(ap);
                                    ldet.Add(dett);
                                }
                            }

                        }
                        catch (Exception e)
                        {
                            LlenarGrid("Error", e.Message, new SolidColorBrush(Colors.Red));
                            //await ActualizaTransacTablet();
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

                    await mensajeDialogo.ShowAsync();
                    
                }
                catch (Exception e)
                {
                    LlenarGrid("Error", e.Message, new SolidColorBrush(Colors.Red));
                    //await ActualizaTransacTablet();
                }
            }

            bvalida = true;
        }*/

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

            if (!val)
            {
                Limpiar();
                val = true;
            }

            try
            { 

                Vs = Visibility.Collapsed;
                    //var mensajeDialogo1 = new MessageDialog("Prueba Carga", "Prueba");
                    //await mensajeDialogo1.ShowAsync();

                tipTran = int.Parse(tip);

                await ValidarPase(Pase.Trim(), 1);

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
                    //cabt = new pre_gate_item();
                    //ldet = new ObservableCollection<pre_gate_details_item>();

                    LlenaDetalleM(Pase.Trim(), tipTran, empresa, null);
                    lpas = new ObservableCollection<string>();
                    lpas.Add(ap.ToString());
                    ldet.Add(dett);
                }

                LlenaCabecera();
                await CreaTransacTablet();

                if (!val)
                {
                    //var mensajeDialogo = new MessageDialog("Problema", mns);
                    //await mensajeDialogo.ShowAsync();

                    LlenarGrid("Problema Transacción Tablet", mns, new SolidColorBrush(Colors.Red));
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


                bPlaca = false;
                bChofer = false;
                Vs = Visibility.Visible;
                //await mensajeDialogo1.ShowAsync();
                //return true;
            }catch(Exception e)
            {
                LlenarGrid("Error Ingresar Transacción", e.Message, new SolidColorBrush(Colors.Red));
            }
            bvalida = true;
        }

        private async Task ValidarPase(string pase, int num)
        {
            val = true;
            ap = 0;
            TransImp = "Servicio Varios";
            pp = null;
            contain = null;

            if (pase.Trim().Length > 0 && int.TryParse(pase, out ap))
            {
                var pr = new vip_find_passRequest();
                pr.pass_id = decimal.Parse(pase);
                pp = await s.vip_find_passAsync(pr);

                if ((pp != null) && (pp.msg == "" || pp.msg != null) && (pp.vip_find_passResult != null))
                {
                    //tip = pp.vip_find_passResult.;
                    ap = int.Parse(pp.vip_find_passResult.pass_id.ToString());

                    turno = pp.vip_find_passResult.end_date.ToString();
                    //TransImp = "Retiro Impo " + tip;

                    if (turno != null && turno.Trim().Length > 0)
                        LlenarGrid("Fecha Cad.", turno, new SolidColorBrush(Colors.Green));

                    pp.vip_find_passResult.user_01 = chofer;
                    pp.vip_find_passResult.truck_id = placa;

                    //var cit = new check_impedimentsRequest();
    
                    //valida chofer
                    var cdr = new vip_validate_driverRequest();
                    cdr.pass = pp.vip_find_passResult;
                    var cda = await s.vip_validate_driverAsync(cdr);

                    if (!cda.vip_validate_driverResult)
                    {
                        //var mensajeDialogo = new MessageDialog("Problema", cda.msg);
                        //await mensajeDialogo.ShowAsync();
                        if (val)
                            mns = cda.msg;
                        val = false;
                        LlenarGrid("Chofer", cda.msg, new SolidColorBrush(Colors.Red));
                    }
                    //valida camion
                    var ctr = new vip_validate_truckRequest();
                    ctr.pass = pp.vip_find_passResult;
                    var cca = await s.vip_validate_truckAsync(ctr);

                    if (!cca.vip_validate_truckResult)
                    {
                        //var mensajeDialogo = new MessageDialog("Problema", cca.msg);
                        //await mensajeDialogo.ShowAsync();
                        if (val)
                            mns = cca.msg;
                        val = false;
                        LlenarGrid("Camión", cca.msg, new SolidColorBrush(Colors.Red));
                    }
                    //valida date
                    var cpd = new vip_validate_dateRequest();
                    cpd.pass = pp.vip_find_passResult;
                    var cfa = await s.vip_validate_dateAsync(cpd);

                    if (!cfa.vip_validate_dateResult)
                    {
                        //var mensajeDialogo = new MessageDialog("Problema", cfa.msg);
                        //await mensajeDialogo.ShowAsync();
                        if (val)
                            mns = cfa.msg;
                        val = false;
                        LlenarGrid("Fecha", cfa.msg, new SolidColorBrush(Colors.Red));
                    }

                }
                else
                {
                    LlenarGrid("Pase", "Pase no existe", new SolidColorBrush(Colors.Red));
                    val = false;
                }
            }
            else
            {
                LlenarGrid("Pase", "No. de Pase inválido", new SolidColorBrush(Colors.Red));
                val = false;
            }
        }

        private async void ListTipo()
        {
            lsTipo = await s.get_transaction_types_listAsync();
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
                c.Dato = "BLANCO";
                c.Info = "BLANCO";
                lsTip.Add(c);
                //Danio9.Dato = "BLANCO";
            }
            //Color = lsColor[0].value;
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

        private void LlenaCabecera()
        {
            //llena cabecera transact
            cabt.device_id = id_device;
            cabt.driver_id = chofer;
            cabt.truck_licence = placa;
            cabt.user = Usuario;
            cabt.pre_gate_details = ldet;
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
            if (refe != null && refe.Trim().Length > 0)
                dett.reference_id = refe;
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

        public void VentanaInicio()
        {
            if (!bvalida)
                return;
            IrVentanaPrincipal(true);
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

        public void Limpiar()
        {
            if (!bvalida)
                return;
            LimpiarCampos();
        }

        private void LimpiarCampos()
        {
            Transact = "";
            Pase = "";
            bPlaca = true;
            bChofer = true;
            bchof = false;
            bplac = false;
            tb = new ObservableCollection<DatoGridViewModel>();
        }

        private void GuardarRecursosAplicacion(ProveedorPageViewModel resultado)
        {
            App.Current.Resources.Remove("ValidaData");
            App.Current.Resources.Add("ValidaData", resultado);
        }

        public void IrVentanaPrincipal(bool esLider)
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
            obj.Dato = dat;
            obj.Info = inf;
            //obj.GColor = "Red"; //new SolidColorBrush(Colors.Red);
            obj.GColor2 = col;
            tb.Add(obj);

        }
        #endregion
    }
}

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
    internal class SealsPageViewModel : NotificationBase
    {
        #region Variables
        private readonly SealsPage _validaPage;
        private string _cgsa;
        private string _seal1;
        private string _seal2;
        private string _seal3;
        private string _seal4;
        public bool bvalida = true;
        private ServiceClient s;
        #endregion

        #region Constructor
        internal SealsPageViewModel(SealsPage ventanaAutenticacion)
        {
            _validaPage = ventanaAutenticacion;
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
            //w = new ServicioMobileClient(binding, new EndpointAddress(endpoint1));
        }
        #endregion

        #region Propiedades
        public string CGSA
        {
            get
            {
                return _cgsa;
            }
            set
            {
                if (_cgsa == value)
                    return;
                _cgsa = value;
                RaisePropertyChanged("CGSA");
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


        #endregion

        #region Metodos

        public void Ingresar()
        {
            if (!bvalida)
                return;
            bvalida = false;
            GuardarRecursosAplicacion(this);
            bvalida = true;
            IrVentanaPrincipal(true);
        }
        
        public void Regresar()
        {
            //var mensajeDialogo = new MessageDialog("No puede continuar", tb[0].GColor.ToString());
            //await mensajeDialogo.ShowAsync();
            //return;
            if (!bvalida)
                return;
            IrVentanaPrincipal(true);
        }

        public void Limpiar()
        {
            if (!bvalida)
                return;
            LimpiarCampos();
        }

        private void LimpiarCampos()
        {
            CGSA = "";
            Seal1 = "";
        }

        private void GuardarRecursosAplicacion(SealsPageViewModel resultado)
        {
            App.Current.Resources.Remove("SealsData");
            App.Current.Resources.Add("SealsData", resultado);
        }

        private void IrVentanaPrincipal(bool esLider)
        {
            _validaPage.Frame.GoBack();// Navigate(typeof(MainPage));
        }

        #endregion
    }
}

using Mobile.Vistas;
using Mobile.ViewModels;
using RECEPTIO.CapaPresentacion.UWP.MVVM;
using System;
using System.Collections.ObjectModel;
using System.Net;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Xaml;
using System.Threading.Tasks;
using Mobile.ServiceTransact;

namespace Mobile.ViewModels
{
    internal class DanioPageViewModel : NotificationBase
    {
        #region Variables
        private readonly DanioPage _danioPage;
        private ObservableCollection<combo_item> _lsTipo;
        private ObservableCollection<combo_item> _lsComp;
        private string _tipo;
        private string _comp;
        private string _severo;
        private string _descrip;
        //private ObservableCollection<DanioGridViewModel> _tb;
        public bool bvalida = true;
        #endregion

        #region Constructor
        internal DanioPageViewModel(DanioPage danioPageG)
        {
            _danioPage = danioPageG;
        }
        #endregion

        #region Propiedades

        //public ObservableCollection<DanioGridViewModel> tb
        //{
        //    get
        //    {
        //        return _tb;
        //    }
        //    set
        //    {
        //        SetProperty(ref _tb, value);
        //    }
        //}

        public string Tipo
        {
            get
            {
                return _tipo;
            }
            set
            {
                if (_tipo == value)
                    return;
                _tipo = value;
                RaisePropertyChanged("Tipo");
            }
        }

        public string Comp
        {
            get
            {
                return _comp;
            }
            set
            {
                if (_comp == value)
                    return;
                _comp = value;
                RaisePropertyChanged("Comp");
            }
        }

        public string Severo
        {
            get
            {
                return _severo;
            }
            set
            {
                if (_severo == value)
                    return;
                _severo = value;
                RaisePropertyChanged("Severo");
            }
        }

        public string Descrip
        {
            get
            {
                return _descrip;
            }
            set
            {
                if (_descrip == value)
                    return;
                _descrip = value;
                RaisePropertyChanged("Descrip");
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
            //tb = new ObservableCollection<DanioGridViewModel>();
            Tipo = "";
            Comp = "";
            Severo = "";
            Descrip = "";
        }

        private void GuardarRecursosAplicacion(DanioPageViewModel resultado)
        {
            App.Current.Resources.Remove("DanioData");
            App.Current.Resources.Add("DanioData", resultado);
        }

        private void IrVentanaPrincipal(bool esLider)
        {
            _danioPage.Frame.GoBack();//.Navigate(typeof(MainPage));
        }

        private void LlenarGrid(bool dat, string inf, string cod)
        {
            
            var obj = new DanioGridViewModel();
            obj.Dato = dat;
            obj.Info = inf;
            //obj.GColor = "Red"; //new SolidColorBrush(Colors.Red);
            obj.Codigo = cod;
            //tb.Add(obj);

        }

        #endregion
    }
}

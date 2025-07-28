using Mobile.ServicioAutenticacion;
using Mobile.Vistas;
using RECEPTIO.CapaPresentacion.UWP.MVVM;
using System;
using System.Net;
using System.ServiceModel;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media;

namespace Mobile.ViewModels
{
    internal class InfoViewModel
    {
        #region Variables
        private string _trans;
        private string _dato;
        private string _info;
        SolidColorBrush _gcolor;
        //private string _ip;
        #endregion

        #region Constructor
        internal InfoViewModel()
        {
            GColor = new SolidColorBrush(Colors.Green);
        }
        #endregion

        #region Propiedades
        public string Trans
        {
            get
            {
                return _trans;
            }
            set
            {
                if (_trans == value)
                    return;
                _trans = value;
            }
        }

        public string Dato
        {
            get
            {
                return _dato;
            }
            set
            {
                if (_dato == value)
                    return;
                _dato = value;
            }
        }

        public string Info
        {
            get
            {
                return _info;
            }
            set
            {
                if (_info == value)
                    return;
                _info = value;
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
            }
        }

        #endregion

        #region Metodos
        //private async void ObtenerIp()
        //{
        //    var ip = await Dns.GetHostEntryAsync(Dns.GetHostName());
        //    _ip = ip.AddressList[1].ToString();
        //}

        private bool PuedoIngresar()
        {
            return !string.IsNullOrWhiteSpace(Trans) && !string.IsNullOrWhiteSpace(Dato);
        }

        public async void Ingresar()
        {
            if (!PuedoIngresar())
            {
                var mensajeDialogo = new MessageDialog("Error", "Faltan Datos.");
                await mensajeDialogo.ShowAsync();
                return;
            }

        }

        private void GuardarRecursosAplicacion(DatosLogin resultado)
        {
            App.Current.Resources.Add("InfoData", resultado);
        }


        #endregion
    }
}

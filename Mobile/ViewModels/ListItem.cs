using Mobile.ServicioAutenticacion;
using Mobile.Vistas;
using RECEPTIO.CapaPresentacion.UWP.MVVM;
using System;
using System.Globalization;
using System.Net;
using System.ServiceModel;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Mobile.ViewModels
{
    internal class ListItem
    {
        #region Variables
        private string _dato;
        private string _info;
        //private string _gcolor;
        #endregion

        #region Constructor
        internal ListItem()
        {
            //await mensajeDialogo.ShowAsync();
        }
        #endregion

        #region Propiedades

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

        #endregion

        #region Metodos
        #endregion
    }

}

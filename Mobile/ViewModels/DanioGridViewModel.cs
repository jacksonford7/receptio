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
    internal class DanioGridViewModel
    {
        #region Variables
        private bool _dato;
        private string _info;
        private string _codigo;
        #endregion

        #region Constructor
        internal DanioGridViewModel()
        {
            //await mensajeDialogo.ShowAsync();
        }
        #endregion

        #region Propiedades

        public bool Dato
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

        public string Codigo
        {
            get
            {
                return _codigo;
            }
            set
            {
                if (_codigo == value)
                    return;
                _codigo = value;
            }
        }

        #endregion

        #region Metodos
        private void GuardarRecursosAplicacion(DatosLogin resultado)
        {
            App.Current.Resources.Add("GridDanio", resultado);
        }

        #endregion
    }
}




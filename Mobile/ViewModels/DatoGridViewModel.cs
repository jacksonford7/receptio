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
    internal class DatoGridViewModel
    {
        #region Variables
        private string _dato;
        private string _info;
        private SolidColorBrush _gcolor2;
        //private string _gcolor;
        #endregion

        #region Constructor
        internal DatoGridViewModel()
        {
            GColor2 = new SolidColorBrush(Colors.Green);
            //GColor = "Green";
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

        //public string GColor
        //{
        //    get
        //    {
        //        return _gcolor;
        //    }
        //    set
        //    {
        //        if (_gcolor == value)
        //            return;
        //        _gcolor = value;
        //    }
        //}

        public SolidColorBrush GColor2
        {
            get
            {
                return _gcolor2;
            }
            set
            {
                if (_gcolor2 == value)
                    return;
                _gcolor2 = value;
            }
        }

        #endregion

        #region Metodos
        private bool PuedoIngresar()
        {
            return !string.IsNullOrWhiteSpace(Info) && !string.IsNullOrWhiteSpace(Dato);
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
            App.Current.Resources.Add("GridData", resultado);
        }



        #endregion
    }

}




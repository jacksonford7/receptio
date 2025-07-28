using Console.ServicioConsole;
using RECEPTIO.CapaPresentacion.UWP.MVVM;
using System;
using System.ServiceModel;

namespace Console.ViewModels
{
    internal class Base : NotificationBase
    {
        public string Usuario
        {
            get
            {
                return ((DatosLogin)App.Current.Resources["DatosLogin"]).Usuario;
            }
        }

        public string Ip
        {
            get
            {
                return ((DatosLogin)App.Current.Resources["DatosLogin"]).Ip;
            }
        }

        protected Tuple<bool, ServicioTransactionQuiosco.ContratoClient> QuioscoTieneConeccion(string ip)
        {
            try
            {
                var binding = new BasicHttpBinding();
                var endpoint = $"http://{ip}:17101/ServicioTransactionQuiosco/";
                var servicioQuiosco = new ServicioTransactionQuiosco.ContratoClient(binding, new EndpointAddress(endpoint));
                return new Tuple<bool, ServicioTransactionQuiosco.ContratoClient>(true, servicioQuiosco);
            }
            catch (Exception)
            {
                return new Tuple<bool, ServicioTransactionQuiosco.ContratoClient>(false, null);
            }
        }
    }
}

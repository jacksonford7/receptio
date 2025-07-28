using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.Windows;
using Transaction.Ayudas;
using Transaction.ServicioTransaction;

namespace Transaction
{
    public partial class App : Application
    {
        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            var actualizadorAutomatico = new ActualizadorAutomatico("TRANSACTION", ResourceAssembly.Location, "Transaction", "RTM");
            //if (!actualizadorAutomatico.EsActualVersion(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()))
            //{
            //    actualizadorAutomatico.ActualizarVersion();
            //    Current.Shutdown();
            //}
        }

        private void ApplicationDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (!(e.Exception is FaultException))
            {
                try
                {
                    var idError = CrearErrorCliente(e.Exception);
                    EnviarProblemaTecnico(new Exception($"Reportelo con el código :{idError}"));
                }
                catch (Exception ex)
                {
                    EnviarProblema(ex);
                }
            }
            else if(e.Exception.Message.Contains("Reportelo con el código :"))
                EnviarProblemaTecnico(e.Exception);
            else
                EnviarProblema(e.Exception);
            e.Handled = true;
        }

        private int CrearErrorCliente(Exception exception)
        {
            using (var servicio = new ServicioTransactionClient())
            {
                return servicio.CrearError(new ERROR
                {
                    IP_SOURCE = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString(),
                    APPLICATION_ID = 2,
                    MESSAGE = exception.Message,
                    DETAILS = $"Mensaje : {exception.Message}///Excepción Interna : {exception.InnerException}///Pila de Seguimiento : {exception.StackTrace}///Fuente : {exception.Source}///Link : {exception.HelpLink}"
                });
            }
        }

        private void EnviarProblemaTecnico(Exception e)
        {
            try
            {
                AnunciarProblemaTecnico(e.Message);
            }
            catch (Exception ex)
            {
                LoguearError($"Mensaje : {ex.Message}///Excepción Interna : {ex.InnerException}///Pila de Seguimiento : {ex.StackTrace}///Fuente : {ex.Source}///Link : {ex.HelpLink}");
            }
        }

        private void AnunciarProblemaTecnico(string mensaje)
        {
            using (var servicio = new ServicioAnuncianteProblema.ServicioAnuncianteProblemaClient())
            {
                servicio.AnunciarProblemaClienteAppTransaction(Convert.ToInt32(mensaje.Split(':')[1]), ((KIOSK)App.Current.Resources["Quiosco"]).ZONE_ID);
            }
        }

        private void EnviarProblema(Exception e)
        {
            try
            {
                AnunciarProblema(e.Message);
            }
            catch (Exception ex)
            {
                LoguearError($"Mensaje : {ex.Message}///Excepción Interna : {ex.InnerException}///Pila de Seguimiento : {ex.StackTrace}///Fuente : {ex.Source}///Link : {ex.HelpLink}");
            }
        }

        private void AnunciarProblema(string mensaje)
        {
            using (var servicio = new ServicioAnuncianteProblema.ServicioAnuncianteProblemaClient())
            {
                servicio.AnunciarProblemaServicioWebTransaction(mensaje, ((KIOSK)App.Current.Resources["Quiosco"]).ZONE_ID, 2);
            }
        }

        private void LoguearError(string mensaje)
        {
            try
            {
                var ruta = Path.Combine(ConfigurationManager.AppSettings["RutaLog"], ConfigurationManager.AppSettings["NombreArchivoLog"]);
                var sw = new StreamWriter(ruta, true);
                sw.WriteLine($"Fecha: {DateTime.Now}. {mensaje}");
                sw.Close();
                MessageBox.Show($"Ha ocurrido un inconveniente.{Environment.NewLine}Contactese con el Departamento de IT.{Environment.NewLine}", "TRANSACTION", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception)
            {
                MessageBox.Show($"Ha ocurrido un inconveniente y no se pudo loguearlo.{Environment.NewLine}Contactarse con el Departamento de IT.{Environment.NewLine}{mensaje}", "TRANSACTION", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

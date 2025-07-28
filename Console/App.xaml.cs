using Console.ServicioConsole;
using Console.Vistas;
using System;
using System.Net;
using System.ServiceModel;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Console
{
    sealed partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
            UnhandledException += AppUnhandledException;
        }

        private async void AppUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            var mensaje = e.Message;
            var detalle = $"Mensaje : {e.Exception.Message}///Excepción Interna : {e.Exception.InnerException}///Pila de Seguimiento : {e.Exception.StackTrace}///Fuente : {e.Exception.Source}///Link : {e.Exception.HelpLink}";
            e.Handled = true;
            UnhandledException -= AppUnhandledException;
            UnhandledException += AppUnhandledException2;
            if (e.Exception is FaultException)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    var mensajeDialogo = new MessageDialog(e.Message, "CONSOLE");
                    await mensajeDialogo.ShowAsync();
                });
            }
            else
                CrearErrorCliente(mensaje, detalle);
            UnhandledException -= AppUnhandledException2;
            UnhandledException += AppUnhandledException;
        }

        private void AppUnhandledException2(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            UnhandledException -= AppUnhandledException2;
            UnhandledException += AppUnhandledException3;
            LoguearError(e.Exception);
            UnhandledException -= AppUnhandledException3;
            UnhandledException += AppUnhandledException;
        }

        private async void AppUnhandledException3(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            UnhandledException -= AppUnhandledException3;
            UnhandledException += AppUnhandledException;
            var mensaje = $"Mensaje : {e.Exception.Message}///Excepción Interna : {e.Exception.InnerException}///Pila de Seguimiento : {e.Exception.StackTrace}///Fuente : {e.Exception.Source}///Link : {e.Exception.HelpLink}";
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            async () =>
            {
                var mensajeDialogo = new MessageDialog($"Ha ocurrido un inconveniente y no se pudo loguear.{Environment.NewLine}Mensaje : {mensaje}", "CONSOLE");
                await mensajeDialogo.ShowAsync();
            });
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (!(Window.Current.Content is Frame rootFrame))
            {
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;
                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                }
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                    rootFrame.Navigate(typeof(VentanaAutenticacion), e.Arguments);
                Window.Current.Activate();
            }
            Application.Current.Resources["SystemControlHighlightListAccentLowBrush"] = new SolidColorBrush(Colors.Cornsilk);
            Application.Current.Resources["SystemControlHighlightListAccentMediumBrush"] = new SolidColorBrush(Colors.Cornsilk);
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }

        private async void CrearErrorCliente(string mensaje, string detalle)
        {
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
            var endpoint = rmap.GetValue("DireccionServicio", ctx).ValueAsString;
            var servicio = new ServicioConsoleClient(binding, new EndpointAddress(endpoint));
            var ip = await Dns.GetHostEntryAsync(Dns.GetHostName());
            var id = await servicio.CrearErrorAsync(new ERROR
            {
                IP_SOURCE = ip.AddressList[1].ToString(),
                APPLICATION_ID = 3,
                MESSAGE = mensaje,
                DETAILS = detalle
            });
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            async () =>
            {
                var mensajeDialogo = new MessageDialog($"Ha ocurrido un inconveniente.{Environment.NewLine}Repórtelo con el código : {id}", "CONSOLE");
                await mensajeDialogo.ShowAsync();
            });
        }

        private async void LoguearError(Exception ex)
        {
            Windows.ApplicationModel.Resources.Core.ResourceContext ctx = new Windows.ApplicationModel.Resources.Core.ResourceContext();
            Windows.ApplicationModel.Resources.Core.ResourceMap rmap = Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap.GetSubtree("Recursos");
            Windows.Storage.StorageFolder carpeta = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile archivo = await carpeta.GetFileAsync($"{rmap.GetValue("NombreArchivoLog", ctx).ValueAsString}.txt");
            var mensaje = $"Mensaje : {ex.Message}///Excepción Interna : {ex.InnerException}///Pila de Seguimiento : {ex.StackTrace}///Fuente : {ex.Source}///Link : {ex.HelpLink}";
            await Windows.Storage.FileIO.WriteTextAsync(archivo, $"Fecha: {DateTime.Now}{Environment.NewLine}Mesanje : {mensaje}");
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            async () =>
            {
                var mensajeDialogo = new MessageDialog($"Ha ocurrido un inconveniente.{Environment.NewLine}Comuniquese con IT.", "CONSOLE");
                await mensajeDialogo.ShowAsync();
            });
        }
    }
}

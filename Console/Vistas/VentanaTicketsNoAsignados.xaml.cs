using Console.ServicioConsole;
using Console.ViewModels;
using System;
using Windows.UI.Core.Preview;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Console.Vistas
{
    public sealed partial class VentanaTicketsNoAsignados : Page
    {
        private VentanaTicketsNoAsignadosViewModel _viewModel;

        public VentanaTicketsNoAsignados()
        {
            InitializeComponent();
            SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += OnCloseRequest;
            ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (Convert.ToInt32(e.Parameter) == 0)
                _viewModel = new VentanaTicketsNoAsignadosViewModel(this);
            else
                _viewModel = new VentanaTicketsSuspendidosViewModel(this);
            base.OnNavigatedTo(e);
        }

        private async void OnCloseRequest(object sender, SystemNavigationCloseRequestedPreviewEventArgs e)
        {
            if (App.Current.Resources.TryGetValue("DatosLogin", out object datosLogin))
            {
                var servicio = (ServicioConsoleClient)App.Current.Resources["ServicioConsole"];
                await servicio.CerrarSesionAsync(((DatosLogin)App.Current.Resources["DatosLogin"]).IdSesion);
            }
        }
    }
}

using Console.ServicioConsole;
using Console.ViewModels;
using Windows.UI.Core.Preview;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;

namespace Console.Vistas
{
    public sealed partial class VentanaKioscos : Page
    {
        private VentanaKioscosViewModel _viewModel;

        public VentanaKioscos()
        {
            InitializeComponent();
            SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += OnCloseRequest;
            ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            _viewModel = new VentanaKioscosViewModel(this);
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

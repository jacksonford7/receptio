using Console.ServicioConsole;
using Console.ViewModels;
using Windows.System;
using Windows.UI.Core.Preview;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Console.Vistas
{
    public sealed partial class VentanaImpresion : Page
    {
        private VentanaImpresionViewModel _viewModel;

        public VentanaImpresion()
        {
            InitializeComponent();
            SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += OnCloseRequest;
            ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            _viewModel = new VentanaImpresionViewModel(this);
        }

        private async void OnCloseRequest(object sender, SystemNavigationCloseRequestedPreviewEventArgs e)
        {
            if (App.Current.Resources.TryGetValue("DatosLogin", out object datosLogin))
            {
                var servicio = (ServicioConsoleClient)App.Current.Resources["ServicioConsole"];
                await servicio.CerrarSesionAsync(((DatosLogin)App.Current.Resources["DatosLogin"]).IdSesion);
            }
        }

        private void TextBoxKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key >= VirtualKey.Number0 && e.Key <= VirtualKey.Number9 || e.Key >= VirtualKey.NumberPad0 && e.Key <= VirtualKey.NumberPad9 || e.Key == VirtualKey.Tab)
                e.Handled = false;
            else
                e.Handled = true;
        }
    }
}

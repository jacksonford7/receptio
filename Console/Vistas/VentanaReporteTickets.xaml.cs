using Console.ServicioConsole;
using Console.ViewModels;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.System;
using Windows.UI.Core.Preview;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Console.Vistas
{
    public sealed partial class VentanaReporteTickets : Page
    {
        private VentanaReporteTicketsViewModel _viewModel;

        public VentanaReporteTickets()
        {
            InitializeComponent();
            SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += OnCloseRequest;
            ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            _viewModel = new VentanaReporteTicketsViewModel(this);
        }

        private void TextBoxKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key >= VirtualKey.Number0 && e.Key <= VirtualKey.Number9 || e.Key >= VirtualKey.NumberPad0 && e.Key <= VirtualKey.NumberPad9 || e.Key == VirtualKey.Tab)
                e.Handled = false;
            else
                e.Handled = true;
        }

        private async void OnCloseRequest(object sender, SystemNavigationCloseRequestedPreviewEventArgs e)
        {
            if (App.Current.Resources.TryGetValue("DatosLogin", out object datosLogin))
            {
                var servicio = (ServicioConsoleClient)App.Current.Resources["ServicioConsole"];
                await servicio.CerrarSesionAsync(((DatosLogin)App.Current.Resources["DatosLogin"]).IdSesion);
            }
        }

        private void DataGrid_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (((DataGrid)sender).CurrentColumn == null || ((DataGrid)sender).SelectedItem == null || ((DataGrid)sender).CurrentColumn.DisplayIndex != 9)
                return;
            var mensaje = ((TextBlock)((DataGrid)sender).CurrentColumn.GetCellContent(((DataGrid)sender).SelectedItem)).Text;
            var mensajeDialogo = new MessageDialog(mensaje, ((DataGrid)sender).CurrentColumn.Header.ToString());
#pragma warning disable CS4014 // Ya que no se esperaba esta llamada, la ejecución del método actual continúa antes de que se complete la llamada
            mensajeDialogo.ShowAsync();
#pragma warning restore CS4014 // Ya que no se esperaba esta llamada, la ejecución del método actual continúa antes de que se complete la llamada
        }
    }
}

using Console.ServicioConsole;
using Console.ViewModels;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System.Linq;
using Windows.UI.Core.Preview;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;

namespace Console.Vistas
{
    public sealed partial class VentanaPrincipal : Page
    {
        private VentanaPrincipalViewModel _viewModel;

        public VentanaPrincipal()
        {
            InitializeComponent();
            SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += OnCloseRequest;
            ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            _viewModel = new VentanaPrincipalViewModel(this);
        }

        private async void OnCloseRequest(object sender, SystemNavigationCloseRequestedPreviewEventArgs e)
        {
            if (_viewModel.Tickets == null || _viewModel.Tickets.All(t => t.FechaFinalizacion.HasValue))
            {
                if (App.Current.Resources.TryGetValue("DatosLogin", out object datosLogin))
                {
                    var servicio = (ServicioConsoleClient)App.Current.Resources["ServicioConsole"];
                    await servicio.CerrarSesionAsync(((DatosLogin)App.Current.Resources["DatosLogin"]).IdSesion);
                }
            }
            else
            {
                var mensajeDialogo = new MessageDialog("No puede cerrar Sesión ya que posee tickets pendientes de resolver.", "No se puede cerrar la aplicación.");
#pragma warning disable CS4014
                mensajeDialogo.ShowAsync();
#pragma warning restore CS4014
                e.Handled = true;
            }
        }

        private void DataGrid_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (((DataGrid)sender).CurrentColumn == null || ((DataGrid)sender).SelectedItem == null || ((DataGrid)sender).CurrentColumn.DisplayIndex < 3)
                return;
            var mensaje = ((TextBlock)((DataGrid)sender).CurrentColumn.GetCellContent(((DataGrid)sender).SelectedItem)).Text;
            var mensajeDialogo = new MessageDialog(mensaje, ((DataGrid)sender).CurrentColumn.Header.ToString());
#pragma warning disable CS4014 // Ya que no se esperaba esta llamada, la ejecución del método actual continúa antes de que se complete la llamada
            mensajeDialogo.ShowAsync();
#pragma warning restore CS4014 // Ya que no se esperaba esta llamada, la ejecución del método actual continúa antes de que se complete la llamada
        }

        private void DataGridPointerPressedMensajesSmdt(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (((DataGrid)sender).CurrentColumn == null || ((DataGrid)sender).SelectedItem == null || ((DataGrid)sender).CurrentColumn.DisplayIndex < 0)
                return;
            var mensaje = ((TextBlock)((DataGrid)sender).CurrentColumn.GetCellContent(((DataGrid)sender).SelectedItem)).Text;
            var mensajeDialogo = new MessageDialog(mensaje, ((DataGrid)sender).CurrentColumn.Header.ToString());
#pragma warning disable CS4014 // Ya que no se esperaba esta llamada, la ejecución del método actual continúa antes de que se complete la llamada
            mensajeDialogo.ShowAsync();
#pragma warning restore CS4014 // Ya que no se esperaba esta llamada, la ejecución del método actual continúa antes de que se complete la llamada
        }
    }
}

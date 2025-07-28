using Console.ViewModels;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;

namespace Console.Vistas
{
    public sealed partial class VentanaAutenticacion : Page
    {
        private VentanaAutenticacionViewModel _viewModel;

        public VentanaAutenticacion()
        {
            InitializeComponent();
            ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            _viewModel = new VentanaAutenticacionViewModel(this);
        }

        private void PswContrasenaKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter && _viewModel.PuedoIngresar(null))
                _viewModel.Ingresar(null);
        }
    }
}

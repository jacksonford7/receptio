using Console.ViewModels;
using System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Console.Vistas
{
    public sealed partial class VentanaDescanso : Page
    {
        private VentanaDescansoViewModel _viewModel;

        public VentanaDescanso()
        {
            InitializeComponent();
            ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _viewModel = new VentanaDescansoViewModel(this, Convert.ToInt32(e.Parameter));
            base.OnNavigatedTo(e);
        }
    }
}

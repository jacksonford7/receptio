using Mobile.ViewModels;
using Windows.UI.Xaml.Controls;
using System.Collections.Generic;
using Windows.UI.Xaml.Media.Imaging;
using System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.UI.Controls;

namespace Mobile.Vistas
{
    public sealed partial class VentanaAutenticacion : Page
    {
        private VentanaAutenticacionViewModel _viewModelAut;

        public VentanaAutenticacion()
        {
            InitializeComponent();
            _viewModelAut = new VentanaAutenticacionViewModel(this);
            //ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            List<string> twoLists = new List<string>();
            twoLists.Add("INGRESO");
            twoLists.Add("SALIDA");
            f_List.ItemsSource = twoLists;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            progress1.IsActive = true;
            progress1.Visibility = Visibility.Visible;

            _viewModelAut.Ingresar();
            //var s = b.Status;
            while (!(_viewModelAut.bvalida)) { await Task.Delay(TimeSpan.FromSeconds(2)); }

            progress1.IsActive = false;
            progress1.Visibility = Visibility.Collapsed;
        }

    }
}

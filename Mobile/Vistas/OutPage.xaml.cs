using Mobile.ViewModels;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.UI.Controls;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Mobile.Vistas
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OutPage : Page
    {
        private OutPageViewModel _viewModelAut;
        

        public OutPage()
        {
            InitializeComponent();
            _viewModelAut = new OutPageViewModel(this);
            //ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            //Dictionary<string, string> twoLists = new Dictionary<string, string>();
            //twoLists.Add("IN", "IN");
            //twoLists.Add("OUT", "OUT");
            //f_List.ItemsSource = twoLists;
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private async void Click_Validar(object sender, RoutedEventArgs e)
        {
            progress1.IsActive = true;
            progress1.Visibility = Visibility.Visible;

            _viewModelAut.Validar();

            while (!(_viewModelAut.bvalida)) { await Task.Delay(TimeSpan.FromSeconds(2)); }

            progress1.IsActive = false;
            progress1.Visibility = Visibility.Collapsed;
        }

        private async void Click_Procesar(object sender, RoutedEventArgs e)
        {
            progress1.IsActive = true;
            progress1.Visibility = Visibility.Visible;

            _viewModelAut.Ingresar();

            while (!(_viewModelAut.bvalida)) { await Task.Delay(TimeSpan.FromSeconds(2)); }

            progress1.IsActive = false;
            progress1.Visibility = Visibility.Collapsed;
        }

        //private void Contenedor_KeyDown(object sender, KeyRoutedEventArgs e)
        //{
        //    if ((e.Key >= VirtualKey.NumberPad0 && e.Key <= VirtualKey.NumberPad9) || (e.Key >= VirtualKey.Number0 && e.Key <= VirtualKey.Number0) || e.Key == VirtualKey.Enter)
        //    {
        //        e.Handled = false;
        //        if (e.Key != VirtualKey.Enter)
        //            return;
        //    }
        //    else
        //        e.Handled = true;
        //}

        private void DGResult_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            //var g = (DataGrid)sender;
            //ObservableCollection<DatoGridViewModel> i = (ObservableCollection<DatoGridViewModel>)g.ItemsSource;
            e.Row.Foreground = _viewModelAut.tb[e.Row.GetIndex()].GColor2;
        }

        //private void LicenciaKeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyStatus >= Key.D0 && e.KeyStatus <= Key.D9 || e.KeyStatus >= Key.NumPad0 && e.KeyStatus <= Key.NumPad9 || e.KeyStatus == Key.Enter)
        //    {
        //        e.Handled = false;
        //        if (e.KeyStatus != Key.Enter)
        //            return;
        //    }
        //    else
        //        e.Handled = true;
        //}
    }
}

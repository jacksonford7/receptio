using Mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Windows;
using System.Windows.Input;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Globalization;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.System;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System.Threading.Tasks;
using Windows.UI.Popups;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Mobile.Vistas
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ValidatePage : Page
    {
        private ValidatePageViewModel _viewModelAut;
        

        public ValidatePage()
        {
            InitializeComponent();
            _viewModelAut = new ValidatePageViewModel(this);
            //ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            //Dictionary<string, string> twoLists = new Dictionary<string, string>();
            //twoLists.Add("IN", "IN");
            //twoLists.Add("OUT", "OUT");
            //f_List.ItemsSource = twoLists;
        }

        //private void Licencia_KeyDown(object sender, KeyRoutedEventArgs e)
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

        private async void  Click_Validar(object sender, RoutedEventArgs e)
        {
            progress1.IsActive = true;
            progress1.Visibility = Visibility.Visible;

            _viewModelAut.Validar();
            //var s = b.Status;
            while (!(_viewModelAut.bvalida)) {await Task.Delay(TimeSpan.FromSeconds(2)); }

            progress1.IsActive = false;
            progress1.Visibility = Visibility.Collapsed;

        }

        private void DGResult_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            //var g = (DataGrid)sender;
            e.Row.Foreground = _viewModelAut.tb[e.Row.GetIndex()].GColor2;
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }
    }

    //public class MiConvertidor : ViewModels.IValueConverter
    //{

    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        string dg = (string)value;

    //        if (dg == "rojo") return new SolidColorBrush(Colors.Red);
    //        if (dg == "verde") return new SolidColorBrush(Colors.Green);
    //        if (dg == "") return new SolidColorBrush(Colors.Black);

    //        return new SolidColorBrush(Colors.Green);
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}

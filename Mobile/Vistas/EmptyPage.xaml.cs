using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Data;
using System.Globalization;
using Mobile.ViewModels;
using System.Collections.ObjectModel;
using Windows.UI.ViewManagement;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System.Threading.Tasks;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Mobile.Vistas
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EmptyPage : Page
    {
        private EmptyPageViewModel _viewModelAut;
        private bool binicia=false;
        public EmptyPage()
        {
            if (!binicia)
            {
                this.InitializeComponent();
                _viewModelAut = new EmptyPageViewModel(this);
            }
            //binicia = true;
            //ApplicationView.GetForCurrentView().TryEnterFullScreenMode();

            List<string> twoLists = new List<string>();
            twoLists.Add("MAJOR");
            twoLists.Add("MINOR");
            f_List.ItemsSource = twoLists;
        }

        private async void Click_Procesar(object sender, RoutedEventArgs e)
        {
            progress1.IsActive = true;
            progress1.Visibility = Visibility.Visible;

            _viewModelAut.Ingresar();
            //var s = b.Status;
            while (!(_viewModelAut.bvalida)) { await Task.Delay(TimeSpan.FromSeconds(2)); }

            progress1.IsActive = false;
            progress1.Visibility = Visibility.Collapsed;

        }

        private void DGResult_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            //var g = (DataGrid)sender;
            e.Row.Foreground = _viewModelAut.tb[e.Row.GetIndex()].GColor;
        }

        private async void Sellos1_Click(object sender, RoutedEventArgs e)
        {
            _viewModelAut.num = 1;
            _viewModelAut.SellosUp();
            while (!(_viewModelAut.binicia)) { await Task.Delay(TimeSpan.FromSeconds(2)); }
            if (_viewModelAut.farb1 != 1)
            {
                GPop.Visibility = Visibility.Visible;
                popup1.IsOpen = true;
            }
        }

        private async void Sellos2_Click(object sender, RoutedEventArgs e)
        {
            _viewModelAut.num = 2;
            _viewModelAut.SellosUp();
            while (!(_viewModelAut.binicia)) { await Task.Delay(TimeSpan.FromSeconds(2)); }
            if (_viewModelAut.farb2 != 1)
            {
                GPop.Visibility = Visibility.Visible;
                popup1.IsOpen = true;
            }
        }

        private void BtSave_Click(object sender, RoutedEventArgs e)
        {
            _viewModelAut.Sellos();
            while (!(_viewModelAut.binicia)) { continue; }
            _viewModelAut.LimpiaSello();
            while (!(_viewModelAut.binicia)) { continue; }
            popup1.IsOpen = false;
            GPop.Visibility = Visibility.Collapsed;
        }

        private void BtSaveD_Click(object sender, RoutedEventArgs e)
        {
            _viewModelAut.Danios();
            while (!(_viewModelAut.binicia)) { continue; }
            _viewModelAut.LimpiaDanio();
            while (!(_viewModelAut.binicia)) { continue; }
            popup2.IsOpen = false;
            GPopD.Visibility = Visibility.Collapsed;
        }

        private void Danios1_Click(object sender, RoutedEventArgs e)
        {
            GPopD.Visibility = Visibility.Visible;
            popup2.IsOpen = true;
            _viewModelAut.num = 1;
            _viewModelAut.DaniosUp();
            while (!(_viewModelAut.binicia)) { continue; }
        }

        private void Danios2_Click(object sender, RoutedEventArgs e)
        {
            GPopD.Visibility = Visibility.Visible;
            popup2.IsOpen = true;
            _viewModelAut.num = 2;
            _viewModelAut.DaniosUp();
            while (!(_viewModelAut.binicia)) { continue; }
        }

        private void Danios3_Click(object sender, RoutedEventArgs e)
        {
            GPopD.Visibility = Visibility.Visible;
            popup2.IsOpen = true;
            _viewModelAut.num = 3;
            _viewModelAut.DaniosUp();
            while (!(_viewModelAut.binicia)) { continue; }
        }

        private void Danios4_Click(object sender, RoutedEventArgs e)
        {
            GPopD.Visibility = Visibility.Visible;
            popup2.IsOpen = true;
            _viewModelAut.num = 4;
            _viewModelAut.DaniosUp();
            while (!(_viewModelAut.binicia)) { continue; }
        }

        private void DEPOSITO_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

        private void PASE1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            _viewModelAut.LlenaDeposito();
        }
    }
}

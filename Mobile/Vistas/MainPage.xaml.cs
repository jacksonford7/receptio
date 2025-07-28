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
    public sealed partial class MainPage : Page
    {
        private Main2PageViewModel _viewModelAut;
        private bool binicia=false;
        public MainPage()
        {
            if (!binicia)
            {
                this.InitializeComponent();
                _viewModelAut = new Main2PageViewModel(this);
            }
            //binicia = true;
            //ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            //Dictionary<string, List<string>> twoLists = new Dictionary<string, List<string>>();
            //twoLists.Add("2 List", new List<string>(new string[] { "2.1", "2.2", "2.3", "2.4" }));
            //twoLists.Add("3 List", new List<string>(new string[] { "3.1", "3.2", "3.3", "3.4" }));
            //twoLists.Add("4 List", new List<string>(new string[] { "4.1", "4.2", "4.3", "4.4" }));
            //f_List.ItemsSource = twoLists;

            List<string> twoLists = new List<string>();
            twoLists.Add("MAJOR");
            twoLists.Add("MINOR");
            f_List.ItemsSource = twoLists;

            //Dictionary<string, string> twoLists = new Dictionary<string, string>();
            //twoLists.Add("M", "MAJOR");
            //twoLists.Add("N", "MINOR");
            //f_List.ItemsSource = twoLists;
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

        private void Sellos1_Click(object sender, RoutedEventArgs e)
        {
            GPop.Visibility = Visibility.Visible;
            popup1.IsOpen = true;
            _viewModelAut.num = 1;
            _viewModelAut.SellosUp();
            while (!(_viewModelAut.binicia)) { continue; }
        }

        private void Sellos2_Click(object sender, RoutedEventArgs e)
        {
            GPop.Visibility = Visibility.Visible;
            popup1.IsOpen = true;
            _viewModelAut.num = 2;
            _viewModelAut.SellosUp();
            while (!(_viewModelAut.binicia)) { continue; }
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
    }


    

    //private static SqlConnection conexionN4()
    //{
    //    return new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["n4catalog"].ConnectionString);
    //}
    //public static DataTable GetBookingsZAL(string bookin, string linea)
    //{
    //    DataTable d = null;
    //    using (var c = conexionN4())
    //    {
    //        var coman = c.CreateCommand();
    //        coman.CommandType = CommandType.StoredProcedure;
    //        coman.CommandText = "n4_sp_get_booking_ZAL";
    //        coman.Parameters.AddWithValue("@booking", bookin);
    //        coman.Parameters.AddWithValue("@fkind", "MTY");
    //        coman.Parameters.AddWithValue("@linea", linea);
    //        //try
    //        //{
    //        c.Open();
    //        //d.Load(coman.ExecuteReader(CommandBehavior.CloseConnection));
    //        d = coman.ExecuteReader(CommandBehavior.CloseConnection);
    //        //}
    //        //catch (/*SqlException ex*/)
    //        //{
    //        //csl_log.log_csl.save_log<SqlException>(ex, "turno", "n4_sp_get_booking_ZAL", DateTime.Now.ToString(), "GetBookingsZAL_");
    //        //}
    //        //finally
    //        //{
    //        if (c.State == ConnectionState.Open)
    //        {
    //            c.Close();
    //        }
    //        c.Dispose();
    //        //}
    //    }
    //    return d;
    //}
}

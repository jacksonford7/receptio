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
    public sealed partial class DanioPage : Page
    {
        private DanioPageViewModel _viewModelDan;
        
        public DanioPage()
        {
            InitializeComponent();
            _viewModelDan = new DanioPageViewModel(this);
            //ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            List<string> twoLists = new List<string>();
            twoLists.Add("MAJOR");
            twoLists.Add("MINOR");
            f_List.ItemsSource = twoLists;
        }

        private void Click_Validar(object sender, RoutedEventArgs e)
        {
            _viewModelDan.Ingresar();
        }
    }
}

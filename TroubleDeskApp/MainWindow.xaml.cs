using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TroubleDeskApp
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public partial class MainWindow : Window//, //IContrato
    {
        private ServiceHost _servicio;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void Inicio(string error)
        {
            TxtAnuncio.Text = error;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _servicio = new ServiceHost(this);
            _servicio.Open();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _servicio.Close();
        }
    }
}

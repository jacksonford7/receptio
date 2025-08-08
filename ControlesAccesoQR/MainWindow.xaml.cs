using System.Windows;
using ControlesAccesoQR.ViewModels.ControlesAccesoQR;

namespace ControlesAccesoQR
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(MainFrame);
        }

        private void AbrirRfIdTest_Click(object sender, RoutedEventArgs e)
        {
            var ventana = new RFIDTestWindow();
            ventana.Show();
        }
    }
}

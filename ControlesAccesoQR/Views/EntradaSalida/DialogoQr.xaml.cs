using System.Windows;

namespace ControlesAccesoQR.Views.EntradaSalida
{
    public partial class DialogoQr : Window
    {
        public string NumeroPase => QrTextBox.Text;

        public DialogoQr()
        {
            InitializeComponent();
        }

        private void Aceptar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}

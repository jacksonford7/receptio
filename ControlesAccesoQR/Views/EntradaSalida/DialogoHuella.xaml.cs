using System.Windows;

namespace ControlesAccesoQR.Views.EntradaSalida
{
    public partial class DialogoHuella : Window
    {
        public DialogoHuella()
        {
            InitializeComponent();
        }

        private void Aceptar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}

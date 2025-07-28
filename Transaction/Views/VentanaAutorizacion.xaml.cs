using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Transaction.ViewModels;

namespace Transaction.Views
{
    public partial class VentanaAutorizacion : Window
    {
        public VentanaAutorizacion()
        {
            InitializeComponent();
        }

        private void PasswordBoxPasswordChanged(object sender, RoutedEventArgs e)
        {
            TxtContrasena.Text = PswContrasena.Password;
            var expresion = TxtContrasena.GetBindingExpression(TextBox.TextProperty);
            if (expresion != null)
                expresion.UpdateSource();
        }

        private void PasswordKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ((VentanaAutorizacionViewModel)(DataContext)).ComandoContinuar.Execute(null);
        }

        private void Cancelar(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}

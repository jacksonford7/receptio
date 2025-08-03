using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ControlesAccesoQR.UserControls
{
    public partial class TecladoNumerico : UserControl
    {
        public TecladoNumerico()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(TecladoNumerico),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty ComandoOkProperty =
            DependencyProperty.Register(nameof(ComandoOk), typeof(ICommand), typeof(TecladoNumerico));

        public ICommand ComandoOk
        {
            get => (ICommand)GetValue(ComandoOkProperty);
            set => SetValue(ComandoOkProperty, value);
        }

        private void Numero_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                Text += btn.Content?.ToString();
                InputTextBox.Focus();
            }
        }

        private void Borrar_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Text))
                Text = Text.Substring(0, Text.Length - 1);
            InputTextBox.Focus();
        }

        private void Limpiar_Click(object sender, RoutedEventArgs e)
        {
            Text = string.Empty;
            InputTextBox.Focus();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (ComandoOk?.CanExecute(null) == true)
                ComandoOk.Execute(null);
            InputTextBox.Focus();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            InputTextBox.Focus();
        }
    }
}

using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using TransactionEmpty.ViewModels;

namespace TransactionEmpty.Views
{
    public partial class VentanaPrincipal : Window
    {
        public VentanaPrincipal()
        {
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            Hide();
            Thread hiloSplash = new Thread(new ThreadStart(() =>
            {
                var splash = new Splash();
                splash.Show();
                Dispatcher.Run();
            }));
            hiloSplash.SetApartmentState(ApartmentState.STA);
            hiloSplash.IsBackground = false;
            hiloSplash.Start();
            DataContext = new VentanaPrincipalViewModel(frmContenedor);
            Show();
            hiloSplash.Abort();
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            if (DataContext != null)
                ((VentanaPrincipalViewModel)DataContext).Dispose();
        }
    }
}

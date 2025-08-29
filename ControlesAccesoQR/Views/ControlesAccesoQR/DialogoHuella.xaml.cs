using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ControlesAccesoQR.ViewModels.ControlesAccesoQR;

namespace ControlesAccesoQR.Views.ControlesAccesoQR
{
    public partial class DialogoHuella : Page
    {
        public DialogoHuella()
        {
            InitializeComponent();
        }

        private async void Aceptar_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is HuellaViewModel vm)
            {
                await vm.OnValidacionCompletadaAsync();
            }

            NavigationService?.GoBack();
        }
    }
}

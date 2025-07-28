using Console.ViewModels;
using Windows.UI.Xaml.Controls;

namespace Console.Vistas
{
    public sealed partial class VentanaAutorizacion : ContentDialog
    {
        internal VentanaAutorizacionViewModel ViewModel;

        public VentanaAutorizacion()
        {
            InitializeComponent();
        }
    }
}

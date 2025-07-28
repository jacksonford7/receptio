using Console.ViewModels;
using Windows.UI.Xaml.Controls;

namespace Console.Vistas
{
    public sealed partial class VentanaTransaccionManual : ContentDialog
    {
        internal VentanaTransaccionManualViewModel ViewModel;

        public VentanaTransaccionManual()
        {
            InitializeComponent();
        }
    }
}

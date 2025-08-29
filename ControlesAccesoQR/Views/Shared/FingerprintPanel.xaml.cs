using System.Windows;
using System.Windows.Controls;
using ControlesAccesoQR.ViewModels;

namespace ControlesAccesoQR.Views.Shared
{
    public partial class FingerprintPanel : UserControl
    {
        public FingerprintPanel()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                if (DataContext is FingerprintPanelViewModel vm)
                    vm.StartCaptureCommand.Execute(null);
            };
            IsVisibleChanged += (s, e) =>
            {
                if (IsVisible && DataContext is FingerprintPanelViewModel vm)
                    vm.StartCaptureCommand.Execute(null);
            };
        }
    }
}

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Transaction.ViewModels;

namespace Transaction.Views
{
    public partial class PaginaInicio : Page
    {
        public PaginaInicio()
        {
            InitializeComponent();
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            TxtPanel.Focus();
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            TxtPanel.Focus();
        }

        private void TxtPanelKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9 || e.Key == Key.Enter)
            {
                e.Handled = false;
                if (e.Key != Key.Enter)
                    return;
                ((PaginaInicioViewModel)DataContext).Numero = ((TextBox)sender).Text;
                ((PaginaInicioViewModel)DataContext).Procesar();
            }
            else
                e.Handled = true;
        }

        private void TxtPanelPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (!e.DataObject.GetDataPresent(typeof(string))) return;
            var pastingText = (string)e.DataObject.GetData(typeof(string));
            try
            {
                var numero = Convert.ToInt64(pastingText);
            }
            catch (Exception)
            {
                e.CancelCommand();
            }
        }
    }
}

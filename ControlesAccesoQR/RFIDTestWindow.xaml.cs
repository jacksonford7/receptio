using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using RECEPTIO.CapaPresentacion.UI.Interfaces.RFID;
using Spring.Context.Support;

namespace ControlesAccesoQR
{
    public partial class RFIDTestWindow : Window
    {
        private IAntena _antena;
        private DispatcherTimer _timer;
        private ObservableCollection<string> _tags = new ObservableCollection<string>();

        public RFIDTestWindow()
        {
            InitializeComponent();
            TagsListView.ItemsSource = _tags;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var ctx = new XmlApplicationContext("~/Springs/SpringAntena.xml");
                _antena = (IAntena)ctx["AdministradorAntena"];
                if (!_antena.ConectarAntena())
                {
                    MessageBox.Show("No se pudo conectar a la antena RFID");
                    Close();
                    return;
                }
                _antena.IniciarLectura();
                _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
                _timer.Tick += (s, args) =>
                {
                    try
                    {
                        var tags = _antena.ObtenerTagsLeidos();
                        foreach (var tag in tags)
                        {
                            if (!_tags.Contains(tag))
                                _tags.Add(tag);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                };
                _timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                _timer?.Stop();
                _antena?.TerminarLectura();
                _antena?.DesconectarAntena();
                _antena?.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

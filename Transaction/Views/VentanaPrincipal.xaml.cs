using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Transaction.ServicioTransaction;
using Transaction.ViewModels;

namespace Transaction.Views
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

        private void ImageMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext != null && ((VentanaPrincipalViewModel)DataContext).PuedoIrHome)
                ((VentanaPrincipalViewModel)DataContext).IrHomeQuiosco();
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            if(DataContext != null)
                ((VentanaPrincipalViewModel)DataContext).Dispose();
        }

        private void TextBlockMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext != null && ((VentanaPrincipalViewModel)DataContext).VentanaAutorizacionDisponible && !((VentanaPrincipalViewModel)DataContext).Quiosco.IS_IN)
            {
                var ventana = new VentanaAutorizacion();
                ventana.DataContext = new VentanaAutorizacionViewModel(ventana, ((VentanaPrincipalViewModel)DataContext).Servicio, ((VentanaPrincipalViewModel)DataContext).TagReal);
                var resultado = ventana.ShowDialog();
                if (resultado.HasValue && resultado.Value)
                {
                    PaseManualSalida(((VentanaAutorizacionViewModel)ventana.DataContext).Usuario);
                    ((VentanaPrincipalViewModel)DataContext).SetearEstado(new PaginaSalidaN4ViewModel());
                    ((VentanaPrincipalViewModel)DataContext).VentanaAutorizacionDisponible = false;
                }
            }
        }

        private void PaseManualSalida(string usuario)
        {
            var transaccion = new KIOSK_TRANSACTION
            {
                PRE_GATE_ID = ((VentanaPrincipalViewModel)DataContext).DatosPreGateSalida.PreGate.PRE_GATE_ID,
                TRANSACTION_ID = Convert.ToInt32(((VentanaPrincipalViewModel)DataContext).DatosPreGateSalida.IdTransaccion),
                PROCESSES = new List<PROCESS> { new PROCESS
                {
                    IS_OK = true,
                    STEP = "RFID_MANUAL",
                    RESPONSE = $"Autorizado por : {usuario}",
                    MESSAGE_ID = 25
                } },
                KIOSK = ((VentanaPrincipalViewModel)DataContext).Quiosco
            };
            ((VentanaPrincipalViewModel)DataContext).Servicio.RegistrarProceso(transaccion);
        }
    }
}

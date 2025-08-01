using ControlesAccesoQR.accesoDatos;
using RECEPTIO.CapaPresentacion.UI.Interfaces.RFID;
using Spring.Context.Support;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ControlesAccesoQR.ViewModels.ControlesAccesoQR
{
    /// <summary>
    /// Paso del flujo que valida el tag RFID del cami\u00f3n.
    /// </summary>
    internal class PaginaRfidViewModel : EstadoProceso
    {
        private readonly string _placa;
        private readonly PasePuertaDataAccess _dataAccess = new PasePuertaDataAccess();
        private readonly IAntena _antena;
        private MainWindowViewModel _viewModel;
        private bool _rfidValido;

        public bool RfidValido
        {
            get => _rfidValido;
            private set { _rfidValido = value; OnPropertyChanged(nameof(RfidValido)); }
        }

        public PaginaRfidViewModel(string placa)
        {
            _placa = placa;
            Titulo = "LECTURA RFID";
            EsVisibleBoton = Visibility.Collapsed;
            var ctx = new XmlApplicationContext("~/Springs/SpringAntena.xml");
            _antena = (IAntena)ctx["AdministradorAntena"];
        }

        internal override void EstablecerControles(MainWindowViewModel viewModel)
        {
            _viewModel = viewModel;
            Worker.DoWork += IniciarHilo;
            Navegar();
            Procesar();
        }

        private void Navegar()
        {
            // No se define una vista espec\u00edfica, se podr\u00eda navegar si existiera.
        }

        private void Procesar()
        {
            Worker.RunWorkerAsync();
        }

        protected virtual void IniciarHilo(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Worker.ReportProgress(0);
            var tagEsperado = _dataAccess.ObtenerTagRfidPorPlaca(_placa);
            if (string.IsNullOrWhiteSpace(tagEsperado))
            {
                Mensaje = "No existe tag en BD";
                return;
            }
            _antena.IniciarLectura();
            System.Threading.Thread.Sleep(1000);
            List<string> tags = _antena.TerminarLectura();
            if (!tags.Any())
            {
                Mensaje = "No se ley\u00f3 ning\u00fan tag";
            }
            else if (tags.Contains(tagEsperado))
            {
                Mensaje = "Tag le\u00eddo v\u00e1lido";
                RfidValido = true;
            }
            else
            {
                Mensaje = "Tag le\u00eddo no coincide";
            }
            _antena.DesconectarAntena();
        }

        protected override void CambiarEstado()
        {
            // Se podr\u00eda avanzar a otro paso si existiera.
        }
    }
}

using RECEPTIO.CapaPresentacion.UI.Interfaces.RFID;
using System;
using System.Collections.Generic;
using GMA_Chafon_RFID;
using System.Configuration;
using System.Linq;
using System.Windows.Forms;

namespace RECEPTIO.CapaPresentacion.UI.RFID_CHAFON
{
    public class Antena : IAntena, IDisposable
    {
        private OC_Chafon_RFID _controlRfid;
        private bool _disposed;

        public bool ConectarAntena()
        {
            _controlRfid = new OC_Chafon_RFID();
            var resultado = _controlRfid.P_AbrePuerto(ConfigurationManager.AppSettings["PuertoRFID"].ToString());
            return resultado.Contains("OK");
        }

        public void IniciarLectura()
        {
            _controlRfid.P_IniciaLecturaContinua(2, 0, "06", "00", 0, 100, 5, true);
        }

        public List<string> TerminarLectura()
        {
            var lista = _controlRfid.P_ListaRFID.Select(l => l.CodigoOC).ToList();
            var pp =_controlRfid.P_DetieneLecturaContinua("1");
            var sas = _controlRfid.P_LimpiaLista(1,1);
            _controlRfid.P_ExportaListaRFID();
            return lista;
        }

        public List<string> ObtenerTagsLeidos()
        {
            return _controlRfid.P_ListaRFID.Select(l => l.CodigoOC).ToList();
        }

        public void DesconectarAntena()
        {
            _controlRfid.P_CierraPuerto(Convert.ToInt32(ConfigurationManager.AppSettings["PuertoRFID"]));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
                _controlRfid.Dispose();
            _disposed = true;
        }
    }
}

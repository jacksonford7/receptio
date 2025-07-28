using OC_FRAMEWORK_CONTECON;
using RECEPTIO.CapaPresentacion.UI.Interfaces.RFID;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace RECEPTIO.CapaPresentacion.UI.RFID
{
    public class Antena : IAntena, IDisposable
    {
        private OC_RFID_UV_RS232_ _controlRfid;
        private bool _disposed;
        private List<string> _tags;

        public bool ConectarAntena()
        {
            var resultadoConexion = "";
            _controlRfid = new OC_RFID_UV_RS232_();
            _controlRfid.OC_ConectaRFID(ConfigurationManager.AppSettings["PuertoRFID"],
                                        Convert.ToInt32(ConfigurationManager.AppSettings["VelocidadRFID"]),
                                        ref resultadoConexion,
                                        Convert.ToInt32(ConfigurationManager.AppSettings["TamanoBufferRFID"]));
            return resultadoConexion.Contains("OK");
        }

        public void IniciarLectura()
        {
            _tags = new List<string>();
            _controlRfid.EPCDatos += EventoRfid;
        }

        public List<string> TerminarLectura()
        {
            _controlRfid.EPCDatos -= EventoRfid;
            return _tags;
        }

        public List<string> ObtenerTagsLeidos()
        {
            return _tags;
        }

        public void DesconectarAntena()
        {
            _controlRfid.OC_DesconectaRFID(ConfigurationManager.AppSettings["PuertoRFID"]);
        }

        private void EventoRfid(string HexCode, string OCCode)
        {
            if(!_tags.Contains(OCCode))
                _tags.Add(OCCode);
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

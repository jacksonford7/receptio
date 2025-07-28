using IAHAL;
using RECEPTIO.CapaPresentacion.UI.Interfaces.UsbRelay;
using System;
using System.Collections;
using System.Configuration;
using System.Linq;

namespace RECEPTIO.CapaPresentacion.UI.UsbRelay
{
    public class Barrera : IBarrera, IDisposable
    {
        private BitArray _arreglosPuertos = new BitArray(16);
        private bool _disposed;
        private IADevices _puertos;
        private IADevice _puertoIa;

        public bool Conectar()
        {
            _puertos = new IADevices();
            var variableError = _puertos.DetectUSBDevices();
            if (variableError != IAError.IA_OK)
                return false;
            foreach (var puerto in _puertos.Cast<IADevice>().Where(puerto => (puerto.HasDigitalInput || puerto.HasDigitalOutput) && puerto.Responsive))
            {
                _puertoIa = puerto;
                break;
            }
            return _puertoIa != null;
        }

        public void LevantarBarrera()
        {
            _arreglosPuertos[Convert.ToInt32(ConfigurationManager.AppSettings["PuertoBarrera"])] = true;
            _puertoIa.WriteDO(_arreglosPuertos);
        }

        public void SoloLevantarBarrera()
        {
            _arreglosPuertos[Convert.ToInt32(ConfigurationManager.AppSettings["PuertoBarrera"])] = true;
            _puertoIa.WriteDO(_arreglosPuertos);
        }

        public void BajarBarrera()
        {
            _arreglosPuertos[Convert.ToInt32(ConfigurationManager.AppSettings["PuertoBarrera"])] = false;
            _puertoIa.WriteDO(_arreglosPuertos);
        }

        public BitArray ObtenerInputs()
        {
            _puertoIa.ReadDI(out _arreglosPuertos);
            return _arreglosPuertos;
        }

        public BitArray ObtenerOutputs()
        {
            _puertoIa.ReadDO(out _arreglosPuertos);
            return _arreglosPuertos;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
                _puertos.Dispose();
            _disposed = true;
        }
    }

    public class BarreraOc : IBarrera
    {
        private OCusUsbRelay.clsUsbRelayOC _usbRelay;

        public bool Conectar()
        {
            _usbRelay = new OCusUsbRelay.clsUsbRelayOC { generaLogOutputs = true };
            var resultado = _usbRelay.f_conecta_usb_relay();
            return resultado != null && resultado.conectado;
        }

        public void LevantarBarrera()
        {
            _usbRelay.f_activaPulsoOutput(Convert.ToInt32(ConfigurationManager.AppSettings["PuertoBarrera"]), 1);
        }

        public void BajarBarrera()
        {
        }

        public BitArray ObtenerInputs()
        {
            var resultado = new BitArray(4);
            foreach (var item in _usbRelay.misInputs)
                resultado[item.inputs - 1] = item.status;
            return resultado;
        }

        public BitArray ObtenerOutputs()
        {
            var resultado = new BitArray(4);
            foreach (var item in _usbRelay.misOuts)
                resultado[item.inputs - 1] = item.status;
            return resultado;
        }

        public bool DesconectarUsbRelay()
        {
            return _usbRelay.f_desconecta_usb_relay();
        }

        public void SoloLevantarBarrera()
        {
        }

        public void Dispose()
        {
        }
    }
}

using System.Collections;

namespace RECEPTIO.CapaPresentacion.UI.Interfaces.UsbRelay
{
    public interface IBarrera
    {
        bool Conectar();
        void LevantarBarrera();
        void SoloLevantarBarrera();
        void BajarBarrera();
        BitArray ObtenerInputs();
        BitArray ObtenerOutputs();
        void Dispose();
    }
}

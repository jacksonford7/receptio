using System.Threading.Tasks;
using RECEPTIO.CapaPresentacion.UI.Biometrico;
using RECEPTIO.CapaPresentacion.UI.Interfaces.Biometrico;

namespace ControlesAccesoQR.Servicios
{
    public class HuellaService : IHuellaService
    {
        private readonly IBiometrico _biometrico;

        public HuellaService()
        {
            _biometrico = new Biometrico();
        }

        public Task<bool> CapturarYValidarAsync(string choferId)
        {
            if (string.IsNullOrWhiteSpace(choferId))
                return Task.FromResult(false);

            if (DevBypass.IsDevKiosk)
                return Task.FromResult(true);

            return Task.Run(() =>
            {
                var resultado = _biometrico.ProcesoHuella(choferId);
                return !string.IsNullOrEmpty(resultado) && resultado.Contains(choferId);
            });
        }
    }
}

using RECEPTIO.CapaPresentacion.UI.Interfaces.Biometrico;

namespace RECEPTIO.CapaPresentacion.UI.Biometrico
{
    public class Biometrico : IBiometrico
    {
        public string ProcesoHuella(string identificador)
        {
            var componenteHuella = new Z_OC_2009_C_G.Z_GMA();
            var respuesta = componenteHuella.Only_VHuella_R(identificador, 0);
            if (respuesta.Length > 2)
            {
                if (respuesta.Substring(0, 2) == "E:" || respuesta.Substring(0, 2) == "A:")
                    respuesta = respuesta.Substring(2, respuesta.Length - 2);
            }
            return respuesta?? "";
        }
    }
}

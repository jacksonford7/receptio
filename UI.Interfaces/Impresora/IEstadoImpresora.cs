using System;
using System.Collections.Generic;

namespace RECEPTIO.CapaPresentacion.UI.Interfaces.Impresora
{
    public interface IEstadoImpresora
    {
        Tuple<List<string>, List<string>> VerEstado();
    }
}

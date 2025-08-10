using System;

namespace ControlesAccesoQR
{
    internal static class DevBypass
    {
        internal static bool IsDevKiosk =>
            Environment.MachineName.Equals("CGDE041", StringComparison.OrdinalIgnoreCase);
    }
}

using System.Windows;
using Transaction.ServicioTransaction;

namespace Transaction.Ayudas
{
    internal class ActualizadorAutomatico
    {
        private readonly string _nombreAplicacion;
        private readonly string _rutaArchivos;
        private readonly string _ejecutable;
        private readonly string _tipoVersion;
        private string _versionActual;
        private string _rutaFuente;

        internal ActualizadorAutomatico(string nombreAplicacion, string rutaArchivos, string ejecutable, string tipoVersion)
        {
            _nombreAplicacion = nombreAplicacion;
            _rutaArchivos = rutaArchivos;
            _ejecutable = ejecutable;
            _tipoVersion = tipoVersion;
        }

        internal bool EsActualVersion(string versionAplicacion)
        {
            var aplicacion = ObtenerAplicacion();
            if (aplicacion == null || string.IsNullOrWhiteSpace(aplicacion.VERSION) || string.IsNullOrWhiteSpace(aplicacion.PATH))
            {
                MessageBox.Show("No existe información de versión de la aplicación para actualizaciones automáticas.", _nombreAplicacion, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return true;
            }
            if (versionAplicacion == aplicacion.VERSION)
                return true;
            _versionActual = aplicacion.VERSION;
            _rutaFuente = aplicacion.PATH;
            return false;
        }

        private APPLICATION ObtenerAplicacion()
        {
            using (var servicio = new ServicioTransactionClient())
            {
                return servicio.ObtenerAplicacion(2);
            }
        }

        public void ActualizarVersion()
        {
            System.Diagnostics.Process.Start(_rutaArchivos.Replace($"{_ejecutable}.exe", "Transferencia.exe"), $"{_versionActual}{_tipoVersion} {_rutaFuente} {_ejecutable} {_nombreAplicacion}");
        }
    }
}

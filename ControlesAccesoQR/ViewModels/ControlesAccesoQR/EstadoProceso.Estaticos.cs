namespace ControlesAccesoQR.ViewModels.ControlesAccesoQR
{
    /// <summary>
    /// Contiene instancias reutilizables de los estados del proceso.
    /// </summary>
    internal static class EstadosProcesoInstancias
    {
        internal static readonly EstadoProceso EnEspera = new EstadoEnEspera();
        internal static readonly EstadoProceso IngresoRegistrado = new EstadoIngresoRegistrado();
        internal static readonly EstadoProceso SalidaRegistrada = new EstadoSalidaRegistrada();
    }
}

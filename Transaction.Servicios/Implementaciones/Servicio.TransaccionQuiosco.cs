using RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Contratos;
using RECEPTIO.CapaAplicacion.Transaction.Aplicacion.Interfaces;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using Spring.Context.Support;
using System;
using System.ServiceModel;

namespace RECEPTIO.CapaServiciosDistribuidos.Transaction.Servicios
{
    public partial class ServicioTransaction
    {
        public Respuesta RegistrarProceso(KIOSK_TRANSACTION transaccion)
        {
            ITransaccionQuiosco administradorTransaccionQuiosco = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringTransaccionQuiosco.xml");
                administradorTransaccionQuiosco = (ITransaccionQuiosco)ctx["AdministradorTransaccionQuiosco"];
                return administradorTransaccionQuiosco.RegistrarProceso(transaccion);
            }
            catch (FaultException)
            {
                throw;
            }
            catch (Exception ex)
            {
                LoguearError($"Mensaje : {ex.Message}///Excepción Interna : {ex.InnerException}///Pila de Seguimiento : {ex.StackTrace}///Fuente : {ex.Source}///Link : {ex.HelpLink}");
                throw;
            }
            finally
            {
                if (administradorTransaccionQuiosco != null)
                    administradorTransaccionQuiosco.LiberarRecursos();
            }
        }
    }
}
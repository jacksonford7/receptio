using RECEPTIO.CapaAplicacion.Mobile.Aplicacion.Contratos;
using RECEPTIO.CapaAplicacion.Mobile.Aplicacion.Interfaces;
using Spring.Context.Support;
using System;
using System.ServiceModel;

namespace RECEPTIO.CapaServiciosDistribuidos.Mobile.Servicios
{
    public partial class ServicioMobile
    {
        public RespuestaProceso EjecutarProcesosDeliveryImportFull(DatosN4 datos)
        {
            IProcesoN4 administradorProcesoN4 = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringProcesoN4.xml");
                administradorProcesoN4 = (IProcesoN4)ctx["AdministradorProcesoN4"];
                return administradorProcesoN4.EjecutarProcesosDeliveryImportFull(datos);
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
                if (administradorProcesoN4 != null)
                    administradorProcesoN4.LiberarRecursos();
            }
        }

        public RespuestaProceso EjecutarProcesosDeliveryImportBrBkCfs(DatosDeliveryImportBrbkCfs datos)
        {
            IProcesoN4 administradorProcesoN4 = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringProcesoN4.xml");
                administradorProcesoN4 = (IProcesoN4)ctx["AdministradorProcesoN4"];
                return administradorProcesoN4.EjecutarProcesosDeliveryImportBrBkCfs(datos);
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
                if (administradorProcesoN4 != null)
                    administradorProcesoN4.LiberarRecursos();
            }
        }

        public RespuestaProceso EjecutarProcesosReceiveExport(DatosReceiveExport datos)
        {
            IProcesoN4 administradorProcesoN4 = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringProcesoN4.xml");
                administradorProcesoN4 = (IProcesoN4)ctx["AdministradorProcesoN4"];
                return administradorProcesoN4.EjecutarProcesosReceiveExport(datos);
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
                if (administradorProcesoN4 != null)
                    administradorProcesoN4.LiberarRecursos();
            }
        }

        public RespuestaProceso EjecutarProcesosReceiveExportBrBk(DatosReceiveExportBrBk datos)
        {
            IProcesoN4 administradorProcesoN4 = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringProcesoN4.xml");
                administradorProcesoN4 = (IProcesoN4)ctx["AdministradorProcesoN4"];
                return administradorProcesoN4.EjecutarProcesosReceiveExportBrBk(datos);
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
                if (administradorProcesoN4 != null)
                    administradorProcesoN4.LiberarRecursos();
            }
        }

        public RespuestaProceso EjecutarProcesosReceiveExportBanano(DatosReceiveExportBanano datos)
        {
            IProcesoN4 administradorProcesoN4 = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringProcesoN4.xml");
                administradorProcesoN4 = (IProcesoN4)ctx["AdministradorProcesoN4"];
                return administradorProcesoN4.EjecutarProcesosReceiveExportBanano(datos);
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
                if (administradorProcesoN4 != null)
                    administradorProcesoN4.LiberarRecursos();
            }
        }

        public RespuestaProceso LiberarHold(DatosN4 datos)
        {
            IProcesoN4 administradorProcesoN4 = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringProcesoN4.xml");
                administradorProcesoN4 = (IProcesoN4)ctx["AdministradorProcesoN4"];
                return administradorProcesoN4.LiberarHold(datos);
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
                if (administradorProcesoN4 != null)
                    administradorProcesoN4.LiberarRecursos();
            }
        }

        public RespuestaProceso CambiarHold(DatosN4 datos)
        {
            IProcesoN4 administradorProcesoN4 = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringProcesoN4.xml");
                administradorProcesoN4 = (IProcesoN4)ctx["AdministradorProcesoN4"];
                return administradorProcesoN4.CambiarHold(datos);
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
                if (administradorProcesoN4 != null)
                    administradorProcesoN4.LiberarRecursos();
            }
        }

        public RespuestaProceso EjecutarProcesosDeliveryImportMTYBooking(DatosN4 datos)
        {
            IProcesoN4 administradorProcesoN4 = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringProcesoN4.xml");
                administradorProcesoN4 = (IProcesoN4)ctx["AdministradorProcesoN4"];
                return administradorProcesoN4.EjecutarProcesosDeliveryImportMTYBooking(datos);
            }
            catch (FaultException)
            {
                throw;
            }
            catch (Exception ex)
            {
                LoguearError($"Mensaje :  {ex.Message}///Excepción Interna : {ex.InnerException}///Pila de Seguimiento : {ex.StackTrace}///Fuente : RECEPTIO.CapaServiciosDistribuidos.Mobile.Servicios - Servicio.ProcesoN4 - EjecutarProcesosDeliveryImportMTYBooking - {ex.Source}///Link : {ex.HelpLink}");
                throw;
            }
            finally
            {
                if (administradorProcesoN4 != null)
                    administradorProcesoN4.LiberarRecursos();
            }
        }

        public RespuestaProceso EjecutarProcesosDeliveryImportP2D(DatosDeliveryImportP2D datos)
        {
            IProcesoN4 administradorProcesoN4 = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringProcesoN4.xml");
                administradorProcesoN4 = (IProcesoN4)ctx["AdministradorProcesoN4"];
                return administradorProcesoN4.EjecutarProcesosDeliveryImportP2D(datos);
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
                if (administradorProcesoN4 != null)
                    administradorProcesoN4.LiberarRecursos();
            }
        }
    }
}
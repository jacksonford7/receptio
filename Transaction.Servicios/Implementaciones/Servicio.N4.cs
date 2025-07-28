using RECEPTIO.CapaAplicacion.Transaction.Aplicacion.Contratos;
using RECEPTIO.CapaAplicacion.Transaction.Aplicacion.Interfaces;
using Spring.Context.Support;
using System;
using System.ServiceModel;

namespace RECEPTIO.CapaServiciosDistribuidos.Transaction.Servicios
{
    public partial class ServicioTransaction
    {
        public DatosN4 EjecutarProcesosEntrada(DatosEntradaN4 datos)
        {
            IProcesosN4 administradorDeliveryImportN4 = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringN4.xml");
                administradorDeliveryImportN4 = (IProcesosN4)ctx["AdministradorN4"];
                return administradorDeliveryImportN4.EjecutarProcesosEntrada(datos);
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
        }

        public DatosN4 EjecutarProcesosSalida(DatosPreGateSalida datos)
        {
            IProcesosN4 administradorDeliveryImportN4 = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringN4.xml");
                administradorDeliveryImportN4 = (IProcesosN4)ctx["AdministradorN4"];
                return administradorDeliveryImportN4.EjecutarProcesosSalida(datos);
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
        }
    }
}
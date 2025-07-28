using RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Contratos;
using RECEPTIO.CapaAplicacion.TransactionEmpty.Aplicacion.Contratos;
using RECEPTIO.CapaAplicacion.TransactionEmpty.Aplicacion.Interfaces;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using Spring.Context.Support;
using System;
using System.ServiceModel;

namespace RECEPTIO.CapaServiciosDistribuidos.TransactionEmpty.Servicios
{
    public partial class ServicioTransactionEmpty
    {
        public RespuestaN4 Procesar(long id, KIOSK kiosco)
        {
            IGeneral administradorGeneral = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringGeneral.xml");
                administradorGeneral = (IGeneral)ctx["AdministradorGeneral"];
                return administradorGeneral.Procesar(id, kiosco);
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

        public Respuesta RegistrarProceso(KIOSK_TRANSACTION transaccion)
        {
            IGeneral administradorGeneral = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringGeneral.xml");
                administradorGeneral = (IGeneral)ctx["AdministradorGeneral"];
                return administradorGeneral.RegistrarProceso(transaccion);
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
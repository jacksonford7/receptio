using RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Contratos;
using RECEPTIO.CapaAplicacion.TransactionDepot.Aplicacion.Contratos;
using RECEPTIO.CapaAplicacion.TransactionDepot.Aplicacion.Interfaces;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using RECEPTIO.CapaServiciosDistribuidos.Nucleo.Servicios;
using Spring.Context.Support;
using System;
using System.ServiceModel;

namespace RECEPTIO.CapaServiciosDistribuidos.TransactionDepot.Servicios
{
    public partial class ServicioTransactionDepot
    {

        public RespuestaN4Depot AutentificacionWS(string usuario, long clave)
        {
            IGeneralDepot administradorGeneral = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringGeneral.xml");
                administradorGeneral = (IGeneralDepot)ctx["AdministradorGeneral"];
                return administradorGeneral.AutentificacionWS(usuario, clave);
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

        public RespuestaN4Depot GeneraVisita(long idTransaccion, string token ,string cedula, string placa, long turno)
        {
            IGeneralDepot administradorGeneral = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringGeneral.xml");
                administradorGeneral = (IGeneralDepot)ctx["AdministradorGeneral"];
                return administradorGeneral.GeneraVisita( idTransaccion, token, cedula, placa, turno);
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

        public RespuestaN4Depot Procesar(long idTransaccion,string token, long preGate, string placa, string contenedor, string booking)
        {
            IGeneralDepot administradorGeneral = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringGeneral.xml");
                administradorGeneral = (IGeneralDepot)ctx["AdministradorGeneral"];
                return administradorGeneral.Procesar(idTransaccion, token, preGate,placa,contenedor, booking);
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

        //public RespuestaN4Depot GeneraEventoFacturaN4(long idTransaccion,string token,  string contenedor)
        //{
        //    IGeneralDepot administradorGeneral = null;
        //    try
        //    {
        //        var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringGeneral.xml");
        //        administradorGeneral = (IGeneralDepot)ctx["AdministradorGeneral"];
        //        return administradorGeneral.GeneraEventoFacturaN4(idTransaccion, token, contenedor);
        //    }
        //    catch (FaultException)
        //    {
        //        throw;
        //    }
        //    catch (Exception ex)
        //    {
        //        LoguearError($"Mensaje : {ex.Message}///Excepción Interna : {ex.InnerException}///Pila de Seguimiento : {ex.StackTrace}///Fuente : {ex.Source}///Link : {ex.HelpLink}");
        //        throw;
        //    }
        //}

        public RespuestaN4Depot GeneraSalida(long idTransaccion,long PreGate, string token)
        {
            IGeneralDepot administradorGeneral = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringGeneral.xml");
                administradorGeneral = (IGeneralDepot)ctx["AdministradorGeneral"];
                return administradorGeneral.GeneraSalida(idTransaccion, PreGate, token);
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
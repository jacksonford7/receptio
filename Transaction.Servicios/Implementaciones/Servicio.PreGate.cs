using RECEPTIO.CapaAplicacion.Transaction.Aplicacion.Contratos;
using RECEPTIO.CapaAplicacion.Transaction.Aplicacion.Interfaces;
using Spring.Context.Support;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace RECEPTIO.CapaServiciosDistribuidos.Transaction.Servicios
{
    public partial class ServicioTransaction
    {
        public DatosPreGate ValidarPreGate(string cedula, short idQuiosco, Dictionary<short, bool> valoresSensores)
        {
            IPreGate administradorPreGate = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringPreGate.xml");
                administradorPreGate = (IPreGate)ctx["AdministradorPreGate"];
                return administradorPreGate.ValidarPreGate(cedula, idQuiosco, valoresSensores);
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
                if (administradorPreGate != null)
                    administradorPreGate.LiberarRecursos();
            }
        }

        public DatosPreGateSalida ValidarEntradaQuiosco(long idDetallePreGate, short idQuiosco, Dictionary<short, bool> valoresSensores)
        {
            IPreGate administradorPreGate = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringPreGate.xml");
                administradorPreGate = (IPreGate)ctx["AdministradorPreGate"];
                return administradorPreGate.ValidarEntradaQuiosco(idDetallePreGate, idQuiosco, valoresSensores);
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
                if (administradorPreGate != null)
                    administradorPreGate.LiberarRecursos();
            }
        }
    }
}
using RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Interfaces;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using Spring.Context.Support;
using System;
using System.ServiceModel;

namespace RECEPTIO.CapaServiciosDistribuidos.Transaction.Servicios
{
    public partial class ServicioTransaction
    {
        public int CrearError(ERROR error)
        {
            IError administradorError = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringError.xml");
                administradorError = (IError)ctx["AdministradorError"];
                return administradorError.CrearError(error);
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
                if (administradorError != null)
                    administradorError.LiberarRecursos();
            }
        }

        public int GrabarErrorTecnico(ERROR error)
        {
            return -1;
        }
    }
}
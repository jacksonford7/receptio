using RECEPTIO.CapaAplicacion.Transaction.Aplicacion.Interfaces;
using Spring.Context.Support;
using System;
using System.ServiceModel;

namespace RECEPTIO.CapaServiciosDistribuidos.Transaction.Servicios
{
    public partial class ServicioTransaction
    {
        public string ObtenerTag(string placa)
        {
            ITag administradorTag = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringTag.xml");
                administradorTag = (ITag)ctx["AdministradorTag"];
                return administradorTag.ObtenerTag(placa);
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
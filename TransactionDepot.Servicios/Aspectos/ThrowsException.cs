using RECEPTIO.CapaAplicacion.Nucleo.Aplicacion.Interfaces;
using RECEPTIO.CapaDominio.Nucleo.Entidades;
using Spring.Aop;
using Spring.Context.Support;
using System;
using System.ServiceModel;

namespace RECEPTIO.CapaServiciosDistribuidos.TransactionDepot.Servicios.Aspectos
{
    public class ThrowsException : IThrowsAdvice
    {
        public void AfterThrowing(Exception ex)
        {
            var ctx = new XmlApplicationContext("~/Implementaciones/Springs/SpringError2.xml");
            var administradorError = (IError)ctx["AdministradorError"];
            var error = new ERROR
            {
                DETAILS = $"Mensaje : {ex.Message}///Excepción Interna : {ex.InnerException}///Pila de Seguimiento : {ex.StackTrace}///Fuente : {ex.Source}///Link : {ex.HelpLink}",
                MESSAGE = ex.Message,
                APPLICATION_ID = 2
            };
            var id = administradorError.GrabarErrorTecnico(error);
            administradorError.LiberarRecursos();
            throw new FaultException($"Ha ocurrido un inconveniente.{Environment.NewLine}Reportelo con el código :{id}");
        }
    }
}
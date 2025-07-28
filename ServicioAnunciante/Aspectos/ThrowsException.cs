using Spring.Aop;
using System;

namespace RECEPTIO.CapaServicioDistribuidos.ServicioAnunciante.Aspectos
{
    public class ThrowsException : IThrowsAdvice
    {
        public void AfterThrowing(Exception ex)
        {
            throw ex;
        }
    }
}
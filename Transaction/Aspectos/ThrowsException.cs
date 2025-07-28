using Spring.Aop;
using System;

namespace Transaction.Aspectos
{
    public class ThrowsException : IThrowsAdvice
    {
        public void AfterThrowing(Exception ex)
        {
            throw ex;
        }
    }
}

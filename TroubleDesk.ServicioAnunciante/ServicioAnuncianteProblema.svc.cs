using RECEPTIO.CapaAplicacion.Console.Aplicacion.Interfaces;
using RECEPTIO.CapaPresentacion.TroubleDeskContrato;
using Spring.Context.Support;
using System;
using System.ServiceModel;

namespace RECEPTIO.CapaServicioDistribuidos.TroubleDesk.ServicioAnunciante
{
    public class ServicioAnuncianteProblema : IServicioAnuncianteProblema
    {
        public void AnunciarProblema(int idTransaccionQuiosco)
        {
            IProblema administradorProblema = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Springs/SpringProblema.xml");
                administradorProblema = (IProblema)ctx["AdministradorProblema"];
                var resultado = administradorProblema.RegistrarProblema(idTransaccionQuiosco);
                if (resultado.Item1)
                    EnviarMensajeProblema(resultado);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (administradorProblema != null)
                    administradorProblema.LiberarRecursos();
            }
        }

        private void EnviarMensajeProblema(Tuple<bool, string, string> resultado)
        {
            var binding = new NetTcpBinding();
            EndpointAddress endpoint = new EndpointAddress($"net.tcp://{resultado.Item3}:17100/ServicioAlarma/");
            var channelFactory = new ChannelFactory<IContrato>(binding, endpoint);
            IContrato cliente = null;
            try
            {
                cliente = channelFactory.CreateChannel();
                cliente.AnunciarError(resultado.Item2);
                ((ICommunicationObject)cliente).Close();
            }
            catch (Exception)
            {
                if (cliente != null)
                    ((ICommunicationObject)cliente).Abort();
                throw;
            }
        }

        public void AnunciarProblemaMobile(long idTosProcess, short idZona)
        {
            IProblema administradorProblema = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Springs/SpringProblema.xml");
                administradorProblema = (IProblema)ctx["AdministradorProblema"];
                var resultado = administradorProblema.RegistrarProblemaMobile(idTosProcess, idZona);
                if (resultado.Item1)
                    EnviarMensajeProblema(resultado);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (administradorProblema != null)
                    administradorProblema.LiberarRecursos();
            }
        }

        public void AnunciarProblemaGenericoMobile(string mensajeError, short idZona)
        {
            IProblema administradorProblema = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Springs/SpringProblema.xml");
                administradorProblema = (IProblema)ctx["AdministradorProblema"];
                var resultado = administradorProblema.RegistrarProblemaGenericoMobile(mensajeError, idZona);
                if (resultado.Item1)
                    EnviarMensajeProblema(resultado);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (administradorProblema != null)
                    administradorProblema.LiberarRecursos();
            }
        }

        public void AnunciarProblemaClienteAppTransaction(int idError, short idZona)
        {
            IProblema administradorProblema = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Springs/SpringProblema.xml");
                administradorProblema = (IProblema)ctx["AdministradorProblema"];
                var resultado = administradorProblema.RegistrarProblemaClienteAppTransaction(idError, idZona);
                if (resultado.Item1)
                    EnviarMensajeProblema(resultado);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (administradorProblema != null)
                    administradorProblema.LiberarRecursos();
            }
        }

        public void AnunciarProblemaServicioWebTransaction(string error, short idZona)
        {
            IProblema administradorProblema = null;
            try
            {
                var ctx = new XmlApplicationContext("~/Springs/SpringProblema.xml");
                administradorProblema = (IProblema)ctx["AdministradorProblema"];
                var resultado = administradorProblema.RegistrarProblemaServicioWebTransaction(error, idZona);
                if (resultado.Item1)
                    EnviarMensajeProblema(resultado);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (administradorProblema != null)
                    administradorProblema.LiberarRecursos();
            }
        }
    }
}

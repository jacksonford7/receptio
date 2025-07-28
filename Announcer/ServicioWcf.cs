using RECEPTIO.CapaPresentacion.TroubleDeskContrato;
using System;
using System.IO;
using System.Management;
using System.ServiceModel;

namespace Announcer
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ServicioWcf : IContrato
    {
        private Maquina _maquina;

        public ServicioWcf()
        {
            _maquina = Maquina.ObtenerInstancia();
        }

        public void AnunciarError(string mensajeError)
        {
            var ruta = $@"C:\Users\{_maquina.ObtenerUsuarioLogueado()}\AppData\Local\Packages\ebd3f0d6-bc77-4051-8b75-b7805679b9f4_4bc8v0095zkxy\LocalState\Observador.txt";
            if (!File.Exists(ruta))
            {
                using (StreamWriter sw = File.CreateText(ruta))
                {
                    sw.WriteLine(mensajeError);
                }
            }
        }
    }

    internal class Maquina
    {
        private static readonly Object _bloquear = new Object();
        private static Maquina _maquina;

        private Maquina()
        {
        }

        internal static Maquina ObtenerInstancia()
        {
            if (_maquina == null)
            {
                lock (_bloquear)
                {
                    if (_maquina == null)
                    {
                        _maquina = new Maquina();
                    }
                }
            }
            return _maquina;
        }

        internal string ObtenerUsuarioLogueado()
        {
            string username = null;
            try
            {
                var ms = new ManagementScope("\\\\.\\root\\cimv2");
                ms.Connect();
                var consulta = new ObjectQuery("SELECT * FROM Win32_ComputerSystem");
                var buscador = new ManagementObjectSearcher(ms, consulta);
                foreach (var mo in buscador.Get())
                    username = mo["UserName"].ToString();
                string[] usernameParts = username.Split('\\');
                username = usernameParts[usernameParts.Length - 1];
            }
            catch (Exception)
            {
                throw;
            }
            return username;
        }
    }
}

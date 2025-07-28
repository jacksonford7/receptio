using System.ServiceProcess;

namespace Announcer
{
    static class Program
    {
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Servicio()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}

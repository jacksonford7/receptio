using System.ServiceModel;
using System.ServiceProcess;

namespace Announcer
{
    partial class Servicio : ServiceBase
    {
        private ServiceHost _servicio;

        public Servicio()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _servicio = new ServiceHost(new ServicioWcf());
            _servicio.Open();
        }

        protected override void OnStop()
        {
            _servicio.Close();
        }
    }
}

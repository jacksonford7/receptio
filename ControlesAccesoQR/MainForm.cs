using System;
using System.Windows.Forms;

namespace RECEPTIO.ControlesAccesoQR
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnEntradaSalida_Click(object sender, EventArgs e)
        {
            new FrmEntradaSalida().Show();
        }

        private void btnSalidaFinal_Click(object sender, EventArgs e)
        {
            new FrmSalidaFinal().Show();
        }
    }
}

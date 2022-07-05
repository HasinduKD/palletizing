using System;
using System.Deployment.Application;
using System.Windows.Forms;

namespace Demo
{
    public partial class FrmSplash : Form
    {
        public FrmSplash()
        {
            Location = Screen.PrimaryScreen.Bounds.Location;
            InitializeComponent();
        }
        private void FrmSplash_Load(object sender, EventArgs e)
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                lblV1Version.Text = "Version : " + ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }
            tmSplash.Start();
        }
        private void TmSplash_Tick(object sender, EventArgs e)
        {
            panel2.Width += 5;
            if (panel2.Width >= 525)
            {
                tmSplash.Stop();
                new FrmMenu() { Location = Screen.PrimaryScreen.Bounds.Location }.Show();
                this.Hide();
            }
        }
    }
}
using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualBasic;


namespace Demo
{
    public partial class FrmCustermerDetails : Form
    {
        private Form dialog;
        //List<string> listA = new List<string>();

        #region INITIALIZE

        //public FrmCustermerDetails(List<string> customer_data)
        public FrmCustermerDetails()
        {
            InitializeComponent();
            //listA = customer_data;

        }

        #endregion INITIALIZE

        public static DialogResult ShowDialog(Form parent, Form dialog)
        {
            FrmCustermerDetails mask = new  FrmCustermerDetails(parent, dialog);
            dialog.StartPosition = FormStartPosition.CenterParent;
            mask.Show();
            var result = dialog.ShowDialog(mask);
            mask.Close();
            return result;
        }

        internal  FrmCustermerDetails(Form parent, Form dialog)
        {

            this.dialog = dialog;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.Black;
            this.Opacity = 0.50;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;
            this.Size = parent.ClientSize;

            this.Location = parent.PointToScreen(System.Drawing.Point.Empty);
            parent.Move += AdjustPosition;
            parent.SizeChanged += AdjustPosition;
        }

        private void AdjustPosition(object sender, EventArgs e)
        {
            Form parent = sender as Form;
            this.Location = parent.PointToScreen(System.Drawing.Point.Empty);
            this.ClientSize = parent.ClientSize;
        }

        private void btnV2CloseWindow_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LoadCSVOnDataGridView(string fileName)
        {
            try
            {
                ReadCSV csv = new ReadCSV(fileName);

                try
                {
                    dgvV2CustermerDetails.DataSource = csv.readCSV;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void dgvV2CustermerDetails_Enter(object sender, EventArgs e)
        {
            LoadCSVOnDataGridView("C:\\Users\\ASUS\\Desktop\\customer_order.csv");
            //dgvV2CustermerDetails.DataSource = listA;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            dgvV2CustermerDetails.Update();
            LoadCSVOnDataGridView("C:\\Users\\ASUS\\Desktop\\customer_order.csv");
        }
    }
}

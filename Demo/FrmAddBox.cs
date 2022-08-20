using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Demo
{
    public partial class FrmAddBox : Form
    {
        private Form dialog;


        public FrmAddBox()
        {
            InitializeComponent();

        }

        private void frmIWDUploadView2_Load(object sender, EventArgs e)
        {

        }
  
        internal FrmAddBox(Form parent, Form dialog)
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

        public static DialogResult ShowDialog(Form parent, Form dialog)
        {
            FrmAddBox mask = new FrmAddBox(parent, dialog);
            dialog.StartPosition = FormStartPosition.CenterParent;
            mask.Show();
            var result = dialog.ShowDialog(mask);
            mask.Close();
            return result;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Escape))
            {
                this.Close();
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void btnV2CloseWindow_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void pnlIWDUploadView2_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                pnlIWDUploadView2.Location = new Point(
                    this.ClientSize.Width / 2 - pnlIWDUploadView2.Size.Width / 2,
                    this.ClientSize.Height / 2 - pnlIWDUploadView2.Size.Height / 2);
                pnlIWDUploadView2.Anchor = AnchorStyles.None;

                ControlPaint.DrawBorder(e.Graphics, pnlIWDUploadView2.ClientRectangle,
                    Color.SteelBlue, 5, ButtonBorderStyle.Dotted,
                    Color.SteelBlue, 5, ButtonBorderStyle.Dotted,
                    Color.SteelBlue, 5, ButtonBorderStyle.Dotted,
                    Color.SteelBlue, 5, ButtonBorderStyle.Dotted);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Validate user input for box ID for a 2 digit integer
            textBox1.MaxLength = 2;

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtV1T1UserId_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Validate user input for box length for a 3 digit integer
            txtV1T1UserId.MaxLength = 3;

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Validate user input for box width for a 3 digit integer
            textBox2.MaxLength = 3;

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Validate user input for box height for a 3 digit integer
            textBox3.MaxLength = 3;

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Validate user input for box weight for a 6 digit decimal
            textBox4.MaxLength = 6;

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void btTPAddBoxClear_Click(object sender, EventArgs e)
        {
            txtV1T1UserId.Clear();
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
        }

        public class company_product
        {
            public int box_id { get; set; }
            public int box_length { get; set; }
            public int box_width { get; set; }
            public int box_height { get; set; }
            public int box_weight { get; set; }
        }

        public static List<company_product> products = new List<company_product>();

        public void btnV1T1Add_Click(object sender, EventArgs e)
        {
            var id = textBox1.Text.ToString();
            var length = txtV1T1UserId.Text.ToString();
            var width = textBox2.Text.ToString();
            var height = textBox3.Text.ToString();
            var weight = textBox4.Text.ToString();
            
            products.Add(new company_product() {box_id = int.Parse(id), box_length = int.Parse(length), box_width = int.Parse(width), box_height = int.Parse(height), box_weight = int.Parse(weight)});
            this.Close();
        }
    }
}
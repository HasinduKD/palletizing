using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

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

        private void btTPAddBoxClear_Click(object sender, EventArgs e)
        {
            txtV1T1UserId.Clear();
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
        }

        private void btnV1T1Add_Click(object sender, EventArgs e)
        {
            StringBuilder csvcontent = new StringBuilder();
            var id = textBox1.Text.ToString();
            var length = txtV1T1UserId.Text.ToString();
            var width = textBox2.Text.ToString();
            var height = textBox3.Text.ToString();
            var weight = textBox4.Text.ToString();

            var newLine = string.Format("{0}, {1}, {2}, {3}, {4}", id, length, width, height, weight);
            csvcontent.AppendLine(newLine);
            string csvpath = "C:\\Users\\ASUS\\Desktop\\box_data.csv";
            File.AppendAllText(csvpath, csvcontent.ToString());
            this.Close();
        }
    }
}
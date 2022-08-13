using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using ABB.Robotics.Controllers;
using ABB.Robotics.Controllers.Discovery;
using ABB.Robotics.Controllers.RapidDomain;
using static Demo.FrmAddBox;

namespace Demo
{
    public partial class FrmMenu : Form
    {
        #region VARIABLES 
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        RapidData Rb_speed;
        RapidData box_1;
        RapidData box_2;
        Num v;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        private const int cGrip = 16;      // Grip size
        private const int cCaption = 32;   // Caption bar height;

        private const int PalletWidth = 300;
        private const int PalletHeight = 300;

        public AppControllerData AppControllerData { get; private set; }

        #endregion

        #region INITIALIZE
        public FrmMenu()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            
            AppControllerData = new AppControllerData(this);

            Initialize_Graphics(PalletWidth, PalletHeight);
        }

        #endregion INITIALIZE      

        #region EVENTS
        private void BtnV1Close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void BtnV1RestoreWindow_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
            btnV1RestoreWindow.Visible = false;
            btnV1MaximizeWindow.Visible = true;
        }
        private void BtnV1MaximizeWindow_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;
            btnV1RestoreWindow.Visible = true;
            btnV1MaximizeWindow.Visible = false;
        }
        private void BtnV1MinimizeWindow_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
        private void pnlV1MenuTop_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                btnV1RestoreWindow.Visible = false;
                btnV1MaximizeWindow.Visible = true;
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            FrmAddBox.ShowDialog(this, new FrmAddBox());
        }
        public static DialogResult ShowDialog(Form parent, Form dialog, string message)
        {
            FrmAddBox mask = new FrmAddBox(parent, dialog);
            dialog.StartPosition = FormStartPosition.CenterParent;
            mask.Show();
            DialogResult result = dialog.ShowDialog(mask);
            mask.Close();
            return result;
        }
        private void DropDownKeyPress(object sender, KeyPressEventArgs e) => e.Handled = true;
        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rc = new Rectangle(this.ClientSize.Width - cGrip, this.ClientSize.Height - cGrip, cGrip, cGrip);
            ControlPaint.DrawSizeGrip(e.Graphics, this.BackColor, rc);
            rc = new Rectangle(0, 0, this.ClientSize.Width, cCaption);
            e.Graphics.FillRectangle(Brushes.DarkBlue, rc);
        }
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x84)
            {  // Trap WM_NCHITTEST
                Point pos = new Point(m.LParam.ToInt32());
                pos = this.PointToClient(pos);
                if (pos.Y < cCaption)
                {
                    m.Result = (IntPtr)2;  // HTCAPTION
                    return;
                }
                if (pos.X >= this.ClientSize.Width - cGrip && pos.Y >= this.ClientSize.Height - cGrip)
                {
                    m.Result = (IntPtr)17; // HTBOTTOMRIGHT
                    return;
                }
            }
            base.WndProc(ref m);
        }
        private void FrmMenu_Load(object sender, EventArgs e)
        {
            try
            {
                #region Home

                #endregion

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }        
        }

        #endregion EVENTS

        #region HomePage 
        #region Events
        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void btnV1T1Add_Click(object sender, EventArgs e)
        {

        }

        private void ddlTP1Controller_Enter(object sender, EventArgs e)
        {
            AppControllerData.FindControllers();
            SetDropDownControllerData();

        }

        private void lblV1ProductName_Click(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label_time.Text = DateTime.Now.ToLongTimeString();
            label_date.Text = DateTime.Now.ToLongDateString();
        }

        private void ddlTP1Controller_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlTP1Controller.SelectedIndex != -1)
            {
                ddTP1Task.Enabled = true;
                var comboBoxControllers = sender as ComboBox;
                AppControllerData.SelectedController = Controller.Connect(comboBoxControllers.SelectedItem as ControllerInfo, ConnectionType.Standalone);
                AppControllerData.SelectedController.Logon(UserInfo.DefaultUser);
                AppControllerData.Tasks?.Clear();
                AppControllerData.Modules?.Clear();
                AppControllerData.RapidVariables?.Clear();

                AppControllerData.Tasks = AppControllerData.SelectedController.Rapid.GetTasks().ToList();
                SetDropDownTaskData();
                string msg = DateTime.Now + " " + "Controller changed to" + " " + comboBoxControllers.SelectedItem.ToString();
                txtV1ExecutionLog.AppendText(msg);
                txtV1ExecutionLog.AppendText(Environment.NewLine);
            }
        }

        private void ddTP1Task_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddTP1Task.SelectedIndex != -1)
            {
                ddTP1Module.Enabled = true;
                var comboBoxTasks = sender as ComboBox;
                AppControllerData.SelectedTask = comboBoxTasks.SelectedItem as Task;
                AppControllerData.Modules?.Clear();
                AppControllerData.RapidVariables?.Clear();

                if (AppControllerData.SelectedTask == null)
                    return;

                AppControllerData.Modules = AppControllerData.SelectedTask.GetModules().ToList();
                SetDropDownModuleData();
                string msg = DateTime.Now + " " + "Task changed to" + " " + comboBoxTasks.SelectedItem.ToString();
                txtV1ExecutionLog.AppendText(msg);
                txtV1ExecutionLog.AppendText(Environment.NewLine);
            }
        }

        private void ddTP1Module_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddTP1Module.SelectedIndex != -1)
            {
                btTP1RobotSpeed.Enabled = true;
                var comboBoxModules = sender as ComboBox;
                AppControllerData.SelectedModule = comboBoxModules.SelectedItem as Module;
                AppControllerData.RapidVariables?.Clear();

                if (AppControllerData.SelectedModule == null)
                    return;

                AppControllerData.RapidVariables = AppControllerData.SelectedModule.SearchRapidSymbol(RapidSymbolSearchProperties.CreateDefaultForData()).ToList();
                string msg = DateTime.Now + " " + "Module changed to" + " " + comboBoxModules.SelectedItem.ToString();
                txtV1ExecutionLog.AppendText(msg);
                txtV1ExecutionLog.AppendText(Environment.NewLine);
            }
        }

        private void btTP1RobotSpeed_Click(object sender, EventArgs e)
        {
            string rb_speed = txtTP1RobotSpeed.Text;
            if (string.IsNullOrEmpty(rb_speed))
            {

            }
            else
            {
                v = Num.Parse(rb_speed);
            }

            using (Mastership master = Mastership.Request(AppControllerData.SelectedController))
            {
                try
                {
                    Rb_speed = AppControllerData.SelectedModule.GetRapidData("RobotSpeed");
                    Rb_speed.Value = v;
                    master.Release();
                    string msg = DateTime.Now + " " + "Robot Speed changed to" + " " + v.ToString();
                    txtV1ExecutionLog.AppendText(msg);
                    txtV1ExecutionLog.AppendText(Environment.NewLine);
                    btTP2Start.Enabled = true;
                    btTP2Stop.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unexpected error occurred: " + ex.Message);
                }
            }
        }

        #endregion

        #region Method
        private void SetDropDownControllerData()
        {
            ddlTP1Controller.SelectedIndexChanged -= ddlTP1Controller_SelectedIndexChanged;
            ddlTP1Controller.DataSource = AppControllerData.Controllers;
            ddlTP1Controller.SelectedIndex = -1;
            ddlTP1Controller.Text = "Please select a Controller";
            ddlTP1Controller.SelectedIndexChanged += ddlTP1Controller_SelectedIndexChanged;
        }

        private void SetDropDownTaskData()
        {
            ddTP1Task.SelectedIndexChanged -= ddTP1Task_SelectedIndexChanged;
            ddTP1Task.DataSource = AppControllerData.Tasks;
            ddTP1Task.SelectedIndex = -1;
            ddTP1Task.Text = "Please select a Task";
            ddTP1Task.SelectedIndexChanged += ddTP1Task_SelectedIndexChanged;
        }

        private void SetDropDownModuleData()
        {
            ddTP1Module.SelectedIndexChanged -= ddTP1Module_SelectedIndexChanged;
            ddTP1Module.DataSource = AppControllerData.Modules;
            ddTP1Module.SelectedIndex = -1;
            ddTP1Module.Text = "Please select a Module";
            ddTP1Module.SelectedIndexChanged += ddTP1Module_SelectedIndexChanged;
        }

        #endregion Method

        #endregion Homepage

        #region ControlPage

        private void btTP2Start_Click(object sender, EventArgs e)
        {
            btTP1RobotSpeed.Enabled = false;
            btTP2Reset.Enabled = false;
            try
            {
                if (AppControllerData.SelectedController.OperatingMode == ControllerOperatingMode.Auto)
                {
                    using (Mastership master = Mastership.Request(AppControllerData.SelectedController))
                    {
                        try
                        {
                            if (AppControllerData.SelectedTask.Enabled == false)
                                AppControllerData.SelectedTask.Enabled = true;

                            if (AppControllerData.SelectedTask.ExecutionStatus.Equals(TaskExecutionStatus.Running))
                                MessageBox.Show("Task is already running.");

                            else
                            {
                                //if (AppControllerData.SelectedTask.ExecutionStatus.Equals(TaskExecutionStatus.Stopped))
                                //    AppControllerData.SelectedTask.ResetProgramPointer();

                                AppControllerData.SelectedController.Rapid.Start(RegainMode.Continue, ExecutionMode.Continuous, ExecutionCycle.AsIs, StartCheck.None, true, TaskPanelExecutionMode.NormalTasks);
                                string msg = DateTime.Now + " " + "The system has started operation";
                                txtV1ExecutionLog.AppendText(msg);
                                txtV1ExecutionLog.AppendText(Environment.NewLine);
                                box_1 = AppControllerData.SelectedModule.GetRapidData("BoxID_1");
                                box_2 = AppControllerData.SelectedModule.GetRapidData("BoxID_2");
                                txtV1T1UserId.Text = box_1.Value.ToString();
                                textBox2.Text = box_2.Value.ToString();
                            }

                            master.Release();
                        }
                        catch (InvalidOperationException ex)
                        {
                            MessageBox.Show("Mastership is held by another client." + ex.Message);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Automatic mode is required to start/stop execution from a remote client.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error occurred: " + ex.Message);
            }

        }

        private void btTP2Stop_Click(object sender, EventArgs e)
        {
            btTP1RobotSpeed.Enabled = true;
            btTP2Reset.Enabled = true;
            try
            {
                if (AppControllerData.SelectedController.OperatingMode == ControllerOperatingMode.Auto)
                {
                    using (Mastership master = Mastership.Request(AppControllerData.SelectedController))
                    {
                        try
                        {
                            AppControllerData.SelectedTask.Stop();
                            master.Release();
                            string msg = DateTime.Now + " " + "The system has stopped operation";
                            txtV1ExecutionLog.AppendText(msg);
                            txtV1ExecutionLog.AppendText(Environment.NewLine);
                        }
                        catch (InvalidOperationException ex)
                        {
                            MessageBox.Show("Mastership is held by another client." + ex.Message);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Automatic mode is required to start/stop execution from a remote client.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error occurred: " + ex.Message);
            }
        }

        private void btTP2Reset_Click(object sender, EventArgs e)
        {
            btTP1RobotSpeed.Enabled = false;
            try
            {
                if (AppControllerData.SelectedController.OperatingMode == ControllerOperatingMode.Auto)
                {
                    using (Mastership master = Mastership.Request(AppControllerData.SelectedController))
                    {
                        try
                        {
                            if (AppControllerData.SelectedTask.Enabled == false)
                                AppControllerData.SelectedTask.Enabled = true;

                            if (AppControllerData.SelectedTask.ExecutionStatus.Equals(TaskExecutionStatus.Running))
                                MessageBox.Show("Task is already running.");

                            else
                            {
                                if (AppControllerData.SelectedTask.ExecutionStatus.Equals(TaskExecutionStatus.Stopped))
                                    AppControllerData.SelectedTask.ResetProgramPointer();
                                    txtV1ExecutionLog.Clear();

                                AppControllerData.SelectedController.Rapid.Start(RegainMode.Continue, ExecutionMode.Continuous, ExecutionCycle.AsIs, StartCheck.None, true, TaskPanelExecutionMode.NormalTasks);
                                string msg = DateTime.Now + " " + "The system operation has been reset";
                                txtV1ExecutionLog.AppendText(msg);
                                txtV1ExecutionLog.AppendText(Environment.NewLine);
                            }

                            master.Release();
                        }
                        catch (InvalidOperationException ex)
                        {
                            MessageBox.Show("Mastership is held by another client." + ex.Message);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Automatic mode is required to start/stop execution from a remote client.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error occurred: " + ex.Message);
            }
        }

        #endregion ControlPage

        #region StackingPage
        private void ddTP1Pallet_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedPallet = ddTP1Pallet.SelectedItem.ToString();
            //MessageBox.Show("selected pallet is {0}", selectedPallet);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;

            FrmCustermerDetails.ShowDialog(this, new FrmCustermerDetails());
            StringBuilder csvfinal = new StringBuilder();
            string csvpath = "C:\\Users\\ASUS\\Desktop\\final_list.csv";
            var header = string.Format("{0}, {1}, {2}, {3}, {4}, {5}", "Box ID", "Length", "Width", "Height", "Weight", "Quantity");
            csvfinal.AppendLine(header);

            using (var reader = new StreamReader("C:\\Users\\ASUS\\Desktop\\customer_order.csv"))
            {
                List<string> listA = new List<string>();
                List<string> listB = new List<string>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    listA.Add(values[0]);
                    listB.Add(values[1]);
                }
                foreach (var item in listA)
                {
                    foreach (var _item in products)
                    {
                        if (item == _item.box_id.ToString())
                        {
                            int index = listA.IndexOf(item);
                            var products_order = string.Format("{0}, {1}, {2}, {3}, {4}, {5}", _item.box_id, _item.box_length, _item.box_width, _item.box_height, _item.box_weight, listB[index]);
                            csvfinal.AppendLine(products_order);
                            break;
                        }
                        else continue;
                    }
                }
                
                File.WriteAllText(csvpath, string.Empty);
                File.AppendAllText(csvpath, csvfinal.ToString());
                string msg = DateTime.Now + " " + "The customer order was imported";
                txtV1ExecutionLog.AppendText(msg);
                txtV1ExecutionLog.AppendText(Environment.NewLine);
                //var message = string.Join(Environment.NewLine, listA);
                //MessageBox.Show(message);
                //    FrmCustermerDetails cus = new FrmCustermerDetails(listA);
                //}
            }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            button3.Enabled = false;
            
            StringBuilder csvcontent = new StringBuilder();
            var header = string.Format("{0}, {1}, {2}, {3}, {4}", "Box ID", "Length", "Width", "Height", "Weight");
            csvcontent.AppendLine(header);
            string csvpath = "C:\\Users\\ASUS\\Desktop\\box_data.csv";

            File.WriteAllText(csvpath, string.Empty);

            foreach (var item in products)
            {
                var productsResults = string.Format("{0}, {1}, {2}, {3}, {4}", item.box_id, item.box_length, item.box_width, item.box_height, item.box_weight);
                csvcontent.AppendLine(productsResults);
            }
            File.AppendAllText(csvpath, csvcontent.ToString());
            string msg = DateTime.Now + " " + "A list of new products was created";
            txtV1ExecutionLog.AppendText(msg);
            txtV1ExecutionLog.AppendText(Environment.NewLine);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Draw_Graphics(boxTable, 1);
            Refresh();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            button6.Enabled = true;
            button7.Enabled = true;
            button8.Enabled = true;

            OpenFileDialog finalListFileDialog = new OpenFileDialog()
            {
                FileName = "Select a csv file",
                Filter = "CSV files (*.csv)|*.csv",
                Title = "Open csv file"
            };

            if (finalListFileDialog.ShowDialog() != DialogResult.OK) return;

            boxTable = new ReadCSV(finalListFileDialog.FileName).readCSV;
        }

        private DataTable boxTable;

        private void button1_Click(object sender, EventArgs e)
        {
            Draw_Graphics(boxTable, 1);
            Refresh();
        }

        Graphics _drawingTable;

        private void Initialize_Graphics(int width, int height)
        {
            // increase length and width by one
            width++;
            height++;

            // set new bitmap to image
            pictureBox2.Image = new Bitmap(width, height);

            // set new drawing table Graphics
            _drawingTable = Graphics.FromImage(pictureBox2.Image);

            // clear drawing table
            _drawingTable.Clear(Color.White);
        }

        private void Draw_Graphics(DataTable boxDataTable, int layer)
        {
            Pen pencil = new Pen(Color.Black, 1);
            Brush brush = new SolidBrush(Color.Black);
            Font font = new Font("Lucida", 8, FontStyle.Regular);

            Rectangle element = new Rectangle();
            Point indexCoordinates = new Point();
            Point heightCoordinates = new Point();

            // clear drawing table
            _drawingTable.Clear(Color.White);

            Random rnd = new Random();

            foreach (DataRow boxRow in boxDataTable.Rows)
            {
                // Not in current layer, ignore.
                if (!int.Parse(boxRow["Layer"].ToString()).Equals(layer)) continue;

                // Not valid coordinate, ignore.
                if (string.IsNullOrEmpty(boxRow["P1X"].ToString()) || string.IsNullOrEmpty(boxRow["P1Y"].ToString())
                    || string.IsNullOrEmpty(boxRow["P2X"].ToString()) || string.IsNullOrEmpty(boxRow["P2Y"].ToString())
                    || string.IsNullOrEmpty(boxRow["P3X"].ToString()) || string.IsNullOrEmpty(boxRow["P3Y"].ToString())
                    || string.IsNullOrEmpty(boxRow["P4X"].ToString()) || string.IsNullOrEmpty(boxRow["P4Y"].ToString()))
                    continue;

                // set element width and heigth
                element.Width = (int.Parse(boxRow["P2X"].ToString()) - int.Parse(boxRow["P1X"].ToString())) * 3;
                element.Height = (int.Parse(boxRow["P3Y"].ToString()) - int.Parse(boxRow["P2Y"].ToString())) * 3;

                // set element lower left corner coordinates
                element.X = (int.Parse(boxRow["P4X"].ToString())) * 3;
                element.Y = PalletHeight - ((int.Parse(boxRow["P4Y"].ToString())) * 3);

                // draw element at DrawingTable
                //_drawingTable.DrawRectangle(pencil, element);

                SolidBrush blueBrush = new SolidBrush(Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256)));
                _drawingTable.FillRectangle(blueBrush, element);

                // set index coordinates
                indexCoordinates.X = element.X;
                indexCoordinates.Y = element.Y;

                StringFormat format = new StringFormat
                {
                    LineAlignment = StringAlignment.Center,
                    Alignment = StringAlignment.Center
                };

                heightCoordinates.X = element.Left + element.Width / 2;
                heightCoordinates.Y = element.Top + element.Height / 2;

                // draw index value
                _drawingTable.DrawString("ID" + boxRow["Box ID"].ToString(), font, brush, indexCoordinates);
                _drawingTable.DrawString("H" + boxRow["Height"].ToString(), font, brush, heightCoordinates, format);
            }

            // release resources
            pencil.Dispose();
            brush.Dispose();
            font.Dispose();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Draw_Graphics(boxTable, 2);
            Refresh();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Draw_Graphics(boxTable, 3);
            Refresh();
        }
        #endregion StackingPage

    }
}
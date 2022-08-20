using ABB.Robotics.Controllers;
using ABB.Robotics.Controllers.RapidDomain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static Demo.FrmAddBox;

namespace Demo
{
    public partial class FrmMenu : Form
    {
        #region VARIABLES 
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        // Declaring variables to store the robot speed from Robotstudio
        RapidData Rb_speed;
        Num v;
        // Declaring variables to store the box data from Robotstudio
        RapidData box_1;
        RapidData box_2;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        // Declaring the Grip size and Caption bar height
        private const int cGrip = 16;
        private const int cCaption = 32;
        // Declaring the pallet dimensions, 300 was used to display a clear image as 100 generates a small pallet
        private const int PalletWidth = 300;
        private const int PalletHeight = 300;

        private readonly Dictionary<int, List<BoxState>> _currentDrawingBoxState;
        private readonly Dictionary<int, int> _currentBoxIndexInLayer;

        // Default layer is 1
        private int _currentLayer = 1;

        private readonly Random _rnd;

        private bool _isControlStop;

        public AppControllerData AppControllerData { get; private set; }

        #endregion

        #region INITIALIZE
        public FrmMenu()
        {
            // Method to define the content on the form
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            
            // Create new object from class of AppControllerData 
            AppControllerData = new AppControllerData(this);
            
            Initialize_Graphics(PalletWidth, PalletHeight);
            
            _currentDrawingBoxState = new Dictionary<int, List<BoxState>>();
            _currentBoxIndexInLayer = new Dictionary<int, int>();

            _rnd = new Random();
        }
        #endregion INITIALIZE      

        #region EVENTS
        private void BtnV1Close_Click(object sender, EventArgs e)
        {
            // Close application when the exit icon is pressed
            Application.Exit();
        }
        private void BtnV1RestoreWindow_Click(object sender, EventArgs e)
        {
            // Change the size of the form to standard size 
            WindowState = FormWindowState.Normal;
            btnV1RestoreWindow.Visible = false;
            btnV1MaximizeWindow.Visible = true;
        }
        private void BtnV1MaximizeWindow_Click(object sender, EventArgs e)
        {
            // Change the size of the form to maximum
            WindowState = FormWindowState.Maximized;
            btnV1RestoreWindow.Visible = true;
            btnV1MaximizeWindow.Visible = false;
        }
        private void BtnV1MinimizeWindow_Click(object sender, EventArgs e)
        {
            // Minimize the application the minimize icon is pressed
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
        // Show the loading image of UI when starting
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
        private void ddlTP1Controller_Enter(object sender, EventArgs e)
        {
            // Get and show controller data on the drop-down box
            AppControllerData.FindControllers();
            SetDropDownControllerData();
        }

        private void lblV1ProductName_Click(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Display date and time
            label_time.Text = DateTime.Now.ToLongTimeString();
            label_date.Text = DateTime.Now.ToLongDateString();
        }

        private void ddlTP1Controller_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If the controller selection has changed and it is not the default choice, clear previous data and get new task data 
            if (ddlTP1Controller.SelectedIndex != -1)
            {
                ddTP1Task.Enabled = true;
                var comboBoxControllers = sender as ComboBox;
                AppControllerData.SelectedController = Controller.Connect(comboBoxControllers.SelectedItem as ControllerInfo, ConnectionType.Standalone);
                AppControllerData.SelectedController.Logon(UserInfo.DefaultUser);
                AppControllerData.Tasks?.Clear();
                AppControllerData.Modules?.Clear();
                AppControllerData.RapidVariables?.Clear();

                // Load the task data for selected controller
                AppControllerData.Tasks = AppControllerData.SelectedController.Rapid.GetTasks().ToList();
                SetDropDownTaskData();
                // Display the change of controllers in activity log
                string msg = DateTime.Now + " " + "Controller changed to" + " " + comboBoxControllers.SelectedItem.ToString();
                txtV1ExecutionLog.AppendText(msg);
                txtV1ExecutionLog.AppendText(Environment.NewLine);
            }
        }

        private void ddTP1Task_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If the task selection has changed and it is not the default choice, clear previous data and get new module data
            if (ddTP1Task.SelectedIndex != -1)
            {
                ddTP1Module.Enabled = true;
                var comboBoxTasks = sender as ComboBox;
                AppControllerData.SelectedTask = comboBoxTasks.SelectedItem as Task;
                AppControllerData.Modules?.Clear();
                AppControllerData.RapidVariables?.Clear();

                if (AppControllerData.SelectedTask == null)
                    return;

                // Load the module data for selected task
                AppControllerData.Modules = AppControllerData.SelectedTask.GetModules().ToList();
                SetDropDownModuleData();
                // Display the change of tasks in activity log
                string msg = DateTime.Now + " " + "Task changed to" + " " + comboBoxTasks.SelectedItem.ToString();
                txtV1ExecutionLog.AppendText(msg);
                txtV1ExecutionLog.AppendText(Environment.NewLine);
            }
        }

        private void ddTP1Module_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If the module selection has changed and it is not the default choice, clear previous data and get new rapid variable data
            if (ddTP1Module.SelectedIndex != -1)
            {
                // Enable the button to change the robot speed 
                btTP1RobotSpeed.Enabled = true;
                var comboBoxModules = sender as ComboBox;
                AppControllerData.SelectedModule = comboBoxModules.SelectedItem as Module;
                AppControllerData.RapidVariables?.Clear();

                if (AppControllerData.SelectedModule == null)
                    return;

                // Load the rapid variable data for selected module
                AppControllerData.RapidVariables = AppControllerData.SelectedModule.SearchRapidSymbol(RapidSymbolSearchProperties.CreateDefaultForData()).ToList();
                // Display the change of modules in activity log
                string msg = DateTime.Now + " " + "Module changed to" + " " + comboBoxModules.SelectedItem.ToString();
                txtV1ExecutionLog.AppendText(msg);
                txtV1ExecutionLog.AppendText(Environment.NewLine);
            }
        }

        private void txtTP1RobotSpeed_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Validate user input for robot speed for a 2 digit integer
            txtTP1RobotSpeed.MaxLength = 2;

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void btTP1RobotSpeed_Click(object sender, EventArgs e)
        {
            // Store and convert the entered robot speed into a num type variable
            string rb_speed = txtTP1RobotSpeed.Text;
            if (string.IsNullOrEmpty(rb_speed))
            {

            }
            else
            {
                v = Num.Parse(rb_speed);
            }

            // Connect to controller in Robotstudio and update the value of robot speed with value entered in UI
            using (Mastership master = Mastership.Request(AppControllerData.SelectedController))
            {
                try
                {
                    Rb_speed = AppControllerData.SelectedModule.GetRapidData("RobotSpeed");
                    Rb_speed.Value = v;
                    master.Release();
                    
                    // Display the new value of robot speed in activity log
                    string msg = DateTime.Now + " " + "Robot Speed changed to" + " " + v.ToString();
                    txtV1ExecutionLog.AppendText(msg);
                    txtV1ExecutionLog.AppendText(Environment.NewLine);
                    
                    // Enable the start and stop buttons in Control tab
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
            // Bind the data of controllers from Robotstudio to controller drop-down list with an initial default option
            ddlTP1Controller.SelectedIndexChanged -= ddlTP1Controller_SelectedIndexChanged;
            ddlTP1Controller.DataSource = AppControllerData.Controllers;
            ddlTP1Controller.SelectedIndex = -1;
            ddlTP1Controller.Text = "Please select a Controller";
            ddlTP1Controller.SelectedIndexChanged += ddlTP1Controller_SelectedIndexChanged;
        }

        private void SetDropDownTaskData()
        {
            // Bind the data of tasks from Robotstudio to task drop-down list with an initial default option
            ddTP1Task.SelectedIndexChanged -= ddTP1Task_SelectedIndexChanged;
            ddTP1Task.DataSource = AppControllerData.Tasks;
            ddTP1Task.SelectedIndex = -1;
            ddTP1Task.Text = "Please select a Task";
            ddTP1Task.SelectedIndexChanged += ddTP1Task_SelectedIndexChanged;
        }

        private void SetDropDownModuleData()
        {
            // Bind the data of modules from Robotstudio to module drop-down list with an initial default option
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
            // Disable the buttons for changing robot speed and resetting when the system is started
            btTP1RobotSpeed.Enabled = false;
            btTP2Reset.Enabled = false;

            if (_boxTable.Rows.Count <= 0)
            {
                MessageBox.Show("Warning! List of box is empty.");
                return;
            }

            StartDrawingBoxManualMode();

            // set control state
            _isControlStop = false;

            try
            {
                if (AppControllerData.SelectedController.OperatingMode == ControllerOperatingMode.Auto)
                {
                    using (Mastership master = Mastership.Request(AppControllerData.SelectedController))
                    {
                        // Try to start the selected task in Robotstudio and display error message if failed
                        try
                        {
                            if (AppControllerData.SelectedTask.Enabled == false)
                                AppControllerData.SelectedTask.Enabled = true;

                            if (AppControllerData.SelectedTask.ExecutionStatus.Equals(TaskExecutionStatus.Running))
                                MessageBox.Show("Task is already running.");

                            else
                            {
                                AppControllerData.SelectedController.Rapid.Start(RegainMode.Continue, ExecutionMode.Continuous, ExecutionCycle.AsIs, StartCheck.None, true, TaskPanelExecutionMode.NormalTasks);
                                
                                // Display the system has started in the activity log
                                string msg = DateTime.Now + " " + "The system has started operation";
                                txtV1ExecutionLog.AppendText(msg);
                                txtV1ExecutionLog.AppendText(Environment.NewLine);
                                
                                // Get box IDs from Robotstudio and display in the UI
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

        private void StartDrawingBoxManualMode()
        {
            // Enable next and previous button.
            btnNext.Enabled = true;
            btnPrevious.Enabled = true;

            // Enable stop button.
            btTP2Stop.Enabled = true;

            // Enable reset button.
            btTP2Reset.Enabled = true;

            // Run level 1 for the first time.
            if (!_currentBoxIndexInLayer.Any())
                btnLvl1ControlTab.PerformClick();
        }

        private void ManualReset2FirstItemOnLayer(int layer)
        {
            ResetDrawStateAllItemOnLayer(layer);
            ManualDrawItemInLayer(1, layer);
        }

        private void ResetDrawStateAllItemOnLayer(int layer)
        {
            _currentDrawingBoxState[layer].ForEach(item => item.IsShowing = false);
        }

        private void GenerateLayerListOfPallet(DataTable palletBoxTable)
        {
            // Loop through all record in box table
            foreach (DataRow boxRow in palletBoxTable.Rows)
            {
                // Validate layer number
                if (int.TryParse(boxRow["Layer"].ToString(), out int layer))
                {
                    // Add current box data to layer if exist.
                    if (_currentDrawingBoxState.ContainsKey(layer))
                    {
                        _currentDrawingBoxState[layer].Add(new BoxState
                        {
                            Detail = boxRow,
                            Index = _currentDrawingBoxState[layer].Count + 1
                        });

                        continue;
                    }

                    // Add new layer if not exist.
                    _currentDrawingBoxState.Add(layer, new List<BoxState>
                    {
                        new BoxState
                        {
                            Index = 1,
                            Detail = boxRow
                        }
                    });
                }
                else
                {
                    throw new Exception($"Layer invalid. Box ID: {boxRow["Box ID"]}");
                }
            }
        }


        private void ManualDrawItemInLayer(int itemIndex, int layer)
        {
            _drawingTableControlTab.Clear(Color.White);

            if (!_currentDrawingBoxState.ContainsKey(layer) || itemIndex < 1)
                return;

            _currentDrawingBoxState[layer][itemIndex - 1].IsShowing = true;

            DrawAllShowingItemInLayer(layer);
        }

        private void ManualUnDrawItemInLayer(int itemIndex, int layer)
        {
            _drawingTableControlTab.Clear(Color.White);

            _currentDrawingBoxState[layer][itemIndex - 1].IsShowing = false;

            DrawAllShowingItemInLayer(layer);
        }

        private void DrawAllShowingItemInLayer(int layer)
        {
            foreach (BoxState boxState in _currentDrawingBoxState[layer])
            {
                if (!boxState.IsShowing) continue;

                DrawDataRow(_drawingTableControlTab, boxState.Detail);
            }

            Refresh();
        }

        private void btTP2Stop_Click(object sender, EventArgs e)
        {
            // Enable the buttons for changing robot speed and resetting when the system is stopped
            btTP1RobotSpeed.Enabled = true;
            btTP2Reset.Enabled = true;

            // Disable next and previous button.
            btnNext.Enabled = false;
            btnPrevious.Enabled = false;

            // Enable start button.
            btTP2Start.Enabled = true;

            // Disable stop button
            btTP2Stop.Enabled = false;

            // set control state
            _isControlStop = true;

            // Try to stop the selected task in Robotstudio and display error message if failed
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
                            // Display the system has stopped in the activity log
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
            // Disable the buttons for changing robot speed and starting when the system is reset
            btTP1RobotSpeed.Enabled = false;
            btTP2Start.Enabled = false;

            foreach (int layer in _currentBoxIndexInLayer.Keys.ToList())
            {
                ResetDrawInLayer(layer);
            }

            // Set control state
            _isControlStop = false;

            // Enable stop btn
            btTP2Stop.Enabled = true;

            btnLvl1ControlTab.PerformClick();

            // Toggle action button
            ToggleNavigationBtn();

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
                                // Reset the program pointer back to entry point when starting
                                if (AppControllerData.SelectedTask.ExecutionStatus.Equals(TaskExecutionStatus.Stopped))
                                    AppControllerData.SelectedTask.ResetProgramPointer();
                                // Clear the activity log of the UI
                                txtV1ExecutionLog.Clear();

                                AppControllerData.SelectedController.Rapid.Start(RegainMode.Continue, ExecutionMode.Continuous, ExecutionCycle.AsIs, StartCheck.None, true, TaskPanelExecutionMode.NormalTasks);
                                // Display the system has reset in the activity log
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

        private void ResetDrawInLayer(int layer)
        {
            ManualReset2FirstItemOnLayer(layer);

            _currentBoxIndexInLayer[layer] = 0;
        }

        #endregion ControlPage

        #region StackingPage
        private void ddTP1Pallet_SelectedIndexChanged(object sender, EventArgs e)
        {
            // make sure a pallet is selected before generating 2D image
            if (ddTP1Pallet.SelectedIndex != 0)
            {
                button5.Enabled = true;
            }
            else
            {
                button5.Enabled = false;
            }
            
            // Store the selected type of pallet from drop-down list
            string selectedPallet = ddTP1Pallet.SelectedItem.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Display Add Box window
            FrmAddBox.ShowDialog(this, new FrmAddBox());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Show the data grid view of the customer order
            FrmCustermerDetails.ShowDialog(this, new FrmCustermerDetails());
            // Create new object from StringBuilder which is used to write the filtered list into a new CSV file
            StringBuilder csvfinal = new StringBuilder();
            string csvpath = "C:\\Users\\ASUS\\Desktop\\final_list.csv";
            // Add a header with the column headings
            var header = string.Format("{0}, {1}, {2}, {3}, {4}, {5}", "Box ID", "Length", "Width", "Height", "Weight", "Quantity");
            csvfinal.AppendLine(header);

            using (var reader = new StreamReader("C:\\Users\\ASUS\\Desktop\\customer_order.csv"))
            {
                List<string> listA = new List<string>();
                List<string> listB = new List<string>();
                // Read the customer order from CSV file and seperate values into 2 lists for each line
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    listA.Add(values[0]);
                    listB.Add(values[1]);
                }
                // Compare the customer order with product template and generate a filtered list of product data
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
                
                // Write the filtered list into the new CSV file
                File.WriteAllText(csvpath, string.Empty);
                File.AppendAllText(csvpath, csvfinal.ToString());
                
                // Display the activity of importing customer order in activity log
                string msg = DateTime.Now + " " + "The customer order was imported";
                txtV1ExecutionLog.AppendText(msg);
                txtV1ExecutionLog.AppendText(Environment.NewLine);
            }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            // Disable the button to add new products
            button3.Enabled = false;

            StringBuilder csvcontent = new StringBuilder();
            var header = string.Format("{0}, {1}, {2}, {3}, {4}", "Box ID", "Length", "Width", "Height", "Weight");
            csvcontent.AppendLine(header);
            string csvpath = "C:\\Users\\ASUS\\Desktop\\box_data.csv";
            
            // Clear the file before writing the new product data
            File.WriteAllText(csvpath, string.Empty);
            
            // Write the product data as entered through the UI
            foreach (var item in products)
            {
                var productsResults = string.Format("{0}, {1}, {2}, {3}, {4}", item.box_id, item.box_length, item.box_width, item.box_height, item.box_weight);
                csvcontent.AppendLine(productsResults);
            }
            File.AppendAllText(csvpath, csvcontent.ToString());
            
            // Display the activity of creating a new product list in activity log
            string msg = DateTime.Now + " " + "A list of new products was created";
            txtV1ExecutionLog.AppendText(msg);
            txtV1ExecutionLog.AppendText(Environment.NewLine);
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            // make sure a stacking algorithm is selected before verifying the choice
            if (comboBox4.SelectedIndex != 0)
            {
                // Enable the button to verify the stacking algorithm
                button1.Enabled = true;
            }
            else
            {
                // Disable the button to verify the stacking algorithm
                button1.Enabled = false;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Draw_Graphics(_boxTable, 1);
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

            _boxTable = new ReadCSV(finalListFileDialog.FileName).readCSV;

            // Generate list of layer for Control Tab.
            GenerateLayerListOfPallet(_boxTable);
        }

        private DataTable _boxTable;

        private void button1_Click(object sender, EventArgs e)
        {
            Draw_Graphics(_boxTable, 1);
            Refresh();
        }

        Graphics _drawingTable;

        // drawing table in control tab.
        Graphics _drawingTableControlTab;

        private void Initialize_Graphics(int width, int height)
        {
            // increase length and width by one
            width++;
            height++;

            // set new bitmap to image
            pictureBox2.Image = new Bitmap(width, height);

            // picture box in control tab.
            pictureBox1.Image = new Bitmap(width, height);

            // set new drawing table Graphics
            _drawingTable = Graphics.FromImage(pictureBox2.Image);

            // init drawing table in control tab.
            _drawingTableControlTab = Graphics.FromImage(pictureBox1.Image);

            // clear drawing table
            _drawingTable.Clear(Color.White);

            // clear drawing table in control tab.
            _drawingTableControlTab.Clear(Color.White);
        }

        private void Draw_Graphics(DataTable boxDataTable, int layer)
        {
            // clear drawing table
            _drawingTable.Clear(Color.White);

            foreach (DataRow boxRow in boxDataTable.Rows)
            {
                // Not in current layer, ignore.
                if (!int.Parse(boxRow["Layer"].ToString()).Equals(layer)) continue;
                DrawDataRow(_drawingTable, boxRow);
            }
        }

        private void DrawDataRow(Graphics graphics, DataRow boxRow)
        {
            // Not valid coordinate, ignore.
            if (string.IsNullOrEmpty(boxRow["P1X"].ToString()) || string.IsNullOrEmpty(boxRow["P1Y"].ToString())
                || string.IsNullOrEmpty(boxRow["P2X"].ToString()) || string.IsNullOrEmpty(boxRow["P2Y"].ToString())
                || string.IsNullOrEmpty(boxRow["P3X"].ToString()) || string.IsNullOrEmpty(boxRow["P3Y"].ToString())
                || string.IsNullOrEmpty(boxRow["P4X"].ToString()) || string.IsNullOrEmpty(boxRow["P4Y"].ToString()))
                return;

            Rectangle element = new Rectangle();
            Point indexCoordinates = new Point();
            Point heightCoordinates = new Point();

            Pen pencil = new Pen(Color.Black, 1);
            Brush brush = new SolidBrush(Color.Black);
            Font font = new Font("Lucida", 8, FontStyle.Regular);

            // set element width and heigth
            element.Width = (int.Parse(boxRow["P2X"].ToString()) - int.Parse(boxRow["P1X"].ToString())) * 3;
            element.Height = (int.Parse(boxRow["P3Y"].ToString()) - int.Parse(boxRow["P2Y"].ToString())) * 3;

            // set element lower left corner coordinates
            element.X = (int.Parse(boxRow["P4X"].ToString())) * 3;
            element.Y = PalletHeight - ((int.Parse(boxRow["P4Y"].ToString())) * 3);

            // draw element at DrawingTable
            //_drawingTable.DrawRectangle(pencil, element);

            SolidBrush blueBrush = new SolidBrush(Color.FromArgb(_rnd.Next(256), _rnd.Next(256), _rnd.Next(256)));
            graphics.FillRectangle(blueBrush, element);

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
            graphics.DrawString("ID" + boxRow["Box ID"], font, brush, indexCoordinates);
            graphics.DrawString("H" + boxRow["Height"], font, brush, heightCoordinates, format);

            // release resources
            pencil.Dispose();
            brush.Dispose();
            font.Dispose();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Draw_Graphics(_boxTable, 2);
            Refresh();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Draw_Graphics(_boxTable, 3);
            Refresh();
        }
        #endregion StackingPage

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            // Jump to previous layer if previous btn clicked in first box of layer.
            if (_currentBoxIndexInLayer[_currentLayer] <= 1)
            {
                // Get last valid layer. For case layer list like 1, 3, 4, 6.....
                do
                {
                    _currentLayer -= 1;
                } while (!_currentDrawingBoxState.ContainsKey(_currentLayer) && _currentLayer > 1);

                PerformClickBtnByLayer(_currentLayer);
                return;
            }

            ManualUnDrawItemInLayer(_currentBoxIndexInLayer[_currentLayer], _currentLayer);
            _currentBoxIndexInLayer[_currentLayer] -= 1;

            // Enable next button if not.
            if (!btnNext.Enabled)
                btnNext.Enabled = true;

            // Disable previous btn if current layer is lowest.
            if (_currentLayer <= 1 && _currentBoxIndexInLayer[_currentLayer] <= 1)
                btnPrevious.Enabled = false;
        }

        private void PerformClickBtnByLayer(int layer)
        {
            switch (layer)
            {
                case 1: btnLvl1ControlTab.PerformClick(); break;
                case 2: btnLvl2ControlTab.PerformClick(); break;
                case 3: btnLvl3ControlTab.PerformClick(); break;
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            // Get top layer.
            int maxLayer = _currentDrawingBoxState.Aggregate((l, r) => l.Key > r.Key ? l : r).Key;

            if (_currentBoxIndexInLayer[_currentLayer] >= _currentDrawingBoxState[_currentLayer].Count)
            {
                do
                {
                    _currentLayer += 1;
                } while (!_currentDrawingBoxState.ContainsKey(_currentLayer) && _currentLayer < maxLayer);

                PerformClickBtnByLayer(_currentLayer);
                return;
            }

            _currentBoxIndexInLayer[_currentLayer] += 1;
            ManualDrawItemInLayer(_currentBoxIndexInLayer[_currentLayer], _currentLayer);

            if (!btnPrevious.Enabled)
                btnPrevious.Enabled = true;

            // Disable next btn if current layer is top and all item in current layer is drawn 
            if (_currentLayer == maxLayer && _currentBoxIndexInLayer[_currentLayer] == _currentDrawingBoxState[_currentLayer].Count)
            {
                btnNext.Enabled = false;
            }
        }

        private void ToggleNavigationBtn()
        {
            // If control state is top. Disable all action button.
            if (_isControlStop)
            {
                btnNext.Enabled = false;
                btnPrevious.Enabled = false;

                return;
            }

            // Get top layer.
            int maxLayer = _currentDrawingBoxState.Aggregate((l, r) => l.Key > r.Key ? l : r).Key;

            // Disable next btn if current layer is top and all item in current layer is drawn. Else enable button
            if (_currentLayer == maxLayer && _currentBoxIndexInLayer[_currentLayer] == _currentDrawingBoxState[_currentLayer].Count)
                btnNext.Enabled = false;
            else
                btnNext.Enabled = true;

            // Disable previous btn if current layer is lowest.
            if (_currentLayer <= 1 && _currentBoxIndexInLayer[_currentLayer] <= 1)
                btnPrevious.Enabled = false;
            else
                btnPrevious.Enabled = true;
        }

        private void ActionLevelChange()
        {
            // Keep first box in layer 1 when press level 1, 2, ..... button.
            if (!_currentBoxIndexInLayer.ContainsKey(_currentLayer))
            {
                _currentBoxIndexInLayer.Add(_currentLayer, _currentLayer == 1 ? 1 : 0);

            }

            // Keep first box in layer 1 when press reset button.
            if (_currentLayer == 1 && _currentBoxIndexInLayer[_currentLayer] == 0)
            {
                _currentBoxIndexInLayer[_currentLayer] = 1;
            }

            ManualDrawItemInLayer(_currentBoxIndexInLayer[_currentLayer], _currentLayer);

            ToggleNavigationBtn();

            Refresh();
        }

        private readonly Color _greenDefaultColor = Color.FromArgb(31, 118, 189);
        private void btnLvl1ControlTab_Click(object sender, EventArgs e)
        {
            _currentLayer = 1;

            btnLvl1ControlTab.BackColor = _greenDefaultColor;
            btnLvl2ControlTab.BackColor = Color.Gray;
            btnLvl3ControlTab.BackColor = Color.Gray;

            ActionLevelChange();
            Refresh();
        }

        private void btnLvl2ControlTab_Click(object sender, EventArgs e)
        {
            _currentLayer = 2;

            btnLvl1ControlTab.BackColor = Color.Gray;
            btnLvl2ControlTab.BackColor = _greenDefaultColor;
            btnLvl3ControlTab.BackColor = Color.Gray;

            ActionLevelChange();
            Refresh();
        }

        private void btnLvl3ControlTab_Click(object sender, EventArgs e)
        {
            _currentLayer = 3;

            btnLvl1ControlTab.BackColor = Color.Gray;
            btnLvl2ControlTab.BackColor = Color.Gray;
            btnLvl3ControlTab.BackColor = _greenDefaultColor;

            ActionLevelChange();
            Refresh();
        }
    }
}
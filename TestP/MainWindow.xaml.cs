using ABB.Robotics.Controllers;
using ABB.Robotics.Controllers.Discovery;
using ABB.Robotics.Controllers.RapidDomain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace TestP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public AppControllerData AppControllerData { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            AppControllerData = new AppControllerData(this);
            DataContext = this;

            AppControllerData.FindControllers();
        }
        //
        // Summary:
        //     Searches the network for Robot Controllers.
        #region Event callback methods 
        private void ComboBox_Controllers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox_Tasks.Visibility = Visibility.Visible;
            var comboBoxControllers = sender as ComboBox;
            AppControllerData.SelectedController = Controller.Connect(comboBoxControllers.SelectedItem as ControllerInfo, ConnectionType.Standalone);
            AppControllerData.SelectedController.Logon(UserInfo.DefaultUser);
            AppControllerData.Tasks?.Clear();
            AppControllerData.Modules?.Clear();
            AppControllerData.RapidVariables?.Clear();

            AppControllerData.Tasks = AppControllerData.SelectedController.Rapid.GetTasks().ToList();
        }
        private void ComboBox_Tasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox_Modules.Visibility = Visibility.Visible;
            var comboBoxTasks = sender as ComboBox;
            AppControllerData.SelectedTask = comboBoxTasks.SelectedItem as Task;
            AppControllerData.Modules?.Clear();
            AppControllerData.RapidVariables?.Clear();

            if (AppControllerData.SelectedTask == null)
                return;

            AppControllerData.Modules = AppControllerData.SelectedTask.GetModules().ToList();
        }
        private void ComboBox_Modules_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox_RapidVariables.Visibility = Visibility.Visible;
            var comboBoxModules = sender as ComboBox;
            AppControllerData.SelectedModule = comboBoxModules.SelectedItem as Module;
            AppControllerData.RapidVariables?.Clear();

            if (AppControllerData.SelectedModule == null)
                return;

            AppControllerData.RapidVariables = AppControllerData.SelectedModule.SearchRapidSymbol(RapidSymbolSearchProperties.CreateDefaultForData()).ToList();
        }
        private void ComboBox_RapidVariables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StackPanel_Subscribed.Visibility = Visibility.Hidden;
            Button_Subscribe.Visibility = Visibility.Visible;
            var comboBoxRapidVariables = sender as ComboBox;
            AppControllerData.SelectedRapidVariable = comboBoxRapidVariables.SelectedItem as RapidSymbol;

            if (AppControllerData.SelectedRapidVariable == null)
                return;

            AppControllerData.SelectedRapidData = AppControllerData.SelectedController.Rapid.GetRapidData(AppControllerData.SelectedTask.ToString(), AppControllerData.SelectedModule.ToString(), AppControllerData.SelectedRapidVariable.ToString());
        }
        private void Button_Subscribe_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Width = 750;
            StackPanel_TaskControl.Visibility = Visibility.Visible;
            StackPanel_Subscribed.Visibility = Visibility.Visible;
            textBlockSubscribedRapidVariable.Text = AppControllerData.SelectedRapidVariable.ToString();
            Button_Subscribe.Visibility = Visibility.Hidden;
            StackPanel_Subscribed.Visibility = Visibility.Visible;

            using (Mastership master = Mastership.Request(AppControllerData.SelectedController))
            {
                try
                {
                    AppControllerData.SelectedRapidData.ValueChanged -= Rd_ValueChanged;
                    AppControllerData.SelectedRapidData.ValueChanged += Rd_ValueChanged;
                    master.Release();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unexpected error occurred: " + ex.Message);
                }
            }
        }

        private void Button_View_Pallet_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            // MessageBox.Show("The button labeled has been clicked", "Information Message");
            ViewPalletPatern viewPalletPatern = new ViewPalletPatern();
            viewPalletPatern.Show();

            

        }
        private void Rd_ValueChanged(object sender, DataValueChangedEventArgs e)
        {
            var subscribedRapidVariable = sender as RapidData;
            textBoxSubscribedRapidVariable.Text = subscribedRapidVariable.Value.ToString();
        }
        private void Button_StartTask_Click(object sender, RoutedEventArgs e)
        {
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

                                AppControllerData.SelectedController.Rapid.Start(RegainMode.Continue, ExecutionMode.Continuous, ExecutionCycle.AsIs, StartCheck.None, true, TaskPanelExecutionMode.NormalTasks);
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
        private void Button_StopTask_Click(object sender, RoutedEventArgs e)
        {
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

        #endregion

        private void ComboBox_Controllers_DropDownOpened(object sender, EventArgs e)
        {
            AppControllerData.FindControllers();
        }
    }
}


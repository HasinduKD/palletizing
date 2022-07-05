using ABB.Robotics.Controllers;
using ABB.Robotics.Controllers.Discovery;
using ABB.Robotics.Controllers.RapidDomain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    public class AppControllerData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public FrmMenu Guid { get; set; }
        public AppControllerData(FrmMenu guid)
        {
            Guid = guid;
        }

        private List<ABB.Robotics.Controllers.RapidDomain.Task> _tasks;
        private List<Module> _modules;
        private List<RapidSymbol> _rapidVariables;
        private RapidData _selectedRapidData;

        public ControllerInfoCollection Controllers { get; set; }
        public Controller SelectedController { get; set; }
        public ABB.Robotics.Controllers.RapidDomain.Task SelectedTask { get; set; }
        public Module SelectedModule { get; set; }
        public RapidSymbol SelectedRapidVariable { get; set; }
        public List<ABB.Robotics.Controllers.RapidDomain.Task> Tasks
        {
            get => _tasks;
            set
            {
                _tasks = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Tasks"));
            }
        }
        public List<Module> Modules
        {
            get => _modules;
            set
            {
                _modules = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Modules"));
            }
        }
        public List<RapidSymbol> RapidVariables
        {
            get => _rapidVariables;
            set
            {
                _rapidVariables = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RapidVariables"));
            }
        }
        public RapidData SelectedRapidData
        {
            get => _selectedRapidData;
            set
            {
                _selectedRapidData = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedRapidData"));
            }
        }

        public void FindControllers()
        {
            var scanner = new NetworkScanner();
            scanner.Scan();
            Controllers = scanner.Controllers;
        }
    }
}
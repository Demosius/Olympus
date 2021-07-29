using Olympus.Torch.View;
using Olympus.Uranus.Staff;
using Olympus.Pantheon.View;
using Olympus.Prometheus.View;
using Olympus.Vulcan.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Olympus.Khaos.View;
using Olympus.ViewModel.Components;

namespace Olympus.ViewModel
{
    public class OlympusVM : INotifyPropertyChanged
    {
        public PrometheusPage Prometheus { get; set; }
        public PantheonPage Pantheon { get; set; }
        public VulcanPage Vulcan { get; set; }
        public TorchPage Torch { get; set; }
        public KhaosPage Khaos { get; set; }

        public EProject CurrentProject { get; set; }

        private Page currentPage;
        public Page CurrentPage 
        {
            get => currentPage;
            set
            {
                currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
            }
        }
        
        /* Sub ViewModels - Components */
        public DBSelectionVM DBSelectionVM { get; set; }
        public InventoryUpdaterVM InventoryUpdaterVM { get; set; }
        public ProjectLauncherVM ProjectLauncherVM { get; set; }
        public UserHandlerVM UserHandlerVM { get; set; }

        /* Commands */

        /* Constructor(s) */
        public OlympusVM()
        {
            DBSelectionVM = new DBSelectionVM(this);
            UserHandlerVM = new UserHandlerVM(this);
            ProjectLauncherVM = new ProjectLauncherVM(this);
            InventoryUpdaterVM = new InventoryUpdaterVM(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        /* Projects */
        public void LoadProject(EProject project)
        {
            switch (project) 
            {
                case EProject.Vulcan:
                    LoadVulcan();
                    break;
                case EProject.Prometheus:
                    LoadPrometheus();
                    break;
                case EProject.Pantheon:
                    LoadPantheon();
                    break;
                case EProject.Torch:
                    LoadTorch();
                    break;
                case EProject.Khaos:
                    LoadKhaos();
                    break;
                default:
                    break;
            }
            CurrentProject = project;
        }

        private void LoadPrometheus()
        {
            if (Prometheus is null) Prometheus = new PrometheusPage();
            CurrentPage = Prometheus;
        }

        private void LoadPantheon()
        {
            if (Pantheon is null) Pantheon = new PantheonPage();
            CurrentPage = Pantheon;
        }

        private void LoadVulcan()
        {
            if (Vulcan is null) Vulcan = new VulcanPage();
            CurrentPage = Vulcan;
        }

        private void LoadTorch()
        {
            if (Torch is null) Torch = new TorchPage();
            CurrentPage = Torch;
        }

        private void LoadKhaos()
        {
            if (Khaos is null) Khaos = new KhaosPage();
            CurrentPage = Khaos;
        }
    }
}

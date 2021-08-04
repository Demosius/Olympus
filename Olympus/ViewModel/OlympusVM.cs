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
using Olympus.Model;

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
                if (!(value is null)) CurrentProject = (value as IProject).EProject;
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
        }

        private void LoadPrometheus()
        {
            if (Prometheus is null) Prometheus = new PrometheusPage();
            SetPage(Prometheus);
        }

        private void LoadPantheon()
        {
            if (Pantheon is null) Pantheon = new PantheonPage();
            SetPage(Pantheon);
        }

        private void LoadVulcan()
        {
            if (Vulcan is null) Vulcan = new VulcanPage();
            SetPage(Vulcan);
        }

        private void LoadTorch()
        {
            if (Torch is null) Torch = new TorchPage();
            SetPage(Torch);
        }

        private void LoadKhaos()
        {
            if (Khaos is null) Khaos = new KhaosPage();
            SetPage(Khaos);
        }

        private void SetPage(IProject project)
        {
            Page page = project as Page;
            if (CurrentPage is null)
                CurrentPage = page;
            else
                CurrentPage.NavigationService.Navigate(page);
        }
    }
}

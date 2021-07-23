using Olympus.EverBurn.View;
using Olympus.Helios.Staff;
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

namespace Olympus.ViewModel
{
    public class OlympusVM : INotifyPropertyChanged
    {
        public PrometheusPage Prometheus { get; set; }
        public PantheonPage Pantheon { get; set; }
        public VulcanPage Vulcan { get; set; }
        public EverBurnPage EverBurn { get; set; }

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


        /* Commands */

        /* Constructor(s) */
        public OlympusVM()
        {
            Prometheus = new PrometheusPage();
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
                case EProject.EverBurn:
                    LoadEverBurn();
                    break;
                default:
                    break;
            }
        }

        public void LoadPrometheus()
        {
            if (Prometheus is null) Prometheus = new PrometheusPage();
            CurrentPage = Prometheus;
        }

        public void LoadPantheon()
        {
            if (Pantheon is null) Pantheon = new PantheonPage();
            CurrentPage = Prometheus;
        }

        public void LoadVulcan()
        {
            if (Vulcan is null) Vulcan = new VulcanPage();
            CurrentPage = Vulcan;
        }

        public void LoadEverBurn()
        {
            if (EverBurn is null) EverBurn = new EverBurnPage();
            CurrentPage = Vulcan;
        }
    }
}

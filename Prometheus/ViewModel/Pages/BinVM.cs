using Uranus;
using Uranus.Inventory.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Prometheus.ViewModel.Pages
{
    public class BinVM : INotifyPropertyChanged
    {
        private List<NAVBin> bins;
        public List<NAVBin> Bins
        {
            get => bins;
            set
            {
                bins = value;
                OnPropertyChanged(nameof(Bins));
            }
        }
        private ObservableCollection<NAVBin> displayBins;
        public ObservableCollection<NAVBin> DisplayBins
        {
            get => displayBins;
            set 
            { 
                displayBins = value;
                OnPropertyChanged(nameof(DisplayBins));
            }
        }
        public NAVBin SelectedBin { get; set; }

        private string binFilter;
        public string BinFilter 
        {
            get => binFilter;
            set
            {
                binFilter = value;
                OnPropertyChanged(nameof(BinFilter));
                _ = Task.Run(ApplyFilter);
            }
        }

        public BinVM()
        {
            if (DesignerProperties.GetIsInDesignMode(new()))
            {
                Bins = new();
                DisplayBins = new(Bins);
            }
            else
            {
                _ = Task.Run(SetBins);
            }
        }

        private void ApplyFilter()
        {
            if ((BinFilter ?? "") == "")
                DisplayBins = new(Bins);
            else
                DisplayBins = new(Bins.Where(b => b.Code.ToLower().Contains(BinFilter.ToLower())).ToList());
        }

        private void SetBins()
        {
            Bins = App.Helios.InventoryReader.NAVBins(pullType: EPullType.ObjectOnly);
            DisplayBins = new(Bins);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }
    }
}

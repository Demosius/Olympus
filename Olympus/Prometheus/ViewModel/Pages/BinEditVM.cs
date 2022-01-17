using Uranus.Inventory.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Prometheus.ViewModel.Pages
{
    class BinEditVM : INotifyPropertyChanged
    {

        public List<string> ExistingBinIDs { get; set; }

        private BinVM parentVM;
        public BinVM ParentVM
        {
            get => parentVM;
            set 
            {
                parentVM = value;
                ExistingBinIDs = parentVM.Bins.Select(b => b.ID).ToList();
                OnPropertyChanged(nameof(ParentVM));
            }
        }

        private NAVBin bin;
        public NAVBin Bin
        {
            get => bin;
            set
            {
                bin = value;
                OnPropertyChanged(nameof(Bin));
            }
        }


        public BinEditVM() { }

        /// <summary>
        /// Saves the bin changes into the parent bin list, and loads it into the database.
        /// </summary>
        public static void SaveChanges()
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

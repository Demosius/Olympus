using System.ComponentModel;
using Aion.Properties;
using Aion.ViewModel.Utility;

namespace Aion.ViewModel
{
    public class AppVM : INotifyPropertyChanged
    {
        public DBManager DBManager { get; set; }

        public AppVM()
        {
            DBManager = new(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }

        public void ResetDB()
        {
            App.Helios.ResetChariots(Settings.Default.SolLocation);
            App.Charon.DatabaseReset(Settings.Default.SolLocation);
        }
    }
}

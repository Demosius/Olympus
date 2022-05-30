using Aion.Properties;
using Aion.View;
using Aion.ViewModels.Utility;
using System.ComponentModel;

namespace Aion.ViewModels;

public class AppVM : INotifyPropertyChanged
{
    public DBManager DBManager { get; set; }
    public AionPage AionPage { get; set; }

    public AppVM()
    {
        DBManager = new DBManager(this);
        AionPage = new AionPage(App.Helios, App.Charon);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void ResetDB()
    {
        App.Helios.ResetChariots(Settings.Default.SolLocation);
        App.Charon.DatabaseReset(Settings.Default.SolLocation);
    }
}
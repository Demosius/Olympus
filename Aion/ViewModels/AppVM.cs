using Aion.Properties;
using Aion.View;
using Aion.ViewModels.Utility;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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
    
    public static void ResetDB()
    {
        App.Helios.ResetChariots(Settings.Default.SolLocation);
        App.Charon.DatabaseReset(Settings.Default.SolLocation);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [Uranus.Annotations.NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
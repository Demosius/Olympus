using Sphynx.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus.Annotations;

namespace Sphynx.ViewModels;

public class AppVM : INotifyPropertyChanged
{
    public SphynxPage SphynxPage { get; set; }

    public AppVM()
    {
        SphynxPage = new SphynxPage(App.Helios, App.Charon, App.ProgressBar);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
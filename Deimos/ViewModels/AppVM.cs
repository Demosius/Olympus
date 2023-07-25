using System.ComponentModel;
using System.Runtime.CompilerServices;
using Deimos.Views;
using Uranus.Annotations;

namespace Deimos.ViewModels;

public class AppVM : INotifyPropertyChanged
{
    public DeimosPage DeimosPage { get; set; }

    public AppVM()
    {
        DeimosPage = new DeimosPage(App.Helios, App.ProgressBar);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
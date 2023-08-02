using System.ComponentModel;
using System.Runtime.CompilerServices;
using Hermes.Views;
using Uranus.Annotations;

namespace Hermes.ViewModels;

public class AppVM : INotifyPropertyChanged
{
    public HermesPage HermesPage { get; set; }

    public AppVM()
    {
        HermesPage = new HermesPage(App.Helios, App.Charon);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
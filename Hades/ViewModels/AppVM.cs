using Hades.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus.Annotations;

namespace Hades.ViewModels;

public class AppVM : INotifyPropertyChanged
{
    public HadesPage HadesPage { get; set; }

    public AppVM()
    {
        HadesPage = new HadesPage(App.Helios);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
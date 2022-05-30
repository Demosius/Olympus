using Hydra.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus.Annotations;

namespace Hydra.ViewModels;

public class AppVM : INotifyPropertyChanged
{
    public HydraPage HydraPage { get; set; }

    public AppVM()
    {
        HydraPage = new HydraPage(App.Helios, App.Charon);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
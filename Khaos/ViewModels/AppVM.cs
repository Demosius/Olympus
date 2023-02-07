using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus.Annotations;

namespace Khaos.ViewModels;

public class AppVM : INotifyPropertyChanged
{
    public KhaosVM KhaosVM { get; set; }

    public AppVM()
    {
        KhaosVM = new KhaosVM(App.Helios);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
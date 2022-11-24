using Panacea.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus.Annotations;

namespace Panacea.ViewModels;

public class AppVM : INotifyPropertyChanged
{
    public PanaceaPage PanaceaPage { get; set; }

    public AppVM()
    {
        PanaceaPage = new PanaceaPage(App.Helios);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
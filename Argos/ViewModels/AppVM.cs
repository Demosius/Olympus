using Argos.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus.Annotations;

namespace Argos.ViewModels;

public class AppVM : INotifyPropertyChanged
{
    public ArgosPage ArgosPage { get; set; }

    public AppVM()
    {
        ArgosPage = new ArgosPage(App.Helios);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
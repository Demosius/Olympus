using Panacea.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus.Annotations;

namespace Panacea.ViewModels;

public class AppVM : INotifyPropertyChanged
{
    private PanaceaPage panaceaPage;
    public PanaceaPage PanaceaPage
    {
        get => panaceaPage;
        set
        {
            panaceaPage = value;
            OnPropertyChanged();
        }
    }

    public AppVM()
    {
        panaceaPage = new PanaceaPage(App.Helios);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
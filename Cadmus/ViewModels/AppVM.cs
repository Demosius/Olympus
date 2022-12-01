using Cadmus.Annotations;
using Cadmus.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Cadmus.ViewModels;

internal class AppVM : INotifyPropertyChanged
{
    private CadmusPage? cadmusPage;
    public CadmusPage? CadmusPage
    {
        get => cadmusPage;
        set
        {
            cadmusPage = value;
            OnPropertyChanged();
        }
    }

    public AppVM()
    {
        CadmusPage = new CadmusPage();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
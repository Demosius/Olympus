using Pantheon.Views;
using Styx;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus;

namespace Pantheon.ViewModels;

internal class AppVM : INotifyPropertyChanged
{
    public Charon? Charon { get; set; }
    public Helios? Helios { get; set; }

    private PantheonPage? pantheonPage;
    public PantheonPage? PantheonPage
    {
        get => pantheonPage;
        set
        {
            pantheonPage = value;
            OnPropertyChanged(nameof(PantheonPage));
        }
    }

    public AppVM()
    {
        PantheonPage = null;
    }

    public void SetDataSources(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;
        PantheonPage = new PantheonPage(charon, helios);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
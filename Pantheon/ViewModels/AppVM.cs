using Pantheon.Views;
using Styx;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Morpheus.ViewModels.Controls;
using Uranus;

namespace Pantheon.ViewModels;

public class AppVM : INotifyPropertyChanged
{
    public Charon Charon { get; set; }
    public Helios Helios { get; set; }
    public ProgressBarVM ProgressBar { get; set; }

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
        Helios = App.Helios;
        Charon = App.Charon;
        ProgressBar = App.ProgressBar;

        PantheonPage = new PantheonPage(Helios, Charon, ProgressBar);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
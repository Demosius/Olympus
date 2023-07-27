using Styx;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Morpheus.ViewModels.Controls;
using Sphynx.ViewModels.Controls;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;

namespace Sphynx.ViewModels;

public class SphynxVM : INotifyPropertyChanged, IDBInteraction
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }
    public ProgressBarVM ProgressBar { get; set; }

    public AutoCounterVM AutoCounter { get; set; }

    #region INotififyPropertyChanged Members



    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }

    #endregion

    public SphynxVM(Helios helios, Charon charon, ProgressBarVM progressBar)
    {
        Helios = helios;
        Charon = charon;
        ProgressBar = progressBar;

        AutoCounter = new AutoCounterVM(helios, progressBar);

        RefreshDataCommand = new RefreshDataCommand(this);
    }

    public async Task RefreshDataAsync()
    {
        await AutoCounter.RefreshDataAsync();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
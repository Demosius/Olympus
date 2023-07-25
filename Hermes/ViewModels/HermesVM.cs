using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Hermes.ViewModels.Controls;
using Styx;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;

namespace Hermes.ViewModels;

public class HermesVM : INotifyPropertyChanged, IDBInteraction
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public AutoCounterVM AutoCounter { get; set; }

    #region INotifypropertyChanged Members
    

    #endregion

    #region Commands
    
    public RefreshDataCommand RefreshDataCommand { get; set; }

    #endregion

    public HermesVM(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        AutoCounter = new AutoCounterVM(helios);

        RefreshDataCommand = new RefreshDataCommand(this);
    }

    public Task RefreshDataAsync()
    {
        throw new System.NotImplementedException();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
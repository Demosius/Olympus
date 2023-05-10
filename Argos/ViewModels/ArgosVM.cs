using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;

namespace Argos.ViewModels;

public class ArgosVM : INotifyPropertyChanged, IDBInteraction
{
    public Helios Helios { get; set; }

    #region INotifyPropertyChanged Members



    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }

    #endregion

    public ArgosVM(Helios helios)
    {
        Helios = helios;

        RefreshDataCommand = new RefreshDataCommand(this);
    }

    public async Task RefreshDataAsync()
    {
        await new Task(() => { });
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
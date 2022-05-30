using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus.Annotations;
using Uranus.Inventory.Models;

namespace Hydra.ViewModels.Controls;

public class ZoneVM : INotifyPropertyChanged
{
    public NAVZone Zone { get; set; }

    public string Code => Zone.Code;
    public string LocationCode => Zone.LocationCode;
    public string ID => Zone.ID;

    #region INotifyPropertyChanged Members



    #endregion

    public ZoneVM(NAVZone zone)
    {
        Zone = zone;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
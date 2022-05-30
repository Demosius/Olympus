using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus.Annotations;
using Uranus.Inventory.Models;

namespace Hydra.ViewModels.Controls;

public class SiteVM : INotifyPropertyChanged
{
    public Site Site { get; set; }

    #region INotifyPropertyChanged Members

    public ObservableCollection<ZoneVM> Zones { get; set; }

    #endregion

    public SiteVM(Site site)
    {
        Site = site;
        Zones = new ObservableCollection<ZoneVM>();

    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
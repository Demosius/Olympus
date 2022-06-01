using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
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

    private SiteVM? siteVM;
    public string SiteName => siteVM?.Site.Name ?? "";
    public Site? Site => siteVM?.Site;
    public SiteVM? SiteVM
    {
        get => siteVM;
        set
        {
            siteVM = value;
            Zone.Site = value?.Site;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Site));
            OnPropertyChanged(nameof(SiteName));
        }
    }

    #endregion

    public ZoneVM(NAVZone zone)
    {
        Zone = zone;
    }

    public void Remove()
    {
        SiteVM?.RemoveZone(this);
    }

    public void Add()
    {
        SiteVM?.AddZone(this);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public override string ToString()
    {
        return $"{Zone.Code} ({Zone.LocationCode})";
    }
}
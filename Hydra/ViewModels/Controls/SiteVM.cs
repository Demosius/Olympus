using Hydra.ViewModels.Commands;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Uranus.Annotations;
using Uranus.Inventory.Models;

namespace Hydra.ViewModels.Controls;

public class SiteVM : INotifyPropertyChanged
{
    public Site Site { get; set; }

    public ZoneListingVM ZoneListingVM { get; set; }

    #region INotifyPropertyChanged Members

    public string MinUnits
    {
        get => Site.MinUnits?.ToString() ?? "";
        set
        {
            if (value != "" && int.TryParse(value, out var minUnits) && minUnits >= 0)
                Site.MinUnits = minUnits;
            else
                Site.MinUnits = null;
            OnPropertyChanged();
        }
    }

    public string MaxUnits
    {
        get => Site.MaxUnits?.ToString() ?? "";
        set
        {
            if (value != "" && int.TryParse(value, out var maxUnits) && maxUnits >= 0)
                Site.MaxUnits = maxUnits;
            else
                Site.MaxUnits = null;
            OnPropertyChanged();
        }
    }

    public string MinCases
    {
        get => Site.MinCases?.ToString() ?? "";
        set
        {
            if (value != "" && int.TryParse(value, out var minCases) && minCases >= 0)
                Site.MinCases = minCases;
            else
                Site.MinCases = null;
            OnPropertyChanged();
        }
    }

    public string MaxCases
    {
        get => Site.MaxCases?.ToString() ?? "";
        set
        {
            if (value != "" && int.TryParse(value, out var maxCases) && maxCases >= 0)
                Site.MaxCases = maxCases;
            else
                Site.MaxCases = null;
            OnPropertyChanged();
        }
    }

    public string MinPct
    {
        get => Site.MinPct is null 
            ? ""
            : ((Site.MinPct ?? 0) * 100).ToString(CultureInfo.CurrentCulture);
        set
        {
            if (value == "" || !float.TryParse(value, NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out var minPct))
                Site.MinPct = null;
            else
            {
                if (minPct < 0) minPct = 0;
                while (minPct > 1) minPct /= 100;

                Site.MinPct = minPct;
            }
            OnPropertyChanged();
        }
    }

    public string MaxPct
    {
        get => Site.MaxPct is null      
            ? "" 
            : ((Site.MaxPct ?? 0) * 100).ToString(CultureInfo.CurrentCulture);
        set
        {
            if (value == "" || !float.TryParse(value, NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out var maxPct))
                Site.MaxPct = null;
            else
            {
                if (maxPct < 0) maxPct = 0;
                while (maxPct > 1) maxPct /= 100;

                Site.MaxPct = maxPct;
            }
            OnPropertyChanged();
        }
    }

    #endregion

    public DeleteSiteCommand DeleteSiteCommand { get; set; }

    public SiteVM(Site site, DeleteSiteCommand deleteSiteCommand)
    {
        Site = site;
        ZoneListingVM = new ZoneListingVM(site.Zones, this);
        DeleteSiteCommand = deleteSiteCommand;
    }

    public void RemoveZone(ZoneVM zoneVM)
    {
        Site.RemoveZone(zoneVM.Zone);
        ZoneListingVM.RemoveZone(zoneVM);
    }

    public void AddZone(ZoneVM zoneVM)
    {
        Site.AddZone(zoneVM.Zone);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
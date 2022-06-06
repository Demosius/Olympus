using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Uranus.Annotations;
using Uranus.Inventory.Models;

namespace Hydra.ViewModels.Controls;

public class SiteItemLevelVM : INotifyPropertyChanged
{
    public SiteItemLevel SiteItemLevel { get; set; }

    public int ItemNumber => SiteItemLevel.ItemNumber;
    public string SiteName => SiteItemLevel.SiteName;

    public Site? Site => SiteItemLevel.Site;
    public NAVItem? Item => SiteItemLevel.Item;

    public SiteVM? SiteVM { get; set; }
    public ItemVM? ItemVM { get; set; }

    #region INotifyPropertyChanged Members

    public bool Active
    {
        get => SiteItemLevel.Active;
        set
        {
            SiteItemLevel.Active = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(SetMinMax));
            OnPropertyChanged(nameof(ToString));
        }
    }

    public bool OverrideDefaults
    {
        get => SiteItemLevel.OverrideDefaults;
        set
        {
            SiteItemLevel.OverrideDefaults = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(SetMinMax));
            OnPropertyChanged(nameof(ToString));
        }
    }

    public bool SetMinMax => Active && OverrideDefaults;

    public string MinUnits
    {
        get => SiteItemLevel.MinUnits?.ToString() ?? "";
        set
        {
            if (value != "" && int.TryParse(value, out var minUnits) && minUnits >= 0)
                SiteItemLevel.MinUnits = minUnits;
            else
                SiteItemLevel.MinUnits = null;
            OnPropertyChanged();
        }
    }

    public string MaxUnits
    {
        get => SiteItemLevel.MaxUnits?.ToString() ?? "";
        set
        {
            if (value != "" && int.TryParse(value, out var maxUnits) && maxUnits >= 0)
                SiteItemLevel.MaxUnits = maxUnits;
            else
                SiteItemLevel.MaxUnits = null;
            OnPropertyChanged();
        }
    }

    public string MinCases
    {
        get => SiteItemLevel.MinCases?.ToString() ?? "";
        set
        {
            if (value != "" && int.TryParse(value, out var minCases) && minCases >= 0)
                SiteItemLevel.MinCases = minCases;
            else
                SiteItemLevel.MinCases = null;
            OnPropertyChanged();
        }
    }

    public string MaxCases
    {
        get => SiteItemLevel.MaxCases?.ToString() ?? "";
        set
        {
            if (value != "" && int.TryParse(value, out var maxCases) && maxCases >= 0)
                SiteItemLevel.MaxCases = maxCases;
            else
                SiteItemLevel.MaxCases = null;
            OnPropertyChanged();
        }
    }

    public string MinPct
    {
        get => SiteItemLevel.MinPct is null
            ? ""
            : ((SiteItemLevel.MinPct ?? 0) * 100).ToString(CultureInfo.CurrentCulture);
        set
        {
            if (value == "" || !float.TryParse(value, NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out var minPct))
                SiteItemLevel.MinPct = null;
            else
            {
                if (minPct < 0) minPct = 0;
                while (minPct > 1) minPct /= 100;

                SiteItemLevel.MinPct = minPct;
            }
            OnPropertyChanged();
        }
    }

    public string MaxPct
    {
        get => SiteItemLevel.MaxPct is null
            ? ""
            : ((SiteItemLevel.MaxPct ?? 0) * 100).ToString(CultureInfo.CurrentCulture);
        set
        {
            if (value == "" || !float.TryParse(value, NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out var maxPct))
                SiteItemLevel.MaxPct = null;
            else
            {
                if (maxPct < 0) maxPct = 0;
                while (maxPct > 1) maxPct /= 100;

                SiteItemLevel.MaxPct = maxPct;
            }
            OnPropertyChanged();
        }
    }
    #endregion

    public SiteItemLevelVM(SiteItemLevel siteItemLevel)
    {
        SiteItemLevel = siteItemLevel;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public override string ToString()
    {
        return SiteItemLevel.ToString();
    }
}
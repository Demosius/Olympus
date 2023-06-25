using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Cadmus.Annotations;
using Cadmus.Properties;
using Uranus;
using Uranus.Commands;
using Uranus.Interfaces;

namespace Cadmus.ViewModels.Controls;

public class MixedCartonSOH_VM : INotifyPropertyChanged, IDBInteraction, IFilters
{
    public Helios Helios { get; set; }

    #region INotifyPropertyChanged Members

    private bool autoDetect;
    public bool AutoDetect
    {
        get => autoDetect;
        set
        {
            autoDetect = value;
            OnPropertyChanged();
        }
    }
    
    public string ZoneString
    {
        get => Settings.Default.MCZoneString;
        set
        {
            Settings.Default.MCZoneString = value;
            OnPropertyChanged();
            Settings.Default.Save();
        }
    }
    
    public string LocationString
    {
        get => Settings.Default.MCLocationString;
        set
        {
            Settings.Default.MCLocationString = value;
            OnPropertyChanged();
            Settings.Default.Save();
        }
    }

    private string zoneFilter;
    public string ZoneFilter
    {
        get => zoneFilter;
        set
        {
            zoneFilter = value;
            OnPropertyChanged();
        }
    }

    private string platformFilter;
    public string PlatformFilter
    {
        get => platformFilter;
        set
        {
            platformFilter = value;
            OnPropertyChanged();
        }
    }

    private string categoryFilter;
    public string CategoryFilter
    {
        get => categoryFilter;
        set
        {
            categoryFilter = value;
            OnPropertyChanged();
        }
    }

    private string divisionFilter;
    public string DivisionFilter
    {
        get => divisionFilter;
        set
        {
            divisionFilter = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }

    #endregion

    public MixedCartonSOH_VM(Helios helios)
    {
        Helios = helios;

        zoneFilter = string.Empty;
        platformFilter = string.Empty; 
        divisionFilter = string.Empty;
        categoryFilter = string.Empty;

        RefreshDataCommand = new RefreshDataCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
    }

    public async Task RefreshDataAsync()
    {
        var dsa = await Helios.InventoryReader.MixedCartonStockAsync(ZoneString.Split('|'), LocationString.Split('|'));
    }

    public void ClearFilters()
    {
        ZoneFilter = string.Empty;
        PlatformFilter = string.Empty;
        DivisionFilter = string.Empty;
        CategoryFilter = string.Empty;
        ApplyFilters();
    }

    public void ApplyFilters()
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
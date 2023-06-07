using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Cadmus.Annotations;
using Uranus;
using Uranus.Commands;
using Uranus.Interfaces;

namespace Cadmus.ViewModels.Controls;

public class MixedCartonSOH_VM : INotifyPropertyChanged, IDBInteraction, IFilters
{
    public Helios Helios { get; set; }

    #region INotifyPropertyChanged Members

    private string zoneString;
    public string ZoneString
    {
        get => zoneString;
        set
        {
            zoneString = value;
            OnPropertyChanged();
        }
    }

    private string platformString;
    public string PlatformString
    {
        get => platformString;
        set
        {
            platformString = value;
            OnPropertyChanged();
        }
    }

    private string categoryString;
    public string CategoryString
    {
        get => categoryString;
        set
        {
            categoryString = value;
            OnPropertyChanged();
        }
    }

    private string divisionString;
    public string DivisionString
    {
        get => divisionString;
        set
        {
            divisionString = value;
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

        zoneString = string.Empty;
        platformString = string.Empty; 
        divisionString = string.Empty;
        categoryString = string.Empty;

        RefreshDataCommand = new RefreshDataCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
    }

    
    public async Task RefreshDataAsync()
    {
        
    }

    public void ClearFilters()
    {
        ZoneString = string.Empty;
        PlatformString = string.Empty;
        DivisionString = string.Empty;
        CategoryString = string.Empty;
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
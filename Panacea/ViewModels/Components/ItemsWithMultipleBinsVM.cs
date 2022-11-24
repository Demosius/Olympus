using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Panacea.Properties;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;

namespace Panacea.ViewModels.Components;

public class ItemsWithMultipleBinsVM : INotifyPropertyChanged, IFilters
{
    public Helios Helios { get; set; }

    #region INotifyPropertyChanged Members


    private string checkZoneString;
    public string CheckZoneString
    {
        get => checkZoneString;
        set
        {
            checkZoneString = value;
            OnPropertyChanged();
            Settings.Default.IWMBZones = value;
            Settings.Default.Save();
        }
    }

    #endregion

    #region Commands

    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }
    public ApplySortingCommand ApplySortingCommand { get; set; }

    #endregion


    public ItemsWithMultipleBinsVM(Helios helios)
    {
        Helios = helios;

        checkZoneString = Settings.Default.IWMBZones;

        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        ApplySortingCommand = new ApplySortingCommand(this);
    }

    public void ClearFilters()
    {
        throw new NotImplementedException();
    }

    public void ApplyFilters()
    {
        throw new NotImplementedException();
    }

    public void ApplySorting()
    {
        throw new NotImplementedException();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
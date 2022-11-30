using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Panacea.Interfaces;
using Panacea.ViewModels.Commands;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;

namespace Panacea.ViewModels.Components;

public class PotentialReplenishmentVM : INotifyPropertyChanged, IFilters, IBinData, IItemData
{

    #region Commands

    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }
    public ApplySortingCommand ApplySortingCommand { get; set; }
    public BinsToClipboardCommand BinsToClipboardCommand { get; set; }
    public ItemsToClipboardCommand ItemsToClipboardCommand { get; set; }

    #endregion

    public void BinsToClipboard()
    {
        throw new NotImplementedException();
    }

    public void ItemsToClipboard()
    {
        throw new NotImplementedException();
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

    public void RunPotentialNegativesCheck()
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
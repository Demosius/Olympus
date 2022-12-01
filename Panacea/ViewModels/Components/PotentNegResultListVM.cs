using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Panacea.Interfaces;
using Panacea.Models;
using Panacea.Properties;
using Panacea.ViewModels.Commands;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;

namespace Panacea.ViewModels.Components;

public class PotentNegResultListVM : INotifyPropertyChanged, IFilters, IBinData, IItemData
{
    public List<PotentNegCheckResult> CheckResults { get; set; }
    public string Header { get; set; }

    #region INotifyPropertyChanged Members
    
    private ObservableCollection<PotentNegCheckResult> filteredCheckResults;
    public ObservableCollection<PotentNegCheckResult> FilteredCheckResults
    {
        get => filteredCheckResults;
        set
        {
            filteredCheckResults = value;
            OnPropertyChanged();
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
            ApplyFilters();
        }
    }

    #endregion

    #region Commands

    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }
    public ApplySortingCommand ApplySortingCommand { get; set; }
    public BinsToClipboardCommand BinsToClipboardCommand { get; set; }
    public ItemsToClipboardCommand ItemsToClipboardCommand { get; set; }

    #endregion

    public PotentNegResultListVM(string header)
    {
        Header = header;

        CheckResults = new List<PotentNegCheckResult>();

        filteredCheckResults = new ObservableCollection<PotentNegCheckResult>();

        zoneFilter = string.Empty;

        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        ApplySortingCommand = new ApplySortingCommand(this);
        BinsToClipboardCommand = new BinsToClipboardCommand(this);
        ItemsToClipboardCommand = new ItemsToClipboardCommand(this);
    }

    public void BinsToClipboard()
    {
        var binList = FilteredCheckResults.Select(checkResult => checkResult.Bin).ToList();
        Clipboard.SetText(string.Join("|", binList));
        MessageBox.Show($"{binList.Count} bins added to clipboard.");
    }

    public void ItemsToClipboard()
    {
        var binList = FilteredCheckResults.Select(checkResult => checkResult.Item).ToList();
        Clipboard.SetText(string.Join("|", binList));
        MessageBox.Show($"{binList.Count} items added to clipboard.");
    }

    public void ClearFilters()
    {
        ZoneFilter = string.Empty;
        ApplyFilters();
    }

    public void ApplyFilters()
    {
        IEnumerable<PotentNegCheckResult> results = CheckResults;

        if (ZoneFilter != string.Empty)
        {
            var regex = new Regex(ZoneFilter, RegexOptions.IgnoreCase);
            results = results.Where(res => regex.IsMatch(res.Zone));
        }

        FilteredCheckResults = new ObservableCollection<PotentNegCheckResult>(results);
    }

    public void ApplySorting()
    {
        throw new NotImplementedException();
    }

    public void SetResults(List<PotentNegCheckResult> newCheckResults)
    {
        CheckResults = newCheckResults;

        ApplyFilters();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
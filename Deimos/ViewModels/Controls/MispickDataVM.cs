using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Morpheus.ViewModels.Controls;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;

namespace Deimos.ViewModels.Controls;

public class MispickDataVM : INotifyPropertyChanged, IDBInteraction, IFilters
{
    public DeimosVM Deimos { get; set; }
    public ErrorAllocationVM ParentVM { get; set; }
    public Helios Helios { get; set; }
    public ProgressBarVM ProgressBar { get; set; }

    public List<MispickVM> AllMispicks { get; set; }

    #region ParentVM Access

    public DateTime? StartDate => ParentVM.StartDate;
    public DateTime? EndDate => ParentVM.EndDate;
     
    #endregion

    #region INotifyPropertyChanged Members

    public ObservableCollection<MispickVM> Mispicks { get; set; }

    private string filterString;
    public string FilterString
    {
        get => filterString;
        set
        {
            filterString = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }

    #endregion

    public MispickDataVM(ErrorAllocationVM parentVM)
    {
        ParentVM = parentVM;
        Deimos = ParentVM.ParentVM;
        Helios = parentVM.Helios;
        ProgressBar = parentVM.ProgressBar;

        filterString = string.Empty;

        AllMispicks = new List<MispickVM>();

        Mispicks = new ObservableCollection<MispickVM>();

        RefreshDataCommand = new RefreshDataCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
    }

    public async Task RefreshDataAsync()
    {
        ProgressBar.StartTask("Gathering Mispick Data...");
        if (StartDate is null || EndDate is null)
            AllMispicks = new List<MispickVM>();
        else
            AllMispicks = (await Helios.StaffReader.RawMispicksAsync((DateTime) StartDate, (DateTime) EndDate)).Select(mp => new MispickVM(mp, Helios)).ToList();

        ApplyFilters();
        ProgressBar.EndTask();
    }

    public void ClearFilters()
    {
        FilterString = string.Empty;
        ApplyFilters();
    }

    public void ApplyFilters()
    {
        var events = AllMispicks.Where(mispick =>
            Regex.IsMatch(mispick.AssignedRF_ID, FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(mispick.ActionNotes, FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(mispick.Comments, FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(mispick.ItemDescription, FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(mispick.ItemNumber.ToString(), FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(mispick.CartonID, FilterString, RegexOptions.IgnoreCase));

        Mispicks.Clear();
        foreach (var pickEvent in events) Mispicks.Add(pickEvent);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
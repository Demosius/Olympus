using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;

namespace Deimos.ViewModels.Controls;

public class MissPickDataVM : INotifyPropertyChanged, IDBInteraction, IFilters
{
    public DeimosVM ParentVM { get; set; }
    public Helios Helios { get; set; }
    public List<MissPickVM> AllMissPicks { get; set; }

    #region ParentVM Access

    public DateTime? StartDate => ParentVM.StartDate;
    public DateTime? EndDate => ParentVM.EndDate;
     
    #endregion

    #region INotifyPropertyChanged Members

    public ObservableCollection<MissPickVM> MissPicks { get; set; }

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

    public MissPickDataVM(DeimosVM parentVM)
    {
        ParentVM = parentVM;
        Helios = parentVM.Helios;

        filterString = string.Empty;

        AllMissPicks = new List<MissPickVM>();

        MissPicks = new ObservableCollection<MissPickVM>();

        RefreshDataCommand = new RefreshDataCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
    }

    public async Task RefreshDataAsync()
    {
        if (StartDate is null || EndDate is null)
            AllMissPicks = new List<MissPickVM>();
        else
            AllMissPicks = (await Helios.StaffReader.RawMissPicksAsync((DateTime) StartDate, (DateTime) EndDate)).Select(mp => new MissPickVM(mp, Helios)).ToList();

        ApplyFilters();
    }

    public void ClearFilters()
    {
        FilterString = string.Empty;
        ApplyFilters();
    }

    public void ApplyFilters()
    {
        var events = AllMissPicks.Where(missPick =>
            Regex.IsMatch(missPick.AssignedRF_ID, FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(missPick.ActionNotes, FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(missPick.Comments, FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(missPick.ItemDescription, FilterString, RegexOptions.IgnoreCase));

        MissPicks.Clear();
        foreach (var pickEvent in events) MissPicks.Add(pickEvent);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
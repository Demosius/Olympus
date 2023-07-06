using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Deimos.Models;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;

namespace Deimos.ViewModels.Controls;

public class ErrorsDiscoveredVM : INotifyPropertyChanged, IDBInteraction, IFilters
{
    public DeimosVM Deimos { get; set; }
    public ErrorAllocationVM ParentVM { get; set; }
    public Helios Helios { get; set; }

    public List<ErrorGroup> AllErrors { get; set; }

    #region ParentVM Access
    
    public DateTime? StartDate => ParentVM.StartDate;
    public DateTime? EndDate => ParentVM.EndDate;

    #endregion

    #region INotifyPropertyChanged Members

    public ObservableCollection<ErrorGroup> Errors { get; set; }

    private string filterString;
    public string FilterString
    {
        get => filterString;
        set
        {
            filterString = value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }

    #endregion

    public ErrorsDiscoveredVM(ErrorAllocationVM parentVM)
    {
        ParentVM = parentVM;
        Deimos = ParentVM.ParentVM;
        Helios = parentVM.Helios;

        filterString = string.Empty;

        AllErrors = new List<ErrorGroup>();
        Errors = new ObservableCollection<ErrorGroup>();

        RefreshDataCommand = new RefreshDataCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
    }

    public async Task RefreshDataAsync()
    {
        if (StartDate is null || EndDate is null)
            AllErrors = new List<ErrorGroup>();
        else
        {
            var mispickTask = Helios.StaffReader.RawMispicksAsync((DateTime)StartDate, (DateTime)EndDate);
            var tagAssignToolTask = Helios.StaffReader.TagAssignmentToolAsync();

            await Task.WhenAll(mispickTask, tagAssignToolTask);

            var mispicks = (await mispickTask).ToList();
            var tagAssignTool = await tagAssignToolTask;

            AllErrors = ErrorGroup.GenerateErrorGroupsPosted(mispicks).OrderBy(e => e.Date).ToList();
            foreach (var errorGroup in AllErrors)
                errorGroup.Employee = tagAssignTool.Employee(errorGroup.Date, errorGroup.AssignedRF_ID);
        }

        ApplyFilters();
    }
    
    public void ClearFilters()
    {
        FilterString = string.Empty;
    }

    public void ApplyFilters()
    {
        var events = AllErrors.Where(mispick =>
            Regex.IsMatch(mispick.AssignedRF_ID, FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(mispick.Employee?.FullName ?? "", FilterString, RegexOptions.IgnoreCase));

        Errors.Clear();
        foreach (var pickEvent in events) Errors.Add(pickEvent);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
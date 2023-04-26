using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Deimos.Models;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;

namespace Deimos.ViewModels.Controls;

public class EmployeeMissPickVM : INotifyPropertyChanged, IDBInteraction, IFilters
{
    public DeimosVM ParentVM { get; set; }

    public List<ErrorGroup> AllErrors { get; set; }

    #region ParentVM Access

    public Helios Helios => ParentVM.Helios;
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
        }
    }

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public RepairDataCommand RepairDataCommand { get; set; }
    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }

    #endregion

    public EmployeeMissPickVM(DeimosVM parentVM)
    {
        ParentVM = parentVM;

        filterString = string.Empty;

        AllErrors = new List<ErrorGroup>();
        Errors = new ObservableCollection<ErrorGroup>();

        RefreshDataCommand = new RefreshDataCommand(this);
        RepairDataCommand = new RepairDataCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
    }

    public void RefreshData()
    {
        if (StartDate is null || EndDate is null)
            AllErrors = new List<ErrorGroup>();
        else
        {
            var missPicks = Helios.StaffReader.RawMissPicks((DateTime)StartDate, (DateTime)EndDate).ToList();
            var tagAssignTool = Helios.StaffReader.TagAssignmentTool();
            AllErrors = ErrorGroup.GenerateErrorGroups(missPicks).OrderBy(e => e.Date).ToList();
            foreach (var errorGroup in AllErrors)
                errorGroup.Employee = tagAssignTool.Employee(errorGroup.Date, errorGroup.AssignedRF_ID);
        }

        ApplyFilters();
    }

    public void RepairData()
    {
        throw new NotImplementedException();
    }

    public void ClearFilters()
    {
        filterString = string.Empty;
        ApplyFilters();
    }

    public void ApplyFilters()
    {
        var events = AllErrors.Where(missPick =>
            Regex.IsMatch(missPick.AssignedRF_ID, FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(missPick.Employee?.FullName ?? "", FilterString, RegexOptions.IgnoreCase));

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
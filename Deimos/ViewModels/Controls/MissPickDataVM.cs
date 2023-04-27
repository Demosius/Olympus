﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;

namespace Deimos.ViewModels.Controls;

public class MissPickDataVM : INotifyPropertyChanged, IDBInteraction, IFilters
{
    public DeimosVM ParentVM { get; set; }

    public List<MissPickVM> AllMissPicks { get; set; }

    #region ParentVM Access

    public Helios Helios => ParentVM.Helios;

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
    public RepairDataCommand RepairDataCommand { get; set; }
    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }

    #endregion

    public MissPickDataVM(DeimosVM parentVM)
    {
        ParentVM = parentVM;

        filterString = string.Empty;

        AllMissPicks = new List<MissPickVM>();

        MissPicks = new ObservableCollection<MissPickVM>();

        RefreshDataCommand = new RefreshDataCommand(this);
        RepairDataCommand = new RepairDataCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
    }

    public void RefreshData()
    {
        if (StartDate is null || EndDate is null)
            AllMissPicks = new List<MissPickVM>();
        else
            AllMissPicks = Helios.StaffReader.RawMissPicks((DateTime)StartDate, (DateTime)EndDate).Select(mp => new MissPickVM(mp, Helios)).ToList();

        ApplyFilters();
    }

    public void RepairData()
    {
        throw new NotImplementedException();
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
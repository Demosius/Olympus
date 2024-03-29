﻿using System;
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
using Uranus.Staff.Models;

namespace Deimos.ViewModels.Controls;

public class PickHistoryVM : INotifyPropertyChanged, IDBInteraction, IFilters
{
    public DeimosVM Deimos { get; set; }
    public ErrorAllocationVM ParentVM { get; set; }
    public Helios Helios { get; set; }
    public ProgressBarVM ProgressBar { get; set; }

    public List<PickEvent> AllEvents { get; set; }

    #region ParentVM Access

    public DateTime? StartDate => ParentVM.StartDate;
    public DateTime? EndDate => ParentVM.EndDate;

    #endregion

    #region InotifyPropertyChanged Members

    private DateTime? date;
    public DateTime? Date
    {
        get => date;
        set
        {
            date = value;
            OnPropertyChanged();
            _ = RefreshDataAsync();
        }
    }

    public ObservableCollection<PickEvent> PickEvents { get; set; }

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

    public PickHistoryVM(ErrorAllocationVM parentVM)
    {
        ParentVM = parentVM;
        Deimos = ParentVM.ParentVM;
        Helios = parentVM.Helios;
        ProgressBar = parentVM.ProgressBar;

        AllEvents = new List<PickEvent>();
        PickEvents = new ObservableCollection<PickEvent>();
        filterString = string.Empty;

        RefreshDataCommand = new RefreshDataCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
    }

    public async Task RefreshDataAsync()
    {
        PickEvents.Clear();

        ProgressBar.StartTask("Loading Pick Events...", $"{Date:dd-MMM-yyyy}");
        AllEvents = Date is null ? new List<PickEvent>() : (await Helios.StaffReader.PickEventsAsync((DateTime) Date)).ToList();
        ProgressBar.EndTask();

        ApplyFilters();
    }

    public void ClearFilters()
    {
        FilterString = string.Empty;
        ApplyFilters();
    }

    public void ApplyFilters()
    {
        var events = AllEvents.Where(e =>
            Regex.IsMatch(e.OperatorRF_ID, FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(e.OperatorDematicID, FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(e.ItemDescription, FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(e.ContainerID, FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(e.ItemNumber.ToString(), FilterString, RegexOptions.IgnoreCase));

        PickEvents.Clear();
        foreach (var pickEvent in events) PickEvents.Add(pickEvent);   
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Pantheon.Annotations;
using SQLite;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.Controls.Shifts;

public class ShiftVM : INotifyPropertyChanged
{
    public Shift Shift { get; set; }

    #region Direct Shift Access

    public string Name => Shift.Name;

    public Department? Department => Shift.Department;

    public TimeSpan StartTime
    {
        get => Shift.StartTime;
        set => Shift.StartTime = value;
    }

    public TimeSpan EndTime
    {
        get => Shift.EndTime;
        set => Shift.EndTime = value;
    }

    public List<Break> Breaks
    {
        get => Shift.Breaks;
        set => Shift.Breaks = value;
    }

    public int DailyTarget
    {
        get => Shift.DailyTarget;
        set
        {
            Shift.DailyTarget = value;
            OnPropertyChanged();
        }
    }

    public bool Default
    {
        get => Shift.Default;
        set
        {
            Shift.Default = value;
            OnPropertyChanged();
            if (!Default || Department?.Shifts is null) return;
            foreach (var shift in Department.Shifts.Where(shift => shift.Name != Name))
                shift.Default = false;
        }
    }

    private string? startString;
    [Ignore]
    public string StartString
    {
        get => startString ??= StartTime.ToString();
        set
        {
            startString = value;
            if (TimeSpan.TryParse(value, out var newSpan))
                StartTime = newSpan;
        }
    }

    private string? endString;
    [Ignore]
    public string EndString
    {
        get => endString ??= EndTime.ToString();
        set
        {
            endString = value;
            if (TimeSpan.TryParse(value, out var newSpan))
                EndTime = newSpan;
        }
    }

    #endregion

    #region Notifiable Properties

    public ObservableCollection<BreakVM> BreaksObservable { get; set; }

    #endregion

    public ShiftVM(Shift shift)
    {
        Shift = shift;
        BreaksObservable = new ObservableCollection<BreakVM>(Breaks.Select(b => new BreakVM(b, this)));
    }

    public void SetBreaks(IEnumerable<Break> newBreaks)
    {
        Breaks = newBreaks.ToList();
        Breaks.Sort();
        BreaksObservable = new ObservableCollection<BreakVM>(Breaks.Select(b => new BreakVM(b, this)));
    }

    public void AddBreaks(IEnumerable<Break> newBreaks)
    {
        foreach (var newBreak in newBreaks)
        {
            Breaks.Add(newBreak);
            BreaksObservable.Add(new BreakVM(newBreak, this));
        }
    }

    public void AddBreak(Break newBreak)
    {
        Breaks.Add(newBreak);
        Breaks.Sort();
        BreaksObservable = new ObservableCollection<BreakVM>(Breaks.Select(b => new BreakVM(b, this)));
        OnPropertyChanged(nameof(BreaksObservable));
    }

    public void RemoveBreak(BreakVM deletedBreak)
    {
        Breaks.Remove(deletedBreak.Break);
        BreaksObservable.Remove(deletedBreak);
    }

    public void SortBreaks()
    {
        Breaks.Sort();
        BreaksObservable = new ObservableCollection<BreakVM>(Breaks.Select(b => new BreakVM(b, this)));
        OnPropertyChanged(nameof(BreaksObservable));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
﻿using System;
using Pantheon.ViewModels.Commands.Rosters;
using Pantheon.ViewModels.Controls.Rosters;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Pantheon.ViewModels.PopUp.Rosters;

internal class PublicHolidayVM : INotifyPropertyChanged
{
    public DepartmentRosterVM ParentVM { get; }

    #region INotifyPropertyChanged Members

    private bool sunday;
    public bool Sunday
    {
        get => sunday;
        set
        {
            sunday = value;
            OnPropertyChanged();
        }
    }

    private bool monday;
    public bool Monday
    {
        get => monday;
        set
        {
            monday = value;
            OnPropertyChanged();
        }
    }

    private bool tuesday;
    public bool Tuesday
    {
        get => tuesday;
        set
        {
            tuesday = value;
            OnPropertyChanged();
        }
    }

    private bool wednesday;
    public bool Wednesday
    {
        get => wednesday;
        set
        {
            wednesday = value;
            OnPropertyChanged();
        }
    }

    private bool thursday;
    public bool Thursday
    {
        get => thursday;
        set
        {
            thursday = value;
            OnPropertyChanged();
        }
    }

    private bool friday;
    public bool Friday
    {
        get => friday;
        set
        {
            friday = value;
            OnPropertyChanged();
        }
    }

    private bool saturday;
    public bool Saturday
    {
        get => saturday;
        set
        {
            saturday = value;
            OnPropertyChanged();
        }
    }

    #endregion

    public ConfirmHolidaysCommand ConfirmHolidaysCommand { get; set; }

    public PublicHolidayVM(DepartmentRosterVM vm)
    {
        ParentVM = vm;

        Sunday = vm.SundayRoster?.PublicHoliday ?? false;
        Monday = vm.MondayRoster?.PublicHoliday ?? false;
        Tuesday = vm.TuesdayRoster?.PublicHoliday ?? false;
        Wednesday = vm.WednesdayRoster?.PublicHoliday ?? false;
        Thursday = vm.ThursdayRoster?.PublicHoliday ?? false;
        Friday = vm.FridayRoster?.PublicHoliday ?? false;
        Saturday = vm.SaturdayRoster?.PublicHoliday ?? false;

        ConfirmHolidaysCommand = new ConfirmHolidaysCommand(this);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void ConfirmHolidays()
    {
        ParentVM.SetPublicHoliday(DayOfWeek.Sunday, Sunday);
        ParentVM.SetPublicHoliday(DayOfWeek.Monday, Monday);
        ParentVM.SetPublicHoliday(DayOfWeek.Tuesday, Tuesday);
        ParentVM.SetPublicHoliday(DayOfWeek.Wednesday, Wednesday);
        ParentVM.SetPublicHoliday(DayOfWeek.Thursday, Thursday);
        ParentVM.SetPublicHoliday(DayOfWeek.Friday, Friday);
        ParentVM.SetPublicHoliday(DayOfWeek.Saturday, Saturday);
    }
}
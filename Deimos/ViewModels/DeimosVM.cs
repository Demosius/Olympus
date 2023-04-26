using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Deimos.Models;
using Deimos.ViewModels.Commands;
using Deimos.ViewModels.Controls;
using Morpheus;
using Morpheus.ViewModels.Commands;
using Morpheus.ViewModels.Interfaces;
using Serilog;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;

namespace Deimos.ViewModels;

public class DeimosVM : INotifyPropertyChanged, IDBInteraction, IRun
{
    public Helios Helios { get; set; }

    public PickHistoryVM PickHistoryVM { get; }
    public MissPickDataVM MissPickDataVM { get; }
    public EmployeeMissPickVM EmployeeMissPickVM { get; }

    #region INotifyPropertChanged Members

    public bool CanRun => StartDate is not null && EndDate is not null;

    public ObservableCollection<DataDateComparison> DataComps { get; set; }

    private DateTime? startDate;
    public DateTime? StartDate
    {
        get => startDate;
        set
        {
            startDate = value;
            if (endDate < startDate)
            {
                endDate = value;
                OnPropertyChanged(nameof(endDate));
            }
            OnPropertyChanged();
            RefreshData();
        }
    }

    private DateTime? endDate;
    public DateTime? EndDate
    {
        get => endDate;
        set
        {
            endDate = value; 
            if (startDate > endDate)
            {
                startDate = value;
                OnPropertyChanged(nameof(startDate));
            }
            OnPropertyChanged();
            RefreshData();
        }
    }

    private int dateRange;
    public int DateRange
    {
        get => dateRange;
        set
        {
            dateRange = value;
            OnPropertyChanged();
        }
    }

    private int datesWithoutData;
    public int DatesWithoutData
    {
        get => datesWithoutData;
        set
        {
            datesWithoutData = value;
            OnPropertyChanged();
        }
    }

    private int datesWithoutEvents;
    public int DatesWithoutEvents
    {
        get => datesWithoutEvents;
        set
        {
            datesWithoutEvents = value;
            OnPropertyChanged();
        }
    }

    private int datesWithoutMissPicks;
    public int DatesWithoutMissPicks
    {
        get => datesWithoutMissPicks;
        set
        {
            datesWithoutMissPicks = value;
            OnPropertyChanged();
        }
    }

    private bool overwrite;
    public bool Overwrite
    {
        get => overwrite;
        set
        {
            overwrite = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public RepairDataCommand RepairDataCommand { get; set; }
    public RunCommand RunCommand { get; set; }
    public UploadPickEventsCommand UploadPickEventsCommand { get; set; }
    public UploadMissPickDataCommand UploadMissPickDataCommand { get; set; }

    #endregion

    public DeimosVM(Helios helios)
    {
        Helios = helios;

        PickHistoryVM = new PickHistoryVM(this);
        MissPickDataVM = new MissPickDataVM(this);
        EmployeeMissPickVM = new EmployeeMissPickVM(this);

        DataComps = new ObservableCollection<DataDateComparison>();

        RefreshDataCommand = new RefreshDataCommand(this);
        RepairDataCommand = new RepairDataCommand(this);
        RunCommand = new RunCommand(this);
        UploadPickEventsCommand = new UploadPickEventsCommand(this);
        UploadMissPickDataCommand = new UploadMissPickDataCommand(this);
    }

    public void UploadPickEvents()
    {
        Mouse.OverrideCursor = Cursors.Wait;
        try
        {
            var lines = Helios.StaffUpdater.UploadPickEvents(General.ClipboardToString());
            if (lines > 0)
            {
                Mouse.OverrideCursor = Cursors.Arrow;
                MessageBox.Show($"{lines} lines affected.", "Upload Success", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                Mouse.OverrideCursor = Cursors.Arrow;
                MessageBox.Show("Failed to upload data. Check that clipboard contents are correct, and try again.", "Upload Failed",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        catch (Exception ex)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            Log.Error(ex, "Failed to upload pick events.");
            MessageBox.Show($"Unexpected Error:\n\n{ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        RefreshData();
    }

    public void UploadMissPickData()
    {
        Mouse.OverrideCursor = Cursors.Wait;
        try
        {
            var lines = Helios.StaffCreator.UploadMissPickData(General.ClipboardToString());
            if (lines > 0)
            {
                Mouse.OverrideCursor = Cursors.Arrow;
                MessageBox.Show($"{lines} lines affected.", "Upload Success", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                Mouse.OverrideCursor = Cursors.Arrow;
                MessageBox.Show("Failed to upload data. Check that clipboard contents are correct, and try again.", "Upload Failed",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        catch (Exception ex)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            Log.Error(ex, "Failed to upload miss pick data.");
            MessageBox.Show($"Unexpected Error:\n\n{ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        RefreshData();
    }

    public void RefreshData()
    {
        DataComps.Clear();

        if (StartDate is null || EndDate is null)
        {
            SetDateData();
            return;
        }

        Mouse.OverrideCursor = Cursors.Wait;

        var events = Helios.StaffReader.RawPickEvents((DateTime)StartDate, (DateTime)EndDate).ToList();
        var missPicks = Helios.StaffReader.RawMissPicks((DateTime) StartDate, (DateTime) EndDate).ToList();

        var comps = DataDateComparison.GetDateComparisons(events, missPicks).OrderBy(d => d.Date);

        foreach (var comp in comps)
            DataComps.Add(comp);

        SetDateData();

        Mouse.OverrideCursor = Cursors.Arrow;
    }

    private void SetDateData()
    {
        if (StartDate is null || EndDate is null)
        {
            DateRange = 0;
            DatesWithoutData = 0;
            DatesWithoutEvents = 0;
            DatesWithoutMissPicks = 0;
            return;
        }

        var sd = (DateTime)StartDate;
        var ed = (DateTime)EndDate;

        DateRange = ed.Subtract(sd).Days;

        DatesWithoutData = DateRange - DataComps.Count;
        DatesWithoutEvents = DataComps.Count(d => !d.HasPickEvents);
        DatesWithoutMissPicks = DataComps.Count(d => !d.HasMissPicks);
    }

    public void RepairData()
    {
        throw new NotImplementedException();
    }
    public void Run()
    {
        if (StartDate is null || EndDate is null)
        {
            MessageBox.Show("Please ensure dates are set before running assignment tool.", "No Dates",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        Mouse.OverrideCursor = Cursors.Wait;
        var errorTool = new ErrorAssignmentTool(Helios, (DateTime) StartDate, (DateTime) EndDate, Overwrite);

        if (errorTool.AssignErrors())
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            MessageBox.Show("Error assignment complete.\n\n" +
                            $"Assigned: {errorTool.AssignedCount} - {errorTool.AssignedUnits}\n" +
                            $"Unassigned: {errorTool.UnassignedCount} - {errorTool.UnassignedUnits}", 
                "Success", MessageBoxButton.OK);
        }
        else
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            MessageBox.Show("Failed to complete error assignment.", "Failure", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
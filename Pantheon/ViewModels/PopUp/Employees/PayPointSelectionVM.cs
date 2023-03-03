﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Pantheon.Annotations;
using Pantheon.ViewModels.Commands.Employees;
using Pantheon.ViewModels.Controls.Employees;
using Pantheon.ViewModels.Interface;
using Styx;
using Uranus;

namespace Pantheon.ViewModels.PopUp.Employees;

public class PayPointSelectionVM : INotifyPropertyChanged, IStringCount
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public ObservableCollection<StringCountVM> PayPoints { get; set; }

    public bool CanCreatePayPoints { get; set; }

    #region INotifyPropertyChanged Members
    
    private StringCountVM? selectedPayPoint;
    public StringCountVM? SelectedPayPoint
    {
        get => selectedPayPoint;
        set
        {
            selectedPayPoint = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanDelete));
        }
    }

    private string newPayPointName;
    public string NewPayPointName
    {
        get => newPayPointName;
        set
        {
            newPayPointName = value;
            OnPropertyChanged();
        }
    }

    public bool CanDelete => SelectedPayPoint?.Count == 0;
    public bool CanConfirm => SelectedPayPoint is not null;
    public bool CanAdd => CanCreatePayPoints && NewPayPointName.Length > 0;

    #endregion

    #region Commands

    public AddNewStringCountCommand AddNewStringCountCommand { get; set; }
    public DeleteStringCountCommand DeleteStringCountCommand { get; set; }
    public ConfirmStringCountSelectionCommand ConfirmStringCountSelectionCommand { get; set; }

    #endregion

    public PayPointSelectionVM(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        PayPoints = new ObservableCollection<StringCountVM>(
            Helios.StaffReader.Employees()
                .GroupBy(e => e.PayPoint)
                .ToDictionary(g => g.Key, g => g.Count())
                .Select(i => new StringCountVM(i.Key, i.Value))
                .OrderBy(p => p.Name)
            );

        CanCreatePayPoints = Charon.CanCreateEmployee();
        newPayPointName = string.Empty;

        AddNewStringCountCommand = new AddNewStringCountCommand(this);
        DeleteStringCountCommand = new DeleteStringCountCommand(this);
        ConfirmStringCountSelectionCommand = new ConfirmStringCountSelectionCommand(this);
    }

    public void AddNewPayPoint()
    {
        var newPP = new StringCountVM(NewPayPointName, 0);
        PayPoints.Add(newPP);
        SelectedPayPoint = newPP;
        NewPayPointName = string.Empty;
    }

    public void DeletePayPoint()
    {
        if (SelectedPayPoint is null || SelectedPayPoint.Count > 0) return;

        PayPoints.Remove(SelectedPayPoint);
        SelectedPayPoint = null;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
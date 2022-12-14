using Pantheon.ViewModels.Pages;
using System;
using System.Windows;
using System.Windows.Input;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.Commands.Shifts;

public class SaveShiftCommand : ICommand
{
    public ShiftPageVM VM { get; set; }

    public SaveShiftCommand(ShiftPageVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedDepartment is not null && (VM.Charon?.CanUpdateShift(VM.SelectedDepartment) ?? false);
    }

    public void Execute(object? parameter)
    {
        if (parameter is not Shift shift || VM.Helios is null) return;
        shift.SortBreaks();
        if (VM.Helios.StaffUpdater.Shift(shift) > 0)
            MessageBox.Show("Shift saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
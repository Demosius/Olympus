using Pantheon.ViewModel.Pages;
using System;
using System.Windows.Input;
using Uranus.Staff.Model;

namespace Pantheon.ViewModel.Commands;

public class LaunchShiftEmployeeWindowCommand : ICommand
{
    public ShiftPageVM VM { get; set; }

    public LaunchShiftEmployeeWindowCommand(ShiftPageVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedDepartment is not null && (VM.Charon?.CanUpdateShift(VM.SelectedDepartment) ?? false);
    }

    public void Execute(object? parameter)
    {
        if (parameter is not Shift shift) return;
        VM.LaunchShiftEmployeeWindow(shift);
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
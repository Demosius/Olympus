using System;
using System.Windows.Input;
using Pantheon.ViewModels.Controls.Shifts;
using Pantheon.ViewModels.Pages;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.Commands.Shifts;

public class LaunchShiftEmployeeWindowCommand : ICommand
{
    public ShiftVM VM { get; set; }

    public LaunchShiftEmployeeWindowCommand(ShiftVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.Department is not null && VM.Charon.CanUpdateShift(VM.Department);
    }

    public void Execute(object? parameter)
    {
        VM.LaunchShiftEmployeeWindow();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
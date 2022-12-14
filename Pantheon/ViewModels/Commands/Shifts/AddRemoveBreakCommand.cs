using Pantheon.ViewModels.Pages;
using System;
using System.Windows.Input;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.Commands.Shifts;

public class AddRemoveBreakCommand : ICommand
{
    public ShiftPageVM VM { get; set; }

    public AddRemoveBreakCommand(ShiftPageVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedDepartment is not null && (VM.Charon?.CanUpdateShift(VM.SelectedDepartment) ?? false);
    }

    public void Execute(object? parameter)
    {
        if (parameter is not Break @break || @break.Shift is null) return;
        if (@break.Name == "Lunch")
            VM.AddBreak(@break.Shift);
        else
            VM.RemoveBreak(@break);
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
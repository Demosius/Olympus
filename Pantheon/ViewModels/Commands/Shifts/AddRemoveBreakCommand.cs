using System;
using System.Windows.Input;
using Pantheon.ViewModels.Controls.Shifts;
using Pantheon.ViewModels.Pages;
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
        if (parameter is not BreakVM breakVM || breakVM.Shift is null) return;
        if (breakVM.Name == "Lunch")
            VM.AddBreak(breakVM.ShiftVM);
        else
            VM.RemoveBreak(breakVM);
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
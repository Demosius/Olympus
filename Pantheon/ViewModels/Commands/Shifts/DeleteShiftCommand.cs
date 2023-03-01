using System;
using System.Windows.Input;
using Pantheon.ViewModels.Controls.Shifts;
using Pantheon.ViewModels.Pages;

namespace Pantheon.ViewModels.Commands.Shifts;

public class DeleteShiftCommand : ICommand
{
    public ShiftPageVM VM { get; set; }

    public DeleteShiftCommand(ShiftPageVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedDepartment is not null && (VM.Charon?.CanDeleteShift(VM.SelectedDepartment) ?? false);
    }

    public void Execute(object? parameter)
    {
        if (parameter is not ShiftVM shift) return;
        VM.DeleteShift(shift);
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
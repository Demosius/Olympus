using System;
using System.Windows.Input;
using Pantheon.ViewModels.Controls.Shifts;

namespace Pantheon.ViewModels.Commands.Shifts;

public class DeleteShiftCommand : ICommand
{
    public ShiftVM VM { get; set; }

    public DeleteShiftCommand(ShiftVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.Department is not null && VM.Charon.CanDeleteShift(VM.Department);
    }

    public void Execute(object? parameter)
    {
        VM.Delete();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
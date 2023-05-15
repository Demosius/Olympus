using System;
using System.Windows.Input;

namespace Aion.ViewModels.Commands;


public class DeleteClockCommand : ICommand
{
    public ShiftEntryPageVM VM { get; set; }

    public DeleteClockCommand(ShiftEntryPageVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.DeleteSelectedClock();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
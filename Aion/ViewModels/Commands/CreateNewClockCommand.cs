using System;
using System.Windows.Input;

namespace Aion.ViewModels.Commands;


public class CreateNewClockCommand : ICommand
{
    public ShiftEntryPageVM VM { get; set; }

    public CreateNewClockCommand(ShiftEntryPageVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.CreateNewClock();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
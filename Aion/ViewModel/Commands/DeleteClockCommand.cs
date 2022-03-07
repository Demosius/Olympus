using System;
using System.Windows.Input;

namespace Aion.ViewModel.Commands;

public class DeleteClockCommand : ICommand
{
    public ShiftEntryPageVM VM { get; set; }

    public DeleteClockCommand(ShiftEntryPageVM vm) { VM = vm; }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object parameter)
    {
        return VM.SelectedClock != null;
    }

    public void Execute(object parameter)
    {
        VM.DeleteSelectedClock();
    }
}
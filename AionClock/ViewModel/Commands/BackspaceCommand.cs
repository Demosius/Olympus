using System;
using System.Windows.Input;

namespace AionClock.ViewModel.Commands;

public class BackspaceCommand : ICommand
{
    public ClockInVM VM { get; set; }

    public BackspaceCommand(ClockInVM vm) { VM = vm; }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object parameter)
    {
        return VM.Input.Length > 0;
    }

    public void Execute(object parameter)
    {
        VM.Input = VM.Input[..^1];
    }
}
using System;
using System.Windows.Input;

namespace AionClock.ViewModels.Commands;

public class InputKeyCommand : ICommand
{
    public ClockInVM VM { get; set; }

    public InputKeyCommand(ClockInVM vm) { VM = vm; }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object parameter)
    {
        return VM.Input.Length < 5;
    }

    public void Execute(object parameter)
    {
        var c = parameter as string;
        VM.Input += c;
    }
}
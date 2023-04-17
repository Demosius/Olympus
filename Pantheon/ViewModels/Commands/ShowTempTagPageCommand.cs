using System;
using System.Windows.Input;

namespace Pantheon.ViewModels.Commands;

public class ShowTempTagPageCommand : ICommand
{
    public PantheonVM VM { get; set; }

    public ShowTempTagPageCommand(PantheonVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.ShowTempTagPage();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
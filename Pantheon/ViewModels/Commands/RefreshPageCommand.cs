using System;
using System.Windows.Input;

namespace Pantheon.ViewModels.Commands;

internal class RefreshPageCommand : ICommand
{
    public PantheonVM VM { get; set; }

    public RefreshPageCommand(PantheonVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.RefreshPage();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
using System;
using System.Windows.Input;

namespace Pantheon.ViewModels.Commands;

public class RefreshPageCommand : ICommand
{
    public PantheonVM VM { get; set; }

    public RefreshPageCommand(PantheonVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public async void Execute(object? parameter)
    {
        await VM.RefreshPage();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
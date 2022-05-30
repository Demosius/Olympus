using System;
using System.Windows.Input;

namespace Pantheon.ViewModels.Commands;

internal class ShowShiftPageCommand : ICommand
{
    public PantheonVM VM { get; set; }

    public ShowShiftPageCommand(PantheonVM vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.ShowShiftPage();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
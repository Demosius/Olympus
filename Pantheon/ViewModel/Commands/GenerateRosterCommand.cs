using System;
using System.Windows.Input;
using Pantheon.ViewModel.Pages;

namespace Pantheon.ViewModel.Commands;

internal class GenerateRosterCommand : ICommand
{
    public RosterPageVM VM { get; set; }

    public GenerateRosterCommand(RosterPageVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.LoadedRoster is not null;
    }

    public void Execute(object? parameter)
    {
        VM.GenerateRoster();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
using Pantheon.ViewModel.Pages;
using System;
using System.Windows.Input;

namespace Pantheon.ViewModel.Commands.Rosters;

public class ExportRosterCommand : ICommand
{
    public RosterPageVM VM { get; set; }

    public ExportRosterCommand(RosterPageVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedRoster is not null;
    }

    public void Execute(object? parameter)
    {
        VM.ExportRoster();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
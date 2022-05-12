using Pantheon.ViewModel.Pages;
using System;
using System.Windows.Input;

namespace Pantheon.ViewModel.Commands.Rosters;

public class LoadRosterCommand : ICommand
{
    public RosterPageVM VM { get; set; }

    public LoadRosterCommand(RosterPageVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        if (VM.LoadedRoster is not null)
            Mouse.OverrideCursor = Cursors.Arrow;

        return VM.SelectedDepartment is not null && VM.SelectedRoster is not null && VM.LoadedRoster is null;
    }

    public void Execute(object? parameter)
    {
        if (VM.SelectedRoster is not null && VM.LoadedRoster is null)
            VM.LoadRoster();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
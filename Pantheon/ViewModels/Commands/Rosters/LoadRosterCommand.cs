using System;
using System.Windows.Input;
using Pantheon.ViewModels.Pages;

namespace Pantheon.ViewModels.Commands.Rosters;

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

    public async void Execute(object? parameter)
    {
        if (VM.SelectedRoster is not null && VM.LoadedRoster is null)
            await VM.LoadRoster();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
using System;
using System.Windows.Input;
using Pantheon.ViewModels.Pages;

namespace Pantheon.ViewModels.Commands.Rosters;

public class ExportRosterCommand : ICommand
{
    public RosterPageVM VM { get; set; }

    public ExportRosterCommand(RosterPageVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedRoster is not null;
    }

    public async void Execute(object? parameter)
    {
        await VM.ExportRoster();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
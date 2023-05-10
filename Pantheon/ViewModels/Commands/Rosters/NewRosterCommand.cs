using System;
using System.Windows.Input;
using Pantheon.ViewModels.Pages;

namespace Pantheon.ViewModels.Commands.Rosters;

public class NewRosterCommand : ICommand
{
    public RosterPageVM VM { get; set; }

    public NewRosterCommand(RosterPageVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedDepartment is not null;
    }

    public async void Execute(object? parameter)
    {
        await VM.NewRoster();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
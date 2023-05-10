using System;
using System.Windows.Input;
using Pantheon.ViewModels.Pages;

namespace Pantheon.ViewModels.Commands.Rosters;

public class SaveRosterCommand : ICommand
{
    public RosterPageVM VM { get; set; }

    public SaveRosterCommand(RosterPageVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.IsLoaded;
    }

    public async void Execute(object? parameter)
    {
        await VM.SaveRoster();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
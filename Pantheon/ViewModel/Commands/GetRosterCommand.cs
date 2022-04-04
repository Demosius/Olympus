using System;
using System.Windows.Input;
using Pantheon.ViewModel.Pages;

namespace Pantheon.ViewModel.Commands;

internal class GetRosterCommand : ICommand
{
    public RosterPageVM VM { get; set; }

    public GetRosterCommand(RosterPageVM vm ) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedDepartment is not null;
    }

    public void Execute(object? parameter)
    {
        VM.GetRoster();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
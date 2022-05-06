using System;
using System.Windows.Input;
using Pantheon.ViewModel.Pages;

namespace Pantheon.ViewModel.Commands;

public class SaveRosterCommand : ICommand 
{
    public RosterPageVM VM { get; set; }

    public SaveRosterCommand(RosterPageVM vm ) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.IsLoaded;
    }

    public void Execute(object? parameter)
    {
        VM.SaveRoster();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
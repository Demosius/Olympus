using Pantheon.ViewModels.Controls.Rosters;
using System;
using System.Windows.Input;

namespace Pantheon.ViewModels.Commands.Rosters;

public class GenerateRosterCommand : ICommand
{
    public DepartmentRosterVM VM { get; set; }

    public GenerateRosterCommand(DepartmentRosterVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.IsInitialized;
    }

    public void Execute(object? parameter)
    {
        VM.GenerateRosterAssignments();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
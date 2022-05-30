using Pantheon.ViewModels.Controls.Rosters;
using System;
using System.Windows.Input;

namespace Pantheon.ViewModels.Commands.Shifts;

public class ClearShiftsCommand : ICommand
{
    public DepartmentRosterVM VM { get; set; }

    public ClearShiftsCommand(DepartmentRosterVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.IsInitialized;
    }

    public void Execute(object? parameter)
    {
        VM.UnAssignAll();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
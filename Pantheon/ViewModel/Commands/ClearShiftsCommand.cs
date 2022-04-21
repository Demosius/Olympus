using Pantheon.ViewModel.Controls;
using System;
using System.Windows.Input;

namespace Pantheon.ViewModel.Commands;

internal class ClearShiftsCommand : ICommand
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
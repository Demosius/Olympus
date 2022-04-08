using System;
using System.Windows;
using System.Windows.Input;

namespace Pantheon.ViewModel.Commands;

internal class ConfirmDepartmentRosterCreationCommand : ICommand 
{
    public RosterCreationVM VM { get; set; }

    public ConfirmDepartmentRosterCreationCommand(RosterCreationVM vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        return VM.RosterName is not null or "";
    }

    public void Execute(object? parameter)
    {
        if (parameter is not Window w) return;

        VM.ConfirmDepartmentRosterCreation();

        w.Close();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
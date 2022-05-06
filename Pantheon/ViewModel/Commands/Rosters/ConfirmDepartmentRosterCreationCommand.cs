using System;
using System.Windows;
using System.Windows.Input;
using Pantheon.ViewModel.PopUp.Rosters;

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

        if (!VM.ConfirmDepartmentRosterCreation()) return;

        w.DialogResult = true;
        w.Close();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
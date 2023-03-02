using System;
using System.Windows.Input;
using Pantheon.ViewModels.Controls.Employees;
using Pantheon.ViewModels.Pages;

namespace Pantheon.ViewModels.Commands.Employees;

public class SelectRoleCommand : ICommand
{
    public EmployeeVM VM { get; set; }

    public SelectRoleCommand(EmployeeVM vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        return VM.Charon?.CanCreateStaffRole() ?? false;
    }

    public void Execute(object? parameter)
    {
        VM.SelectRole();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
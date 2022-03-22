using Pantheon.ViewModel.Pages;
using System;
using System.Windows.Input;

namespace Pantheon.ViewModel.Commands;

public class AddRoleCommand : ICommand
{
    public EmployeePageVM VM { get; set; }

    public AddRoleCommand(EmployeePageVM vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        return VM.Charon?.CanCreateStaffRole() ?? false;
    }

    public void Execute(object? parameter)
    {
        VM.AddRole();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
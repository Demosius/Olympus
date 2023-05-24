using System;
using System.Windows.Input;
using Pantheon.ViewModels.Pages;

namespace Pantheon.ViewModels.Commands.Employees;

public class ActivateUserCommand : ICommand
{
    public EmployeePageVM VM { get; set; }

    public ActivateUserCommand(EmployeePageVM vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedEmployeeVM is not null &&
               !VM.SelectedEmployeeVM.IsUser &&
               (VM.Charon?.CanCreateUser(VM.SelectedEmployeeVM.Employee) ?? false);
    }

    public void Execute(object? parameter)
    {
        VM.ActivateUser();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
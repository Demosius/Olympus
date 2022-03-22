using System;
using System.Windows.Input;
using Pantheon.ViewModel.Pages;

namespace Pantheon.ViewModel.Commands;

public class AddDepartmentCommand : ICommand
{
    public EmployeePageVM VM { get; set; }

    public AddDepartmentCommand(EmployeePageVM vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        return VM.Charon?.CanCreateDepartment() ?? false;
    }

    public void Execute(object? parameter)
    {
        VM.AddDepartment();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
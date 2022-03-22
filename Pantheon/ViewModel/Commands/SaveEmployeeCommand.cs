using System;
using System.Windows.Input;
using Pantheon.ViewModel.Pages;

namespace Pantheon.ViewModel.Commands;

public class SaveEmployeeCommand : ICommand
{
    public EmployeePageVM VM { get; set; }

    public SaveEmployeeCommand(EmployeePageVM vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        if (VM.SelectedEmployee is null) return false;
        return VM.Charon?.CanUpdateEmployee(VM.SelectedEmployee) ?? false;
    }

    public void Execute(object? parameter)
    {
        VM.SaveEmployee();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
#nullable enable
using System;
using System.Windows.Input;

namespace Aion.ViewModels.Commands;

public class GoToEmployeeCommand : ICommand
{
    public EmployeePageVM VM { get; set; }

    public GoToEmployeeCommand(EmployeePageVM vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedReport is not null;
    }

    public void Execute(object? parameter)
    {
        VM.GoToEmployee();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
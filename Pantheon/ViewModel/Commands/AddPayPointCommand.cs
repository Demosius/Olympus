using System;
using System.Windows.Input;
using Pantheon.ViewModel.Pages;

namespace Pantheon.ViewModel.Commands;

internal class AddPayPointCommand : ICommand
{
    public EmployeePageVM VM { get; set; }

    public AddPayPointCommand(EmployeePageVM vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        return VM.Charon?.CanCreateEmployee() ?? false;
    }

    public void Execute(object? parameter)
    {
        VM.AddPayPoint();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
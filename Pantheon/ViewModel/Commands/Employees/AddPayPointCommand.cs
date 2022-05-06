using System;
using System.Windows.Input;
using Pantheon.ViewModel.Interface;

namespace Pantheon.ViewModel.Commands.Employees;

public class AddPayPointCommand : ICommand
{
    public IPayPoints VM { get; set; }

    public AddPayPointCommand(IPayPoints vm)
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
using System;
using System.Windows.Input;
using Pantheon.ViewModels.Interface;

namespace Pantheon.ViewModels.Commands.Employees;

public class SelectPayPointCommand : ICommand
{
    public IPayPoints VM { get; set; }

    public SelectPayPointCommand(IPayPoints vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        return VM.Charon.CanCreateEmployee();
    }

    public void Execute(object? parameter)
    {
        VM.SelectPayPoint();
    }
    
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
using System;
using System.Windows.Input;
using Pantheon.ViewModels.Interface;

namespace Pantheon.ViewModels.Commands.Employees;


public class AddNewStringCountCommand : ICommand
{
    public IStringCount VM { get; set; }

    public AddNewStringCountCommand(IStringCount vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.CanAdd;
    }

    public void Execute(object? parameter)
    {
        VM.AddNewPayPoint();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
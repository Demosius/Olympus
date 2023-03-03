using System;
using System.Windows.Input;
using Pantheon.ViewModels.Interface;
using Pantheon.ViewModels.PopUp.Employees;

namespace Pantheon.ViewModels.Commands.Employees;


public class DeleteStringCountCommand : ICommand
{
    public IStringCount VM { get; set; }

    public DeleteStringCountCommand(IStringCount vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.CanDelete;
    }

    public void Execute(object? parameter)
    {
        VM.DeletePayPoint();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
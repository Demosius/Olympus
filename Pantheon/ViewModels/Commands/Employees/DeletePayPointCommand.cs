using System;
using System.Windows.Input;
using Pantheon.ViewModels.PopUp.Employees;

namespace Pantheon.ViewModels.Commands.Employees;


public class DeletePayPointCommand : ICommand
{
    public PayPointSelectionVM VM { get; set; }

    public DeletePayPointCommand(PayPointSelectionVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedPayPoint?.Count == 0;
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
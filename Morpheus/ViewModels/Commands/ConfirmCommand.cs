using System;
using System.Windows.Input;
using Morpheus.ViewModels.Interfaces;

namespace Morpheus.ViewModels.Commands;

public class ConfirmCommand : ICommand
{
    public IConfirm VM { get; set; }

    public ConfirmCommand(IConfirm vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.Confirm();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
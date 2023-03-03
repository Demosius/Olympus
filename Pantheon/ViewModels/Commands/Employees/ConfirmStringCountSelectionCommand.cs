using System;
using System.Windows;
using System.Windows.Input;
using Pantheon.ViewModels.Interface;

namespace Pantheon.ViewModels.Commands.Employees;


public class ConfirmStringCountSelectionCommand : ICommand
{
    public IStringCount VM { get; set; }

    public ConfirmStringCountSelectionCommand(IStringCount vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.CanConfirm;
    }

    public void Execute(object? parameter)
    {
        if (parameter is not Window w) return;
        w.DialogResult = true;
        w.Close();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
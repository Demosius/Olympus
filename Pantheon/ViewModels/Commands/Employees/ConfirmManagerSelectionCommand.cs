using System;
using System.Windows;
using System.Windows.Input;
using Pantheon.ViewModels.PopUp.Employees;

namespace Pantheon.ViewModels.Commands.Employees;


public class ConfirmManagerSelectionCommand : ICommand
{
    public ManagerSelectionVM VM { get; set; }

    public ConfirmManagerSelectionCommand(ManagerSelectionVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedManager is not null;
    }

    public void Execute(object? parameter)
    {
        if (parameter is not Window w) return;
        w.DialogResult = VM.SelectedManager is not null;
        w.Close();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
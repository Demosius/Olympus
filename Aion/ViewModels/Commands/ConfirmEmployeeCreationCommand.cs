using System;
using System.Windows;
using System.Windows.Input;

namespace Aion.ViewModels.Commands;


public class ConfirmEmployeeCreationCommand : ICommand
{
    public EmployeeCreationVM VM { get; set; }

    public ConfirmEmployeeCreationCommand(EmployeeCreationVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        var w = parameter as Window;
        VM.ConfirmCreation();
        if (w == null) return;
        w.DialogResult = true;
        w.Close();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
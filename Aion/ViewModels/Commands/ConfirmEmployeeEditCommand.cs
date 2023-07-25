using System;
using System.Windows;
using System.Windows.Input;

namespace Aion.ViewModels.Commands;


public class ConfirmEmployeeEditCommand : ICommand
{
    public EmployeeEditorVM VM { get; set; }

    public ConfirmEmployeeEditCommand(EmployeeEditorVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        var w = parameter as Window;
        VM.ConfirmEdit();
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
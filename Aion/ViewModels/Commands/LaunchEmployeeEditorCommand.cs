using System;
using System.Windows.Input;

namespace Aion.ViewModels.Commands;

public class LaunchEmployeeEditorCommand : ICommand
{
    public EmployeePageVM VM { get; set; }

    public LaunchEmployeeEditorCommand(EmployeePageVM vm) { VM = vm; }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedEmployee != null;
    }

    public void Execute(object? parameter)
    {
        VM.LaunchEmployeeEditor();
    }
}
#nullable enable
using System;
using System.Windows.Input;

namespace Aion.ViewModel.Commands;

public class AddPayPointCommand : ICommand
{
    public EmployeeEditorVM VM { get; set; }

    public AddPayPointCommand(EmployeeEditorVM vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.AddPayPoint();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
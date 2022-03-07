#nullable enable
using System;
using System.Windows.Input;

namespace Aion.ViewModel.Commands;

public class AddLocationCommand : ICommand
{
    public EmployeeEditorVM VM { get; set; }

    public AddLocationCommand(EmployeeEditorVM vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.AddLocation();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
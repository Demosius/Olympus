using System;
using System.Windows.Input;
using Pantheon.ViewModels.Controls.TempTags;

namespace Pantheon.ViewModels.Commands.TempTags;

public class ReOrderEmployeesCommand : ICommand
{
    public TempTagEmployeeVM VM { get; set; }

    public ReOrderEmployeesCommand(TempTagEmployeeVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.ReOrderEmployees();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
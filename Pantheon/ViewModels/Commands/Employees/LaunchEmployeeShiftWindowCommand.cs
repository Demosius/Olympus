using System;
using System.Windows.Input;
using Pantheon.ViewModels.Controls.Employees;

namespace Pantheon.ViewModels.Commands.Employees;

public class LaunchEmployeeShiftWindowCommand : ICommand
{
    public EmployeeVM VM { get; set; }

    public LaunchEmployeeShiftWindowCommand(EmployeeVM vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        return VM.CanUpdate || VM.CanUpdateShift;
    }

    public void Execute(object? parameter)
    {
        VM.LaunchEmployeeShiftWindow();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
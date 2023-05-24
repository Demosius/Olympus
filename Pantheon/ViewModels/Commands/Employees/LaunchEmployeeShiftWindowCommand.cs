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
        return VM.Charon.CanUpdateEmployee(VM.Employee) ||
               VM.Department is not null && VM.Charon.CanUpdateShift(VM.Department);
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
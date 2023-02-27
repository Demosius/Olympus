using System;
using System.Windows;
using System.Windows.Input;
using Pantheon.ViewModels.PopUp.Employees;

namespace Pantheon.ViewModels.Commands.Employees;

internal class ConfirmShiftAdjustmentsCommand : ICommand
{
    public EmployeeShiftVM VM { get; set; }

    public ConfirmShiftAdjustmentsCommand(EmployeeShiftVM vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        var w = parameter as Window;
        VM.ConfirmShiftAdjustments();
        w?.Close();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
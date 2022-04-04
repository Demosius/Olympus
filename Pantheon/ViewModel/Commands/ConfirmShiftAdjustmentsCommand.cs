using System;
using System.Windows;
using System.Windows.Input;

namespace Pantheon.ViewModel.Commands;

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
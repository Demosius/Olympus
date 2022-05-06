using System;
using System.Windows;
using System.Windows.Input;
using Pantheon.ViewModel.PopUp.Employees;

namespace Pantheon.ViewModel.Commands.Employees;

internal class ConfirmDepartmentCreationCommand : ICommand
{
    public DepartmentCreationVM VM { get; set; }

    public ConfirmDepartmentCreationCommand(DepartmentCreationVM vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        return VM.Department.Name != string.Empty && !VM.DepartmentNames.Contains(VM.Department.Name);
    }

    public void Execute(object? parameter)
    {
        if (parameter is not Window w) return;
        w.DialogResult = VM.ConfirmDepartmentCreation();
        w.Close();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
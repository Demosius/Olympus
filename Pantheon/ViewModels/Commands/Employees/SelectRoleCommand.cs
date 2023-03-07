using System;
using System.Windows.Input;
using Pantheon.ViewModels.Interface;

namespace Pantheon.ViewModels.Commands.Employees;

public class SelectRoleCommand : ICommand
{
    public IRoles VM { get; set; }

    public SelectRoleCommand(IRoles vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        return VM.Charon.CanCreateStaffRole();
    }

    public void Execute(object? parameter)
    {
        VM.SelectRole();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
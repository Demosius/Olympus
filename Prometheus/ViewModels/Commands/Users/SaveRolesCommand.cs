using Prometheus.ViewModels.Pages.Users;
using System;
using System.Windows.Input;

namespace Prometheus.ViewModels.Commands.Users;

internal class SaveRolesCommand : ICommand
{
    public RolesVM VM { get; set; }

    public SaveRolesCommand(RolesVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.Charon?.CanEditUserRole() ?? false;
    }

    public void Execute(object? parameter)
    {
        VM.SaveRoles();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
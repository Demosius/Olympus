﻿using Prometheus.ViewModels.Pages.Users;
using System;
using System.Windows.Input;

namespace Prometheus.ViewModels.Commands.Users;

public class ChangeUserRoleCommand : ICommand
{
    public UserViewVM VM { get; set; }

    public ChangeUserRoleCommand(UserViewVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedUser is not null && VM.Charon is not null && VM.Charon.CanAssignUserRole();
    }

    public async void Execute(object? parameter)
    {
        await VM.ChangeUserRole();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
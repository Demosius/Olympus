#nullable enable
using System;
using System.Windows;
using System.Windows.Input;

namespace Olympus.ViewModels.Commands;

internal class ConfirmPasswordChangeCommand : ICommand
{
    public ChangePasswordVM VM { get; set; }

    public ConfirmPasswordChangeCommand(ChangePasswordVM vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        return VM.CheckPassword();
    }

    public void Execute(object? parameter)
    {
        var w = (Window)parameter!;
        if (VM.ChangePassword())
            w.Close();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
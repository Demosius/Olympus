#nullable enable
using System;
using System.Windows.Input;

namespace Olympus.ViewModels.Commands;

public class ChangePasswordCommand : ICommand
{
    public bool CanExecute(object? parameter)
    {
        return App.Charon.User is not null;
    }

    public void Execute(object? parameter)
    {
        OlympusVM.LaunchPasswordChanger();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
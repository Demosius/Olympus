#nullable enable
using System;
using System.Windows.Input;

namespace Olympus.ViewModels.Commands;

public class ChangePasswordCommand : ICommand
{
    public OlympusVM VM { get; set; }

    public ChangePasswordCommand(OlympusVM vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        return App.Charon.User is not null;
    }

    public void Execute(object? parameter)
    {
        VM.LaunchPasswordChanger();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
using Olympus.ViewModels.Components;
using System;
using System.Windows.Input;

namespace Olympus.ViewModels.Commands;

public class UserCommand : ICommand
{
    public UserHandlerVM VM { get; set; }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public UserCommand(UserHandlerVM vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        var commandName = parameter as string;
        switch (commandName)
        {
            case "Register":
                VM.Register();
                break;
            case "Log In":
                VM.LogIn();
                break;
            default:
                VM.LogOut();
                break;
        }
    }
}
using System;
using System.Windows;
using System.Windows.Input;
using Pantheon.Views;
using Uranus;

namespace Pantheon.ViewModels.Commands;

public class LogInCommand : ICommand
{
    public LoginVM VM { get; set; }

    public LogInCommand(LoginVM vm) { VM = vm; }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object? parameter)
    {
        return VM.UserCode.Length > 0 && VM.Password.Length > 0;
    }

    public void Execute(object? parameter)
    {
        if (!AsyncHelper.RunSync(() => VM.LogIn())) return;

        var w = parameter as Window;
        PantheonWindow pw = new();
        w?.Close();
        pw.Show();
    }
}
using Pantheon.View;
using System;
using System.Windows;
using System.Windows.Input;

namespace Pantheon.ViewModel.Commands;

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
        if (!VM.LogIn()) return;

        var w = parameter as Window;
        PantheonWindow pw = new(App.Charon, App.Helios);
        w?.Close();
        pw.Show();
    }
}
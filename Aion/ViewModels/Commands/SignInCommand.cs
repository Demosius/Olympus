using System;
using System.Windows;
using System.Windows.Input;

namespace Aion.ViewModels.Commands;

public class SignInCommand : ICommand
{
    public SignInVM VM { get; set; }

    public SignInCommand(SignInVM vm) { VM = vm; }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object? parameter)
    {
        return VM.Code.Length > 0;
    }

    public void Execute(object? parameter)
    {
        var w = parameter as Window;
        if (VM.SignIn())
        {
            if (w == null) return;
            w.DialogResult = true;
            w.Close();
        }
        else
        {
            MessageBox.Show("Incorrect Code for Selected Manager", "Fail", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
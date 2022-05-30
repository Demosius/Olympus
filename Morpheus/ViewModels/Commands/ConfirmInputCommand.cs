using System;
using System.Windows;
using System.Windows.Input;

namespace Morpheus.ViewModels.Commands;

public class ConfirmInputCommand : ICommand
{
    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        var w = (Window)parameter!;
        w.DialogResult = true;
        w.Close();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
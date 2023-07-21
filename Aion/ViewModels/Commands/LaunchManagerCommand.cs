using Aion.View;
using System;
using System.Windows.Input;

namespace Aion.ViewModels.Commands;

public class LaunchManagerCommand : ICommand
{
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        AionWindow aionWindow = new();
        aionWindow.ShowDialog();
    }
}
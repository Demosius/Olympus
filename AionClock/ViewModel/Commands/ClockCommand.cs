using AionClock.View;
using System;
using System.Windows;
using System.Windows.Input;

namespace AionClock.ViewModel.Commands;

public class ClockCommand : ICommand
{
    public ClockConfirmationVM VM { get; set; }

    public ClockCommand(ClockConfirmationVM vm) { VM = vm; }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object parameter)
    {
        return true;
    }

    public void Execute(object parameter)
    {
        var w = parameter as Window;
        ClockSuccessView newSuccess = new(VM.ClockIn());
        w?.Close();
        newSuccess.ShowDialog();
    }
}
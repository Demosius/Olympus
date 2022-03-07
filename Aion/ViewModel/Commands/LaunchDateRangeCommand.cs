using System;
using System.Windows.Input;
using Aion.ViewModel.Interfaces;

namespace Aion.ViewModel.Commands;

public class LaunchDateRangeCommand : ICommand
{
    public IDateRange VM { get; set; }

    public LaunchDateRangeCommand(IDateRange vm)
    {
        VM = vm;
    }

    public bool CanExecute(object parameter)
    {
        return true;
    }

    public void Execute(object parameter)
    {
        VM.LaunchDateRangeWindow();
    }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
using AionClock.ViewModel.Utility;
using System;
using System.Windows.Input;

namespace AionClock.ViewModel.Commands;

public class ChangeDatabaseCommand : ICommand
{
    public DBManager VM { get; set; }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public ChangeDatabaseCommand(DBManager vm)
    {
        VM = vm;
    }

    public bool CanExecute(object parameter)
    {
        return true;
    }

    public void Execute(object parameter)
    {
        VM.ChangeDatabase();
    }
}
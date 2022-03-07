using System;
using System.Windows.Input;
using AionClock.ViewModel.Utility;

namespace AionClock.ViewModel.Commands;

public class MoveDatabaseCommand : ICommand
{
    public DBManager VM { get; set; }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public MoveDatabaseCommand(DBManager vm)
    {
        VM = vm;
    }

    public bool CanExecute(object parameter)
    {
        return false; // (parameter ?? "null") is string dbLocation && App.Charon.CanMoveDatabase() && dbLocation.ToLower() != "local";
    }

    public void Execute(object parameter)
    {
        VM.MoveDatabase();
    }
}
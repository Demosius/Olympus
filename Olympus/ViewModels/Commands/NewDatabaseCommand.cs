using Olympus.ViewModels.Utility;
using System;
using System.Windows.Input;

namespace Olympus.ViewModels.Commands;

public class NewDatabaseCommand : ICommand
{
    public DBManager Dbm { get; set; }

    public NewDatabaseCommand(DBManager dbm) { Dbm = dbm; }

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
        Dbm.NewDatabase();
    }
}
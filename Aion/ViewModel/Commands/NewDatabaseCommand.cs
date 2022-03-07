using System;
using System.Windows.Input;
using Aion.ViewModel.Utility;

namespace Aion.ViewModel.Commands;

public class NewDatabaseCommand : ICommand
{
    public DBManager DBM { get; set; }

    public NewDatabaseCommand(DBManager dbm) { DBM = dbm; }

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
        DBM.NewDatabase();
    }
}
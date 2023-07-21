using Aion.ViewModels.Utility;
using System;
using System.Windows.Input;

namespace Aion.ViewModels.Commands;


public class CopyDatabaseCommand : ICommand
{
    public DBManager VM { get; set; }

    public CopyDatabaseCommand(DBManager vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.CopyDatabase();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
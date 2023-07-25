using Olympus.ViewModels.Utility;
using System;
using System.Windows.Input;

namespace Olympus.ViewModels.Commands;


public class ChangeDatabaseCommand : ICommand
{
    public DBManager VM { get; set; }

    public ChangeDatabaseCommand(DBManager vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public async void Execute(object? parameter)
    {
        await VM.ChangeDatabase();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
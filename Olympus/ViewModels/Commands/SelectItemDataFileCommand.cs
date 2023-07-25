using Olympus.ViewModels.Utility;
using System;
using System.Windows.Input;

namespace Olympus.ViewModels.Commands;

public class SelectItemDataFileCommand : ICommand
{
    public DBManager VM { get; set; }

    public SelectItemDataFileCommand(DBManager vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.SelectItemDataFile();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
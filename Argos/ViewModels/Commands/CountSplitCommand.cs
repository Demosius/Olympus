using System;
using System.Windows.Input;
using Argos.Interfaces;

namespace Argos.ViewModels.Commands;

public class CountSplitCommand : ICommand
{
    public IBatchTOGroupHandler VM { get; set; }

    public CountSplitCommand(IBatchTOGroupHandler vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedGroup is not null && VM.SelectedGroups.Count <= 1;
    }

    public void Execute(object? parameter)
    {
        VM.CountSplit();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
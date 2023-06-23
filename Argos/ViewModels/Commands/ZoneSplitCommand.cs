using System;
using System.Windows.Input;
using Argos.Interfaces;

namespace Argos.ViewModels.Commands;

public class ZoneSplitCommand : ICommand
{
    public IBatchTOGroupHandler VM { get; set; }

    public ZoneSplitCommand(IBatchTOGroupHandler vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedGroup is not null && VM.SelectedGroups.Count <= 1;
    }

    public void Execute(object? parameter)
    {
        VM.ZoneSplit();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
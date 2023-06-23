using System;
using System.Windows.Input;
using Argos.Interfaces;

namespace Argos.ViewModels.Commands;

public class BaySplitCommand : ICommand
{
    public IBatchTOGroupHandler VM { get; set; }

    public BaySplitCommand(IBatchTOGroupHandler vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedGroup is not null && VM.SelectedGroups.Count <= 1;
    }

    public async void Execute(object? parameter)
    {
        await VM.BaySplit();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
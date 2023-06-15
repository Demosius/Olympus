using System;
using System.Windows.Input;
using Argos.ViewModels.Components;

namespace Argos.ViewModels.Commands;

public class ProcessBatchLabelsCommand : ICommand
{
    public CCNCommandVM VM { get; set; }

    public ProcessBatchLabelsCommand(CCNCommandVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedGroup is not null && VM.SelectedGroups.Count <= 1;
    }

    public async void Execute(object? parameter)
    {
        await VM.ProcessBatchLabelsAsync();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
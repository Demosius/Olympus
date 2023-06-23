using System;
using System.Windows.Input;
using Argos.ViewModels.Components;

namespace Argos.ViewModels.Commands;

public class SetPriorityFillProgressCommand : ICommand
{
    public MainBatchVM VM { get; set; }

    public SetPriorityFillProgressCommand(MainBatchVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedBatch is not null && VM.SelectedBatches.Count == 1;
    }

    public async void Execute(object? parameter)
    {
        await VM.SetPriorityFillProgressAsync();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
using System;
using System.Windows.Input;
using Argos.ViewModels.Components;

namespace Argos.ViewModels.Commands;

public class MultiAddTagCommand : ICommand
{
    public MainBatchVM VM { get; set; }

    public MultiAddTagCommand(MainBatchVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedBatches.Count > 1;
    }

    public async void Execute(object? parameter)
    {
        await VM.MultiAddTagAsync();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
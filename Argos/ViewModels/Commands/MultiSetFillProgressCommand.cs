using System;
using System.Windows.Input;
using Argos.ViewModels.Components;

namespace Argos.ViewModels.Commands;

public class MultiSetFillProgressCommand : ICommand
{
    public MainBatchVM VM { get; set; }

    public MultiSetFillProgressCommand(MainBatchVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedBatches.Count > 1;
    }

    public async void Execute(object? parameter)
    {
        await VM.MultiSetFillProgressAsync();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
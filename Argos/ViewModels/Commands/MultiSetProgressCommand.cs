using System;
using System.Windows.Input;
using Argos.ViewModels.Components;

namespace Argos.ViewModels.Commands;

public class MultiSetProgressCommand : ICommand
{
    public MainBatchVM VM { get; set; }

    public MultiSetProgressCommand(MainBatchVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedBatches.Count > 1;
    }

    public async void Execute(object? parameter)
    {
        await VM.MultiSetProgressAsync();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
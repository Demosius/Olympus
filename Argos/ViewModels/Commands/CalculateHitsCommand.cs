using System;
using System.Windows.Input;
using Argos.ViewModels.Components;

namespace Argos.ViewModels.Commands;

public class CalculateHitsCommand : ICommand
{
    public MainBatchVM VM { get; set; }

    public CalculateHitsCommand(MainBatchVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedBatch is not null;
    }

    public async void Execute(object? parameter)
    {
        await VM.CalculateHitsAsync();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
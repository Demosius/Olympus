using System;
using System.Windows.Input;
using Argos.ViewModels.Components;

namespace Olympus.ViewModels.Commands;

public class UploadBatchesCommand : ICommand
{
    public MainBatchVM VM { get; set; }

    public UploadBatchesCommand(MainBatchVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public async void Execute(object? parameter)
    {
        await VM.UploadBatchesAsync();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
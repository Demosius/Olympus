using System;
using System.Windows.Input;
using Argos.ViewModels.Components;

namespace Argos.ViewModels.Commands;

public class UploadPickLinesCommand : ICommand
{
    public MainBatchVM VM { get; set; }

    public UploadPickLinesCommand(MainBatchVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public async void Execute(object? parameter)
    {
        await VM.UploadPickLinesAsync();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
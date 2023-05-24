using System;
using System.Windows.Input;

namespace Deimos.ViewModels.Commands;

public class UploadMispickDataCommand : ICommand
{
    public DeimosVM VM { get; set; }

    public UploadMispickDataCommand(DeimosVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public async void Execute(object? parameter)
    {
        await VM.UploadMispickData();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
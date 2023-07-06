using System;
using System.Windows.Input;

namespace Deimos.ViewModels.Commands;

public class UploadQACartonsCommand : ICommand
{
    public DeimosVM VM { get; set; }

    public UploadQACartonsCommand(DeimosVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public async void Execute(object? parameter)
    {
        await VM.UploadQACartons();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
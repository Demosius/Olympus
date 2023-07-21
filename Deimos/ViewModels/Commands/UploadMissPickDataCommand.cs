using System;
using System.Windows.Input;
using Deimos.Interfaces;

namespace Deimos.ViewModels.Commands;

public class UploadMispickDataCommand : ICommand
{
    public IMispickData VM { get; set; }

    public UploadMispickDataCommand(IMispickData vm) { VM = vm; }

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
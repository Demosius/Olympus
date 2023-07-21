using System;
using System.Windows.Input;
using Deimos.Interfaces;

namespace Deimos.ViewModels.Commands;

public class UploadPickEventsCommand : ICommand
{
    public IPickEvents VM { get; set; }

    public UploadPickEventsCommand(IPickEvents vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public async void Execute(object? parameter)
    {
        await VM.UploadPickEvents();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
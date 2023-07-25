using Morpheus.ViewModels.Controls;
using System;
using System.Windows.Input;

namespace Morpheus.ViewModels.Commands;

public class UploadZonesCommand : ICommand
{
    public ZoneHandlerVM VM { get; set; }

    public UploadZonesCommand(ZoneHandlerVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public async void Execute(object? parameter)
    {
        await VM.UploadZones();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
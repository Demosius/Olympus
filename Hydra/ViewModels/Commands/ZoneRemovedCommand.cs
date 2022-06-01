using Hydra.ViewModels.Controls;
using System;
using System.Windows.Input;

namespace Hydra.ViewModels.Commands;


public class ZoneRemovedCommand : ICommand
{
    public ZoneListingVM VM { get; set; }

    public ZoneRemovedCommand(ZoneListingVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.RemoveZone(VM.RemovedZoneVM!);
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}

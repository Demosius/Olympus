using Hydra.ViewModels.Controls;
using System;
using System.Windows.Input;

namespace Hydra.ViewModels.Commands;


public class ZoneReceivedCommand : ICommand
{
    public ZoneListingVM VM { get; set; }

    public ZoneReceivedCommand(ZoneListingVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.AddZone(VM.IncomingZoneVM!);
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
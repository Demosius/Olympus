using Hydra.ViewModels.Controls;
using System;
using System.Windows.Input;

namespace Hydra.ViewModels.Commands;


public class ZoneInsertedCommand : ICommand
{
    public ZoneListingVM VM { get; set; }

    public ZoneInsertedCommand(ZoneListingVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.InsertZone(VM.InsertedZoneVM!, VM.TargetZoneVM!);
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}

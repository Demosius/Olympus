using Hydra.ViewModel.Controls;
using System;
using System.Windows.Input;

namespace Hydra.ViewModel.Commands;

public class UpdateZonesCommand : ICommand
{
    public ZoneHandlerVM VM { get; set; }

    public UpdateZonesCommand(ZoneHandlerVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.UpdateZones();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
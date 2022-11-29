using System;
using System.Windows.Input;

namespace Morpheus.ViewModels.Commands;

public class SaveZonesCommand : ICommand
{
    public ZoneHandlerVM VM { get; set; }

    public SaveZonesCommand(ZoneHandlerVM vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.SaveZones();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
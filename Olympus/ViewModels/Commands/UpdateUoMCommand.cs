using Olympus.ViewModels.Components;
using System;
using System.Windows.Input;

namespace Olympus.ViewModels.Commands;

public class UpdateUoMCommand : ICommand
{
    public InventoryUpdaterVM VM { get; set; }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public UpdateUoMCommand(InventoryUpdaterVM vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public async void Execute(object? parameter)
    {
        await VM.UpdateUoM();
    }
}
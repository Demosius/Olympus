using Olympus.ViewModels.Components;
using System;
using System.Windows.Input;

namespace Olympus.ViewModels.Commands;

public class UpdateBinsCommand : ICommand
{
    public InventoryUpdaterVM VM { get; set; }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public UpdateBinsCommand(InventoryUpdaterVM vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public async void Execute(object? parameter)
    {
        await VM.UpdateBins();
    }
}
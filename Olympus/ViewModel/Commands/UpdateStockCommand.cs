using Olympus.ViewModel.Components;
using System;
using System.Windows.Input;

namespace Olympus.ViewModel.Commands;

public class UpdateStockCommand : ICommand
{
    public InventoryUpdaterVM VM { get; set; }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public UpdateStockCommand(InventoryUpdaterVM vm)
    {
        VM = vm;
    }

    public bool CanExecute(object parameter)
    {
        return true;
    }

    public void Execute(object parameter)
    {
        VM.UpdateStock();
    }
}
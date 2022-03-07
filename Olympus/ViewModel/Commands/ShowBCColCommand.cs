using Olympus.ViewModel.Components;
using System;
using System.Windows.Input;

namespace Olympus.ViewModel.Commands;

public class ShowBcColCommand : ICommand
{
    public InventoryUpdaterVM VM { get; set; }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public ShowBcColCommand(InventoryUpdaterVM vm)
    {
        VM = vm;
    }

    public bool CanExecute(object parameter)
    {
        return true;
    }

    public void Execute(object parameter)
    {
        InventoryUpdaterVM.BcInfo();
    }
}
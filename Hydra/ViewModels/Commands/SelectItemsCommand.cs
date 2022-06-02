using Hydra.ViewModels.Controls;
using System;
using System.Windows.Input;

namespace Hydra.ViewModels.Commands;


public class SelectItemsCommand : ICommand
{
    public ItemLevelsVM VM { get; set; }

    public SelectItemsCommand(ItemLevelsVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.SelectItems();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
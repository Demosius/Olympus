using Hydra.Interfaces;
using System;
using System.Windows.Input;

namespace Hydra.ViewModels.Commands;


public class FilterItemsFromClipboardCommand : ICommand
{
    public IItemDataVM VM { get; set; }

    public FilterItemsFromClipboardCommand(IItemDataVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.FilterItemsFromClipboard();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
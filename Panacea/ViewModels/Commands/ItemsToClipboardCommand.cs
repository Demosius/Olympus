using Panacea.Interfaces;
using System;
using System.Windows.Input;

namespace Panacea.ViewModels.Commands;


public class ItemsToClipboardCommand : ICommand
{
    public IItemData VM { get; set; }

    public ItemsToClipboardCommand(IItemData vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.ItemsToClipboard();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
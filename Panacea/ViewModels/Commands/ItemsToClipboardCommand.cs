using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Panacea.Interfaces;
using Panacea.ViewModels.Components;

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
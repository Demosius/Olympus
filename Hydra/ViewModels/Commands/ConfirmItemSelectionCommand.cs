using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hydra.ViewModels.Commands;


public class ConfirmItemSelectionCommand : ICommand
{
    public ItemSelectionVM VM { get; set; }

    public ConfirmItemSelectionCommand(ItemSelectionVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.ConfirmItemSelection();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
using System;
using System.Windows.Input;
using Morpheus.ViewModels.Controls.Inventory;

namespace Morpheus.ViewModels.Commands;


public class AddMixCtnItemCommand : ICommand
{
    public MixedCartonHandlerVM VM { get; set; }

    public AddMixCtnItemCommand(MixedCartonHandlerVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedMixedCarton is not null;
    }

    public void Execute(object? parameter)
    {
        VM.AddMixCtnItem();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
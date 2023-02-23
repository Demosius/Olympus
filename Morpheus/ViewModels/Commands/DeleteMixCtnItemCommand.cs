using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Morpheus.ViewModels.Controls;

namespace Morpheus.ViewModels.Commands;


public class DeleteMixCtnItemCommand : ICommand
{
    public MixedCartonHandlerVM VM { get; set; }

    public DeleteMixCtnItemCommand(MixedCartonHandlerVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedMixCtnItem is not null;
    }

    public void Execute(object? parameter)
    {
        VM.DeleteMixCtnItem();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
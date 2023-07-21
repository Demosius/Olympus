using System;
using System.Windows.Input;
using Morpheus.ViewModels.Controls;

namespace Morpheus.ViewModels.Commands;


public class DeleteMixedCartonCommand : ICommand
{
    public MixedCartonHandlerVM VM { get; set; }

    public DeleteMixedCartonCommand(MixedCartonHandlerVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedMixedCarton is not null;
    }

    public void Execute(object? parameter)
    {
        VM.DeleteMixedCarton();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
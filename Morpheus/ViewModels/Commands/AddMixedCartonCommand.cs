using System;
using System.Windows.Input;
using Morpheus.ViewModels.Controls;

namespace Morpheus.ViewModels.Commands;


public class AddMixedCartonCommand : ICommand
{
    public MixedCartonHandlerVM VM { get; set; }

    public AddMixedCartonCommand(MixedCartonHandlerVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.AddMixedCarton();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
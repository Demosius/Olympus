using Morpheus.ViewModels.Controls;
using System;
using System.Windows.Input;

namespace Morpheus.ViewModels.Commands;


public class SaveMixedCartonsCommand : ICommand
{
    public MixedCartonHandlerVM VM { get; set; }

    public SaveMixedCartonsCommand(MixedCartonHandlerVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public async void Execute(object? parameter)
    {
        await VM.SaveMixedCartons();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
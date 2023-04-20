using System;
using System.Windows.Input;
using Morpheus.ViewModels.Interfaces;

namespace Morpheus.ViewModels.Commands;


public class DeselectAllCommand : ICommand
{
    public IMultiSelect VM { get; set; }

    public DeselectAllCommand(IMultiSelect vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.DeselectAll();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
using System;
using System.Windows.Input;
using Panacea.ViewModels.Components;

namespace Panacea.ViewModels.Commands;


public class UpdateTOLinesCommand : ICommand
{
    public ItemsWithNoPickBinVM VM { get; set; }

    public UpdateTOLinesCommand(ItemsWithNoPickBinVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.UpdateTOLines();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
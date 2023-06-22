using System;
using System.Windows.Input;
using Argos.ViewModels.Components;

namespace Argos.ViewModels.Commands;

public class ShowPickLinesCommand : ICommand
{
    public BatchVM VM { get; set; }

    public ShowPickLinesCommand(BatchVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.ShowPickLines();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
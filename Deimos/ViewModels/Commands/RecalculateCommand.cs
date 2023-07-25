using System;
using System.Windows.Input;
using Deimos.Interfaces;

namespace Deimos.ViewModels.Commands;

public class RecalculateCommand : ICommand
{
    public IRecalculate VM { get; set; }

    public RecalculateCommand(IRecalculate vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.Recalculate(parameter);
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
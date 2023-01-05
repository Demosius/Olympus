using System;
using System.Windows.Input;
using Panacea.Interfaces;

namespace Panacea.ViewModels.Commands;


public class RunChecksCommand : ICommand
{
    public IChecker VM { get; set; }

    public RunChecksCommand(IChecker vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.RunChecks();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
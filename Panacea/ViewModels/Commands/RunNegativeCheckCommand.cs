using Panacea.ViewModels.Components;
using System;
using System.Windows.Input;

namespace Panacea.ViewModels.Commands;


public class RunNegativeChecksCommand : ICommand
{
    public NegativeCheckerVM VM { get; set; }

    public RunNegativeChecksCommand(NegativeCheckerVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.RunNegativeChecks();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
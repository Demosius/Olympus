using Panacea.ViewModels.Components;
using System;
using System.Windows.Input;

namespace Panacea.ViewModels.Commands;


public class RunPotentialNegativesCheckCommand : ICommand
{
    public PotentialNegativeVM VM { get; set; }

    public RunPotentialNegativesCheckCommand(PotentialNegativeVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.RunPotentialNegativesCheck();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
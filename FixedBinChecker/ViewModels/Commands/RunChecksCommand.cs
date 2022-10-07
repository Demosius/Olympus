using System;
using System.Windows.Input;

namespace FixedBinChecker.ViewModels.Commands;

public class RunChecksCommand : ICommand
{
    public FixedBinCheckerVM VM { get; set; }

    public RunChecksCommand(FixedBinCheckerVM vm) { VM = vm; }

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
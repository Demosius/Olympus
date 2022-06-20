using Hydra.ViewModels.Controls;
using System;
using System.Linq;
using System.Windows.Input;

namespace Hydra.ViewModels.Commands;

public class ExportToCSVCommand : ICommand
{
    public RunVM VM { get; set; }

    public ExportToCSVCommand(RunVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.CurrentMoves.Any();
    }

    public void Execute(object? parameter)
    {
        VM.ExportToCSV();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
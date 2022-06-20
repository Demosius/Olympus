using Hydra.ViewModels.Controls;
using System;
using System.Linq;
using System.Windows.Input;


namespace Hydra.ViewModels.Commands;


public class ExportToPDFCommand : ICommand
{
    public RunVM VM { get; set; }

    public ExportToPDFCommand(RunVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.CurrentMoves.Any();
    }

    public void Execute(object? parameter)
    {
        VM.ExportToPDF();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Hydra.ViewModels.Controls;

namespace Hydra.ViewModels.Commands;


public class GenerateMovesCommand : ICommand
{
    public RunVM VM { get; set; }

    public GenerateMovesCommand(RunVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.GenerateMoves();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
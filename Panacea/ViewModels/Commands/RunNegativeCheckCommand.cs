using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Panacea.ViewModels.Components;

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
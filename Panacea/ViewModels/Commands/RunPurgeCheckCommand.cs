using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Panacea.ViewModels.Components;

namespace Panacea.ViewModels.Commands;


public class RunPurgeCheckCommand : ICommand
{
    public PurgeVM VM { get; set; }

    public RunPurgeCheckCommand(PurgeVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.RunPurgeCheck();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
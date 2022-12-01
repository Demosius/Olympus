using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Panacea.ViewModels.Components;

namespace Panacea.ViewModels.Commands;


public class RunIWMBChecksCommand : ICommand
{
    public ItemsWithMultipleBinsVM VM { get; set; }

    public RunIWMBChecksCommand(ItemsWithMultipleBinsVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
    return true;
    }

    public void Execute(object? parameter)
    {
        VM.RunIWMBChecks();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
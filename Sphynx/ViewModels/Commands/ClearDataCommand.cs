using System;
using System.Windows.Input;
using Sphynx.ViewModels.Controls;

namespace Sphynx.ViewModels.Commands;

public class ClearDataCommand : ICommand
{
    public AutoCounterVM VM { get; set; }

    public ClearDataCommand(AutoCounterVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.ClearData();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
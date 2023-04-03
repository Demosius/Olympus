using Cadmus.Interfaces;
using System;
using System.Windows.Input;

namespace Cadmus.ViewModels.Commands;


public class PrintCommand : ICommand
{
    public IPrintable VM { get; set; }

    public PrintCommand(IPrintable vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.Print();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
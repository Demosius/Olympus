using System;
using System.Windows.Input;

namespace Cadmus.ViewModels.Commands;


public class ShowPrinterCommand : ICommand
{
    public IPrintable VM { get; set; }

    public ShowPrinterCommand(IPrintable vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.ShowPrinter();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
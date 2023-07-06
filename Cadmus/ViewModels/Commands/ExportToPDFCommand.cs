using System;
using System.Windows.Input;
using Cadmus.Interfaces;

namespace Cadmus.ViewModels.Commands;

public class ExportToPDFCommand : ICommand
{
    public IExport VM { get; set; }

    public ExportToPDFCommand(IExport vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.CanExport;
    }

    public async void Execute(object? parameter)
    {
        await VM.ExportToPDF();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
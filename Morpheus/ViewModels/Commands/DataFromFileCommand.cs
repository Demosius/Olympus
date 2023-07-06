using System;
using System.Windows.Input;
using Morpheus.ViewModels.Interfaces;

namespace Morpheus.ViewModels.Commands;

public class DataFromFileCommand : ICommand
{
    public IFileUpload VM { get; set; }

    public DataFromFileCommand(IFileUpload vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.DataFromFileAsync();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
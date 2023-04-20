using System;
using System.Windows.Input;
using Morpheus.ViewModels.Interfaces;

namespace Morpheus.ViewModels.Commands;


public class CreateCommand : ICommand
{
    public ISelector VM { get; set; }

    public CreateCommand(ISelector vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.CanCreate;
    }

    public void Execute(object? parameter)
    {
        VM.Create();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
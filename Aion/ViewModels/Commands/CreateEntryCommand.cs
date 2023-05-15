using System;
using System.Windows.Input;

namespace Aion.ViewModels.Commands;


public class CreateEntryCommand : ICommand
{
    public EntryCreationVM VM { get; set; }

    public CreateEntryCommand(EntryCreationVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.CreateEntry();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
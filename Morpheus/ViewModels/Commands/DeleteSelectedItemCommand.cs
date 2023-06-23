using System;
using System.Windows.Input;
using Morpheus.ViewModels.Interfaces;

namespace Morpheus.ViewModels.Commands;

public class DeleteSelectedItemCommand<T> : ICommand
{
    public IDelete<T> VM { get; set; }

    public DeleteSelectedItemCommand(IDelete<T> vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedItem is not null;
    }

    public void Execute(object? parameter)
    {
        VM.DeleteSelectedItemAsync();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
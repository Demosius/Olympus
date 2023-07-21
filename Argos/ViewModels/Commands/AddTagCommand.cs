using System;
using System.Windows.Input;
using Argos.ViewModels.Components;

namespace Argos.ViewModels.Commands;

public class AddTagCommand : ICommand
{
    public BatchVM VM { get; set; }

    public AddTagCommand(BatchVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public async void Execute(object? parameter)
    {
       await VM.AddTagAsync();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
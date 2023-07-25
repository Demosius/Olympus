using System;
using System.Windows.Input;
using Argos.ViewModels.Components;

namespace Argos.ViewModels.Commands;

public class DeleteTagCommand : ICommand
{
    public BatchVM VM { get; set; }

    public DeleteTagCommand(BatchVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedTag is not null;
    }

    public async void Execute(object? parameter)
    {
        await VM.DeleteTagAsync();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
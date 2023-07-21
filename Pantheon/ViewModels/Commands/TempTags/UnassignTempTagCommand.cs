using System;
using System.Windows.Input;
using Pantheon.ViewModels.Interfaces;

namespace Pantheon.ViewModels.Commands.TempTags;


public class UnassignTempTagCommand : ICommand
{
    public ITempTags VM { get; set; }

    public UnassignTempTagCommand(ITempTags vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.CanUnassign;
    }

    public void Execute(object? parameter)
    {
        VM.UnassignTempTag();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
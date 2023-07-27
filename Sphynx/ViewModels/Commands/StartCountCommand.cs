using System;
using System.Windows.Input;
using Sphynx.ViewModels.Controls;

namespace Sphynx.ViewModels.Commands;

public class StartCountCommand : ICommand
{
    public AutoCounterVM VM { get; set; }

    public StartCountCommand(AutoCounterVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public async void Execute(object? parameter)
    {
        await VM.StartCount();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
using System;
using System.Windows.Input;
using Hermes.ViewModels.Controls;

namespace Hermes.ViewModels.Commands;

public class GenerateEmptyBinListCommand : ICommand
{
    public AutoCounterVM VM { get; set; }

    public GenerateEmptyBinListCommand(AutoCounterVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public async void Execute(object? parameter)
    {
        await VM.GenerateEmptyBinList();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
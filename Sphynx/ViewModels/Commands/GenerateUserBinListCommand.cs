using System;
using System.Windows.Input;
using Sphynx.ViewModels.Controls;

namespace Sphynx.ViewModels.Commands;

public class GenerateUserBinListCommand : ICommand
{
    public AutoCounterVM VM { get; set; }

    public GenerateUserBinListCommand(AutoCounterVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public async void Execute(object? parameter)
    {
        await VM.GenerateUserBinList();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
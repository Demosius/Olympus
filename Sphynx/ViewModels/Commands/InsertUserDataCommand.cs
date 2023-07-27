using System;
using System.Windows.Input;
using Sphynx.ViewModels.Controls;

namespace Sphynx.ViewModels.Commands;

public class InsertUserDataCommand : ICommand
{
    public AutoCounterVM VM { get; set; }

    public InsertUserDataCommand(AutoCounterVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.InsertUserData();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
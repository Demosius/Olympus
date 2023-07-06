using System;
using System.Windows.Controls;
using System.Windows.Input;
using Deimos.ViewModels.Controls;

namespace Deimos.ViewModels.Commands;

public class SetBlackoutCommand : ICommand
{
    public QAErrorManagementVM VM { get; set; }

    public SetBlackoutCommand(QAErrorManagementVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedQALine is not null;
    }

    public async void Execute(object? parameter)
    {
        await VM.SetBlackoutAsync();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
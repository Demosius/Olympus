using System;
using System.Windows.Input;
using Deimos.ViewModels.Controls;

namespace Deimos.ViewModels.Commands;

public class SetBackoutCommand : ICommand
{
    public QAErrorManagementVM VM { get; set; }

    public SetBackoutCommand(QAErrorManagementVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedQALine is not null;
    }

    public async void Execute(object? parameter)
    {
        await VM.SetBackoutAsync();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
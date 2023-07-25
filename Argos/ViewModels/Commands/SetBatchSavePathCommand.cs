using System;
using System.Windows.Input;
using Argos.ViewModels.PopUps;

namespace Argos.ViewModels.Commands;

public class SetBatchSavePathCommand : ICommand
{
    public FileLocationMenuVM VM { get; set; }

    public SetBatchSavePathCommand(FileLocationMenuVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.SetBatchSavePath();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
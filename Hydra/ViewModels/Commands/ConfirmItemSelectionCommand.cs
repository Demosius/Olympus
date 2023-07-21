using Hydra.ViewModels.PopUps;
using System;
using System.Windows;
using System.Windows.Input;

namespace Hydra.ViewModels.Commands;


public class ConfirmItemSelectionCommand : ICommand
{
    public ItemSelectionVM VM { get; set; }

    public ConfirmItemSelectionCommand(ItemSelectionVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public async void Execute(object? parameter)
    {
        if (parameter is not Window window) return;
        Mouse.OverrideCursor = Cursors.Wait;
        await VM.ConfirmItemSelection();
        Mouse.OverrideCursor = Cursors.Arrow;
        window.DialogResult = true;
        window.Close();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
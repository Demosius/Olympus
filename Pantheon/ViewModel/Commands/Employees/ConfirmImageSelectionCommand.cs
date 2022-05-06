using System;
using System.Windows;
using System.Windows.Input;
using Pantheon.ViewModel.Interface;

namespace Pantheon.ViewModel.Commands.Employees;

internal class ConfirmImageSelectionCommand : ICommand
{
    public IImageSelector VM { get; set; }

    public ConfirmImageSelectionCommand(IImageSelector vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedImage is not null;
    }

    public void Execute(object? parameter)
    {
        if (parameter is not Window w) return;
        VM.ConfirmImageSelection();
        w.Close();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
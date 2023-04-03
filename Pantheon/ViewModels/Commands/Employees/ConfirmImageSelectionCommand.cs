using System;
using System.Windows;
using System.Windows.Input;
using Pantheon.ViewModels.Interface;

namespace Pantheon.ViewModels.Commands.Employees;

public class ConfirmImageSelectionCommand : ICommand
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
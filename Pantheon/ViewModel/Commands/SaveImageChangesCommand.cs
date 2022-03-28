using Pantheon.ViewModel.Interface;
using System;
using System.Windows.Input;

namespace Pantheon.ViewModel.Commands;

internal class SaveImageChangesCommand : ICommand
{
    public IImageSelector VM { get; set; }

    public SaveImageChangesCommand(IImageSelector vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.CanSaveImage;
    }

    public void Execute(object? parameter)
    {
        VM.SaveImageChanges();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
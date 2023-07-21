using Hydra.ViewModels.PopUps;
using System;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace Hydra.ViewModels.Commands;

public class ConfirmSiteChangesCommand : ICommand
{
    public SiteManagementVM VM { get; set; }

    public ConfirmSiteChangesCommand(SiteManagementVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public async void Execute(object? parameter)
    {
        if (parameter is not Window window) throw new DataException("No appropriate parameter passed to site management command.");
        await VM.ConfirmSiteChanges();
        window.DialogResult = true;
        window.Close();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
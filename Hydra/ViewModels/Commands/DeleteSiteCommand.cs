using Hydra.ViewModels.Controls;
using System;
using System.Data;
using System.Windows.Input;

namespace Hydra.ViewModels.Commands;


public class DeleteSiteCommand : ICommand
{
    public SiteManagerVM VM { get; set; }

    public DeleteSiteCommand(SiteManagerVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        if (parameter is not SiteVM siteVM) throw new DataException("No appropriate parameter passed to delete.");
        VM.DeleteSite(siteVM);
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
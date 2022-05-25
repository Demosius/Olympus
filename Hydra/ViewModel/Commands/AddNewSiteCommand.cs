using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Hydra.ViewModel.Controls;

namespace Hydra.ViewModel.Commands;

public class AddNewSiteCommand : ICommand
{
    public SiteManagerVM VM { get; set; }

    public AddNewSiteCommand(SiteManagerVM vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.AddNewSite();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
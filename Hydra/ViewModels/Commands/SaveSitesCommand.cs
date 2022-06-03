using Hydra.ViewModels.Controls;
using System;
using System.Windows.Input;

namespace Hydra.ViewModels.Commands
{

    public class SaveSitesCommand : ICommand
    {
        public SiteManagerVM VM { get; set; }

        public SaveSitesCommand(SiteManagerVM vm) { VM = vm; }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            VM.SaveSites();
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}

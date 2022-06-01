using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Hydra.ViewModels.Controls;

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

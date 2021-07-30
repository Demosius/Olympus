using Olympus.ViewModel.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Olympus.ViewModel.Commands
{
    public class MoveDatabaseCommand : ICommand
    {
        public DBSelectionVM VM { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public MoveDatabaseCommand(DBSelectionVM vm)
        {
            VM = vm;
        }

        public bool CanExecute(object parameter)
        {
            string dbLocation = (parameter ?? "null") as string;
            return App.Charon.CanMoveDatabase() && dbLocation.ToLower() != "local";
        }

        public void Execute(object parameter)
        {
            VM.MoveDatabase();
        }
    }
}

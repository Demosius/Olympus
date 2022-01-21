using Olympus.ViewModel.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Olympus.ViewModel.Commands
{
    public class NewDatabaseCommand : ICommand
    {
        public DBManager DBM { get; set; }

        public NewDatabaseCommand(DBManager dbm) { DBM = dbm; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            DBM.NewDatabase();
        }
    }
}

using Olympus.ViewModel.Utility;
using System;
using System.Windows.Input;

namespace Olympus.ViewModel.Commands
{
    public class MergeDatabaseCommand : ICommand
    {
        public DBManager Dbm { get; set; }

        public MergeDatabaseCommand(DBManager dbm) { Dbm = dbm; }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            DBManager.MergeDatabase();
        }
    }
}

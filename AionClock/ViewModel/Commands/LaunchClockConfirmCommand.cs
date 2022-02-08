using AionClock.View;
using System;
using System.Windows.Input;
using Uranus.Staff.Model;

namespace AionClock.ViewModel.Commands
{
    public class LaunchClockConfirmCommand : ICommand
    {
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
            ClockConfirmationView confirmationView = new(parameter as Employee);
            confirmationView.ShowDialog();
        }
    }
}

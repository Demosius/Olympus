using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Olympus.Styx.ViewModel.Commands
{
    public class AlphaRegisterCommand : ICommand
    {
        public AlphaRegistrationVM VM { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public AlphaRegisterCommand(AlphaRegistrationVM vm)
        {
            VM = vm;
        }

        public bool CanExecute(object parameter)
        {
            return VM.PasswordGood && VM.Employee.ID > 0 && 
                   VM.Employee.DisplayName != "" && 
                   VM.Department.Name != "" && VM.Role.Name != "";
        }

        public void Execute(object parameter)
        {
            VM.Register();
        }
    }
}

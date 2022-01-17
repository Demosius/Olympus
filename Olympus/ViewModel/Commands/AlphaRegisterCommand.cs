using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Olympus.ViewModel.Commands
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
            if (VM.Register(out string message))
            {
                Window window = parameter as Window;
                window.Close();
            }
            // TODO: Improve this message process.
            MessageBox.Show(message);
        }
    }
}

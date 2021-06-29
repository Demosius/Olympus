using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Olympus.Charon
{
    /// <summary>
    /// Interaction logic for LogIn.xaml
    /// </summary>
    public partial class LogIn : Window
    {
        public LogIn()
        {
            InitializeComponent();
        }

        private void BtnLogIn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void LogInCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = TxtUser.Text.Length > 1 && PwdPassword.Password.Length > 1;
        }

        private void LogInCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show(PwdPassword.Password);
        }
    }

    /// <summary>
    ///  Static Command class for custom commands.
    /// </summary>
    public static partial class Commands
    {
        public static readonly RoutedUICommand LogIn = new RoutedUICommand
            (
                "Log In",
                "Log_In",
                typeof(Commands)
            );
    }
}

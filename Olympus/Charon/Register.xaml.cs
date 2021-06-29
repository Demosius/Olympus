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
    /// Interaction logic for Registerxaml.xaml
    /// </summary>
    public partial class Register : Window
    {
        public Register()
        {
            InitializeComponent();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void RegisterCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = TxtUser.Text.Length > 1 && PwdFirst.Password.Length >= 6 && PwdFirst.Password == PwdSecond.Password;
        }

        private void RegisterCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show(PwdFirst.Password);
        }


        private void ExitCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ExitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }

    /// <summary>
    ///  Static Command class for custom commands.
    /// </summary>
    public static partial class Commands
    {
        public static readonly RoutedUICommand Register = new RoutedUICommand
            ( 
                "Register",
                "Register",
                typeof(Commands)
            );

        public static readonly RoutedUICommand Exit = new RoutedUICommand
            (
                "Exit",
                "Exit",
                typeof(Commands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F4, ModifierKeys.Alt)
                }
            );
    }
}


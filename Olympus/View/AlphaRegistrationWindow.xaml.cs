using System.Windows;
using System.Windows.Controls;

namespace Olympus.View
{
    /// <summary>
    /// Interaction logic for AlphaRegistrationWindow.xaml
    /// </summary>
    public partial class AlphaRegistrationWindow : Window
    {
        public AlphaRegistrationWindow()
        {
            InitializeComponent(); 
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (VM != null) { VM.Password = ((PasswordBox)sender).Password; }
        }

        private void PasswordBox_CnfrmPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (VM != null) { VM.ConfirmPassword = ((PasswordBox)sender).Password; }
        }
    }
}

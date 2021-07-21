using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Olympus.Styx.Model;
using Olympus.Styx.View;

namespace Olympus.View.Components
{
    /// <summary>
    /// Interaction logic for SignIn.xaml
    /// </summary>
    public partial class UserHandler : UserControl
    {
        public Charon Charon { get; set; }

        public UserHandler()
        {
            InitializeComponent();
            Charon = new Charon();
        }
         
        private void BtnSignIn_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow login = new LoginWindow();
            login.ShowDialog();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            int userCount = App.Charioteer.UserReader.UserCount();
            if (userCount == 0)
            {
                AlphaRegistrationWindow regForm = new AlphaRegistrationWindow();
                regForm.ShowDialog();
            }
            else
            {
                RegisterWindow regForm = new RegisterWindow();
                regForm.ShowDialog();
            }
        }
    }
}

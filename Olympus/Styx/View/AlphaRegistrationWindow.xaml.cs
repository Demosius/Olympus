using Olympus.Styx.ViewModel;
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

namespace Olympus.Styx.View
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
            if (this.DataContext != null)
            { ((AlphaRegistrationVM)DataContext).Password = ((PasswordBox)sender).Password; }
        }

        private void PasswordBox_CnfrmPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            { ((AlphaRegistrationVM)DataContext).ConfirmPassword = ((PasswordBox)sender).Password; }
        }
    }
}

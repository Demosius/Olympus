using Olympus.Model;
using Uranus.Staff;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Olympus.Pantheon.View
{
    /// <summary>
    /// Interaction logic for PantheonPage.xaml
    /// </summary>
    public partial class PantheonPage : Page, IProject
    {
        public PantheonPage()
        {
            InitializeComponent();
        }

        public EProject EProject { get => EProject.Pantheon; }
    }
}

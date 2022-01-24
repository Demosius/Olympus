using Uranus.Staff;
using System;
using System.Windows.Controls;
using Uranus;

namespace Pantheon.View
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

        public bool RequiresUser => true;

        public void RefreshData()
        {
            throw new NotImplementedException();
        }
    }
}

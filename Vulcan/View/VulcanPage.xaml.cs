using Uranus.Staff;
using System;
using System.Windows.Controls;
using Uranus;

namespace Vulcan.View
{
    /// <summary>
    /// Interaction logic for VulcanPage.xaml
    /// </summary>
    public partial class VulcanPage : Page, IProject
    {
        public VulcanPage()
        {
            InitializeComponent();
        }

        public EProject EProject => EProject.Vulcan;

        public bool RequiresUser => false;

        public void RefreshData()
        {
            throw new NotImplementedException();
        }
    }
}

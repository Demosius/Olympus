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

        public EProject Project => EProject.Vulcan;

        public static bool RequiresUser => false;

        public void RefreshData()
        {
            throw new NotImplementedException();
        }
    }
}

using Uranus.Staff;
using System;
using Uranus;

namespace Pantheon.View
{
    /// <summary>
    /// Interaction logic for PantheonPage.xaml
    /// </summary>
    public partial class PantheonPage : IProject

    {
        public PantheonPage()
        {
            InitializeComponent();
        }

        public EProject Project => EProject.Pantheon;

        public static bool RequiresUser => true;

        public void RefreshData()
        {
            throw new NotImplementedException();
        }
    }
}

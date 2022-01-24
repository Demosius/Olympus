using Uranus.Staff;
using System;
using System.Windows.Controls;
using Uranus;

namespace Khaos.View
{
    /// <summary>
    /// Interaction logic for KhaosPage.xaml
    /// </summary>
    public partial class KhaosPage : Page, IProject
    {
        public KhaosPage()
        {
            InitializeComponent();
        }

        public EProject EProject => EProject.Khaos;

        public bool RequiresUser => false;

        public void RefreshData()
        {
            throw new NotImplementedException();
        }
    }
}

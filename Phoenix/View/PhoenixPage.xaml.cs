using Uranus.Staff;
using System;
using System.Windows.Controls;
using Uranus;

namespace Phoenix.View
{
    /// <summary>
    /// Interaction logic for TorchPage.xaml
    /// </summary>
    public partial class PhoenixPage : Page, IProject
    {
        public PhoenixPage()
        {
            InitializeComponent();
        }

        public EProject EProject => EProject.Phoenix;

        public bool RequiresUser => false;

        public void RefreshData()
        {
            throw new NotImplementedException();
        }
    }
}

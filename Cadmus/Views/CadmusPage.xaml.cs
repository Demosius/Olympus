using System;
using Uranus.Interfaces;
using Uranus.Staff;

namespace Cadmus.Views
{
    /// <summary>
    /// Interaction logic for CadmusPage.xaml
    /// </summary>
    public partial class CadmusPage : IProject
    {
        public CadmusPage()
        {
            InitializeComponent();
        }

        public EProject Project => EProject.Cadmus;

        public static bool RequiresUser => false;

        public void RefreshData()
        {
            throw new NotImplementedException();
        }
    }
}

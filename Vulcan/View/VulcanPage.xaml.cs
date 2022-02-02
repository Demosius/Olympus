using Uranus.Staff;
using Uranus;

namespace Vulcan.View
{
    /// <summary>
    /// Interaction logic for VulcanPage.xaml
    /// </summary>
    public partial class VulcanPage : IProject
    {
        public VulcanPage()
        {
            InitializeComponent();
        }

        public EProject Project => EProject.Vulcan;

        public static bool RequiresUser => false;

        public void RefreshData()
        {
            VM.RefreshData();
        }
    }
}

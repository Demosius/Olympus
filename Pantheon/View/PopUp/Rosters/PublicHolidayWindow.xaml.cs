using Pantheon.ViewModel.Controls.Rosters;
using Pantheon.ViewModel.PopUp.Rosters;

namespace Pantheon.View.PopUp.Rosters
{
    /// <summary>
    /// Interaction logic for PublicHolidayWindow.xaml
    /// </summary>
    public partial class PublicHolidayWindow
    {
        public PublicHolidayWindow(DepartmentRosterVM parentVM)
        {
            DataContext = new PublicHolidayVM(parentVM);
            InitializeComponent();
        }
    }
}

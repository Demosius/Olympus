using Pantheon.ViewModels.Controls.Rosters;
using Pantheon.ViewModels.PopUp.Rosters;

namespace Pantheon.Views.PopUp.Rosters;

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
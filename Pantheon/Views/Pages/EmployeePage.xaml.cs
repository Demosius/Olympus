using Styx;
using Uranus;

namespace Pantheon.Views.Pages;

/// <summary>
/// Interaction logic for EmployeePage.xaml
/// </summary>
public partial class EmployeePage
{
    public EmployeePage(Charon charon, Helios helios)
    {
        InitializeComponent();
        VM.SetDataSources(helios, charon);
    }
}
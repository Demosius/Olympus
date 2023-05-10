using Pantheon.ViewModels.Pages;
using Styx;
using Uranus;

namespace Pantheon.Views.Pages;

/// <summary>
/// Interaction logic for EmployeePage.xaml
/// </summary>
public partial class EmployeePage
{
    public EmployeePageVM VM { get; set; }

    public EmployeePage(Charon charon, Helios helios)
    {
        InitializeComponent();
        VM = new EmployeePageVM(helios, charon);
        DataContext = VM;
    }
}